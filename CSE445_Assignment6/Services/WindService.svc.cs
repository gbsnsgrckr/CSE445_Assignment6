using System;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace CSE445_Assignment6.WindService
{
    /// <summary>
    /// WCF service that returns the annual average wind speed (m/s) at 10 m
    /// for the user-selected US ZIP code
    /// **Modified from Assignment 4 submission
    /// </summary>
    public class WindService : IWindService
    {
        // ZIP conversion to lat/lon from NWS DWML
        private const string ZipToLatLonUrl =
            "https://graphical.weather.gov/xml/SOAP_server/ndfdXMLclient.php?listZipCodeList={0}";

        // Base API URL
        private const string PowerClimUrl =
            "https://power.larc.nasa.gov/api/temporal/climatology/point?parameters=WS10M&latitude={0}&longitude={1}&community=RE&format=JSON";

        // Extras URL
        private const string PowerClimUrlDetails =
            "https://power.larc.nasa.gov/api/temporal/climatology/point?parameters=WS10M,WS50M&community=RE&longitude={0}&latitude={1}&format=JSON";

        /// <summary>
        /// Returns annual average wind speed for a US ZIP code in m/s at 10 m.
        /// </summary>
        public decimal WindIntensityByZip(string zipcode)
        {
            if (string.IsNullOrWhiteSpace(zipcode))
            {
                throw new ArgumentException("Error: Zipcode is required.");
            }

            zipcode = zipcode.Trim();

            if (zipcode.Length != 5 || !zipcode.All(char.IsDigit))
            {
                throw new ArgumentException("Error: Zipcode must be 5 digits.");
            }

            // Convert ZIP to lat/lon
            var latlon = GetLatLonFromZip(zipcode);

            if (latlon == null)
            {
                throw new ArgumentException("Error: Invalid or unsupported ZIP code.");
            }

            return WindIntensity((decimal)latlon.Value.lat, (decimal)latlon.Value.lon);
        }

        /// <summary>
        /// Returns the annual average wind speed for the user-selected lat/lon within the U.S.
        /// </summary>
        public decimal WindIntensity(decimal latitude, decimal longitude)
        {
            Validate(latitude, longitude);

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string url = string.Format(CultureInfo.InvariantCulture, PowerClimUrl, latitude, longitude);
                string json = DownloadString(url);

                var serializer = new JavaScriptSerializer();
                var root = serializer.Deserialize<Dictionary<string, object>>(json);

                if (root == null)
                {
                    throw new Exception("Error: Empty or invalid JSON.");
                }

                // error
                if (root.TryGetValue("errors", out var errsObj))
                {
                    var errs = errsObj as object[];
                    if (errs != null && errs.Length > 0)
                    {
                        throw new Exception(string.Join("; ", Array.ConvertAll(errs, e => e?.ToString() ?? "")));
                    }
                }

                var properties = GetDict(root, "properties") ?? throw new Exception("Error: No 'properties' node in response.");

                if (!properties.TryGetValue("parameter", out var paramObj) || !(paramObj is Dictionary<string, object> parameter))
                {
                    throw new Exception("Error: No 'parameter' node in response.");
                }

                if (!parameter.TryGetValue("WS10M", out var wsObj) || !(wsObj is Dictionary<string, object> ws10m))
                {
                    throw new Exception("POWER response missing WS10M.");
                }

                if (ws10m.TryGetValue("ANN", out var annualObj) && TryParseDecimal(annualObj, out var ann))
                {
                    return Math.Round(ann, 2);
                }

                var monthKeysNumeric = new[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
                var monthly = TryCollect(ws10m, monthKeysNumeric);

                if (monthly.Count == 0)
                {
                    var monthKeysText = new[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
                    monthly = TryCollect(ws10m, monthKeysText);
                }

                if (monthly.Count == 0)
                {
                    foreach (var kv in ws10m)
                    {
                        if (TryParseDecimal(kv.Value, out var v))
                        {
                            monthly.Add(v);
                        }
                    }
                }

                if (monthly.Count == 0)
                {
                    string keys = string.Join(",", ws10m.Keys);
                    throw new Exception("Error: No monthly WS10M values found. Keys present: " + keys);
                }

                decimal annual = Math.Round(monthly.Average(), 2);
                return annual;
            }
            catch (WebException wex)
            {
                string body = ReadErrorBody(wex);
                throw new Exception($"Error: Error retrieving wind data: {wex.Message}" +
                                    (string.IsNullOrWhiteSpace(body) ? "" : $" | Response: {body}"));
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Error retrieving wind data: " + ex.Message);
            }
        }

        // ======== Pretty HTML Add-on (ZIP convenience) ========

        public string[] WindDetailsHtmlByZip(string zipcode)
        {
            if (string.IsNullOrWhiteSpace(zipcode))
            {
                return new[] { "Error: zipcode is required." };
            }

            zipcode = zipcode.Trim();

            if (zipcode.Length != 5 || !zipcode.All(char.IsDigit))
            {
                return new[] { "Error: zipcode must be 5 digits." };
            }

            var latlon = GetLatLonFromZip(zipcode);

            if (latlon == null)
            {
                return new[] { "Error: invalid or unsupported ZIP code." };
            }

            return WindDetailsHtml((decimal)latlon.Value.lat, (decimal)latlon.Value.lon);
        }

        public string[] WindDetailsHtml(decimal latitude, decimal longitude)
        {
            try
            {
                var data = FetchWindMonthly(latitude, longitude);
                string html = BuildWindCard(data);
                return new[] { html };
            }
            catch (Exception ex)
            {
                return new[] { "Error: " + System.Web.HttpUtility.HtmlEncode(ex.Message) };
            }
        }

        private sealed class WindMonthly
        {
            public decimal Lat { get; set; }
            public decimal Lon { get; set; }
            public decimal AnnualWs10 { get; set; }
            public decimal AnnualWs50 { get; set; }
            public List<(string mon, decimal ws10)> Rows { get; set; } = new List<(string, decimal)>();
        }

        // Get monthly values
        private WindMonthly FetchWindMonthly(decimal lat, decimal lon)
        {
            string url = string.Format(
                CultureInfo.InvariantCulture,
                PowerClimUrlDetails,
                lon, lat
            );

            string json = DownloadString(url);

            var js = new JavaScriptSerializer();
            dynamic obj = js.DeserializeObject(json);

            if (obj == null)
            {
                throw new Exception("Error: Empty response from NASA POWER.");
            }

            // error
            if (obj.ContainsKey("errors"))
            {
                var errs = obj["errors"] as object[];
                if (errs != null && errs.Length > 0)
                {
                    throw new Exception(string.Join("; ", Array.ConvertAll(errs, e => e?.ToString() ?? "")));
                }
            }

            var output = new WindMonthly { Lat = lat, Lon = lon };

            dynamic ws10 = obj["properties"]["parameter"]["WS10M"];
            dynamic ws50 = obj["properties"]["parameter"]["WS50M"];

            string[] months = new[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
            string[] labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            decimal sum10 = 0m;

            // For each month
            for (int i = 0; i < 12; i++)
            {
                decimal v10 = Convert.ToDecimal(ws10[months[i]], CultureInfo.InvariantCulture);
                sum10 += v10;
                output.Rows.Add((labels[i], v10));
            }

            output.AnnualWs10 = sum10 / 12m;
            output.AnnualWs50 = Convert.ToDecimal(ws50["ANN"], CultureInfo.InvariantCulture);

            return output;
        }

        // Classify wind speed according to Beaufort Wind Scale for a more relatable description
        private static string Beaufort(decimal mps)
        {
            double v = (double)mps;

            if (v < 0.5) {
                return "Calm (B0)";
            }
            if (v < 1.6) {
                return "Light Air (B1)";
            }
            if (v < 3.4) {
                return "Light Breeze (B2)";
            }
            if (v < 5.5) {
                return "Gentle Breeze (B3)";
            }
            if (v < 8.0) {
                return "Moderate Breeze (B4)";
            }
            if (v < 10.8) {
                return "Fresh Breeze (B5)";
            }
            if (v < 13.9) {
                return "Strong Breeze (B6)";
            }
            if (v < 17.2) {
                return "Near Gale (B7)";
            }
            if (v < 20.8) {
                return "Gale (B8)";
            }
            if (v < 24.5) {
                return "Strong Gale (B9)";
            }
            if (v < 28.5) {
                return "Storm (B10)";
            }
            if (v < 32.7) {
                return "Violent Storm (B11)";
            }
            // Hope not
            return "Hurricane (B12)";
        }

        // Wind speed classification above 50m in height
        private static string WindClassHint(decimal ws50)
        {
            if (ws50 < 5.0m) {
                return "Low resource";
            }
            if (ws50 < 6.5m) {
                return "Moderate";
            }
            if (ws50 < 8.0m) {
                return "Good";
            }
            return "Excellent";
        }

        // Build a nice display for page view - adapted from Solar service
        private string BuildWindCard(WindMonthly m)
        {
            string H(string s)
            {
                return System.Web.HttpUtility.HtmlEncode(s ?? "");
            }

            decimal maxBar = 0m;

            foreach (var row in m.Rows)
            {
                if (row.ws10 > maxBar)
                {
                    maxBar = row.ws10;
                }
            }

            if (maxBar <= 0m)
            {
                maxBar = 1m;
            }

            var sb = new StringBuilder();
            sb.Append(@"
<style>
  .card-wind { border:1px solid #ccc; border-radius:10px; padding:16px; font-family: Segoe UI, Arial, sans-serif; }
  .wind-head { display:flex; justify-content:space-between; align-items:baseline; margin-bottom:8px; }
  .wind-title{ font-size:18px; font-weight:600; }
  .wind-sub  { font-size:12px; opacity:0.8; }
  .wind-annual{ margin:8px 0 12px 0; display:flex; gap:18px; flex-wrap:wrap; }
  .pill     { background:#f3f3f3; padding:6px 10px; border-radius:999px; font-size:13px; }
  .wind-grid{ width:100%; border-collapse:collapse; table-layout:fixed; }
  .wind-grid th, .wind-grid td { padding:6px; text-align:left; }
  .barwrap  { height:10px; background:#eee; border-radius:6px; overflow:hidden; }
  .barwind  { height:10px; background:#90caf9; }
  .mono     { font-family:Consolas, Menlo, monospace; }
</style>
<div class='card-wind'>
  <div class='wind-head'>
    <div class='wind-title'>Wind Intensity</div>
    <div class='wind-sub'>Lat " + H(m.Lat.ToString("0.#####")) + ", Lon " + H(m.Lon.ToString("0.#####")) + @"</div>
  </div>
  <div class='wind-annual'>
    <div class='pill'>WS10m (annual): <span class='mono'>" + H(m.AnnualWs10.ToString("0.0")) + @"</span> m/s (" + H(Beaufort(m.AnnualWs10)) + @")</div>
    <div class='pill'>WS50m (annual): <span class='mono'>" + H(m.AnnualWs50.ToString("0.0")) + @"</span> m/s • " + H(WindClassHint(m.AnnualWs50)) + @"</div>
  </div>
  <table class='wind-grid'>
    <thead>
      <tr><th>Month</th><th>WS10m</th></tr>
    </thead>
    <tbody>
");
            // Add the monthly data for each row
            foreach (var row in m.Rows)
            {
                int w = (int)Math.Round((double)(row.ws10 / maxBar * 100m));
                sb.Append("<tr>");
                sb.Append("<td>" + H(row.mon) + "</td>");
                sb.Append("<td><div class='barwrap'><div class='barwind' style='width:" + w + "%'></div></div> <span class='mono'>" + H(row.ws10.ToString("0.0")) + "</span></td>");
                sb.Append("</tr>");
            }

            sb.Append(@"</tbody></table>
  <div class='wind-sub' style='margin-top:8px;'>Source: NASA POWER climatology</div>
</div>");

            return sb.ToString();
        }

        // helpers
        // Try to collect data from dictionary
        private static List<decimal> TryCollect(Dictionary<string, object> dict, IEnumerable<string> orderedKeys)
        {
            var vals = new List<decimal>(12);

            foreach (var k in orderedKeys)
            {
                if (dict.TryGetValue(k, out var obj) && TryParseDecimal(obj, out var v))
                {
                    vals.Add(v);
                }
            }

            return vals;
        }

        // Validate lat/lon or throw error
        private static void Validate(decimal lat, decimal lon)
        {
            if (lat < -90 || lat > 90)
            {
                throw new ArgumentException("Error: Latitude must be in [-90, 90].");
            }

            if (lon < -180 || lon > 180)
            {
                throw new ArgumentException("Error: Longitude must be in [-180, 180].");
            }
        }

        // Use dictionary to access data
        private static Dictionary<string, object> GetDict(Dictionary<string, object> parent, string key)
        {
            if (parent != null && parent.TryGetValue(key, out var obj) && obj is Dictionary<string, object> d)
            {
                return d;
            }

            return null;
        }

        // Try parsing the values
        private static bool TryParseDecimal(object obj, out decimal value)
        {
            value = 0m;

            if (obj == null)
            {
                return false;
            }

            // Use switch to find type of object
            switch (obj)
            {
                case decimal d: value = d;
                    {
                        return true;
                    }
                case double db: value = (decimal)db;
                    {
                        return true;
                    }
                case float f: value = (decimal)f;
                    {
                        return true;
                    }
                case int i: value = i;
                    {
                        return true;
                    }
                case long l: value = l;
                    {
                        return true;
                    }
                case string s:
                    {
                        return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
                    }
                default:
                    {
                        return decimal.TryParse(obj.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out value);
                    }
            }
        }

        // Retrieve the string from the provided URL
        private static string DownloadString(string url)
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.UserAgent, "CSE445-Assignment6-WindService");
                wc.Encoding = System.Text.Encoding.UTF8;

                try
                {
                    return wc.DownloadString(url);
                }
                catch (WebException wex)
                {
                    string body = ReadErrorBody(wex);

                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        throw new WebException($"{wex.Message} | {body}", wex);
                    }

                    throw;
                }
            }
        }

        // Read the error
        private static string ReadErrorBody(WebException wex)
        {
            try
            {
                using (var resp = wex.Response)
                {
                    // whyyyyyyy
                    if (resp == null)
                    {
                        return null;
                    }

                    using (var s = resp.GetResponseStream())
                    using (var r = new StreamReader(s))
                    {
                        // Empty it
                        return r.ReadToEnd();
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Convert a US ZIP code to (lat, lon) using the NWS DWML service.
        /// Pulled from Weather service
        /// </summary>
        private static (double lat, double lon)? GetLatLonFromZip(string zipcode)
        {
            string xml = DownloadString(string.Format(ZipToLatLonUrl, HttpUtility.UrlEncode(zipcode)));
            var doc = XDocument.Parse(xml);

            var latLonList = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "latLonList")?.Value?.Trim();

            // If empty
            if (string.IsNullOrWhiteSpace(latLonList))
            {
                return null;
            }

            var parts = latLonList.Split(',');

            if (parts.Length != 2)
            {
                return null;
            }

            if (double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double lat) &&
                double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double lon))
            {
                return (lat, lon);
            }

            return null;
        }
    }
}
