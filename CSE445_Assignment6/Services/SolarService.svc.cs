using System;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Xml.Linq;

namespace CSE445_Assignment6.SolarService
{
    /// <summary>
    /// Returns an annual-average solar intensity for a user-selected
    /// US ZIP code.
    /// **Modified from Assignment 4 submission
    /// </summary>
    public class SolarService : ISolarService
    {
        // Use NWS SOAP client endpoints to convert ZIP to lat/lon
        private const string ZipToLatLonUrl =
            "https://graphical.weather.gov/xml/SOAP_server/ndfdXMLclient.php?listZipCodeList={0}";

        // API URL - annual
        private const string ApiUrl =
            "https://developer.nrel.gov/api/solar/solar_resource/v1.json?lat={0}&lon={1}&api_key={2}";

        // API URL - monthly
        private const string ApiUrlMonthly =
            "https://developer.nrel.gov/api/solar/solar_resource/v1.json?lat={0}&lon={1}&api_key={2}&format=json&attributes=ghi,dni&monthly=true";

        /// <summary>
        /// Returns annual average solar intensity for a US ZIP code.
        /// </summary>
        public decimal SolarIntensityByZip(string zipcode)
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

            // Use helper to get lat/lon from US ZIP code
            var latlon = GetLatLonFromZip(zipcode);

            if (latlon == null)
            {
                throw new ArgumentException("Error: Invalid or unsupported ZIP code.");
            }

            return SolarIntensity((decimal)latlon.Value.lat, (decimal)latlon.Value.lon);
        }

        /// <summary>
        /// Returns the annual average solar intensity for the user-selected lat/lon (in the US).
        /// </summary>
        public decimal SolarIntensity(decimal latitude, decimal longitude)
        {
            try
            {
                string apiKey = ConfigurationManager.AppSettings["NrelApiKey"];

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    throw new Exception("Error: No NREL API key found. Check Web.config file.");
                }

                string url = string.Format(CultureInfo.InvariantCulture, ApiUrl, latitude, longitude, apiKey);
                string json = DownloadString(url);

                var serializer = new JavaScriptSerializer();
                var root = serializer.Deserialize<Dictionary<string, object>>(json);

                if (root == null)
                {
                    throw new Exception("Error: Empty or invalid JSON returned from NREL.");
                }

                // Error
                if (root.TryGetValue("errors", out var errorsObj))
                {
                    var errs = errorsObj as object[];
                    if (errs != null && errs.Length > 0)
                    {
                        throw new Exception(string.Join("; ", Array.ConvertAll(errs, e => e?.ToString() ?? string.Empty)));
                    }
                }

                var outputs = GetDict(root, "outputs");

                if (outputs == null)
                {
                    throw new Exception("Error: No 'outputs' object found in API response.");
                }

                decimal dni;

                if (TryReadAnnual(outputs, "avg_dni", out dni))
                {
                    return dni;
                }

                decimal ghi;

                if (TryReadAnnual(outputs, "avg_ghi", out ghi))
                {
                    return ghi;
                }

                throw new Exception("Error: Solar intensity not found. Lat/Lon may be invalid or data may not be available for specified location.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Error retrieving solar data from NREL.", ex);
            }
        }

        // Get data by html using zip

        public string[] SolarDetailsHtmlByZip(string zipcode)
        {
            // Error if empty
            if (string.IsNullOrWhiteSpace(zipcode))
            {
                return new[] { "Error: zipcode is required." };
            }

            zipcode = zipcode.Trim();

            // Error if too short
            if (zipcode.Length != 5 || !zipcode.All(char.IsDigit))
            {
                return new[] { "Error: zipcode must be 5 digits." };
            }

            var latlon = GetLatLonFromZip(zipcode);

            if (latlon == null)
            {
                return new[] { "Error: invalid or unsupported ZIP code." };
            }

            return SolarDetailsHtml((decimal)latlon.Value.lat, (decimal)latlon.Value.lon);
        }

        // Get the Solar data using lat/lon
        public string[] SolarDetailsHtml(decimal latitude, decimal longitude)
        {
            try
            {
                var data = FetchSolarMonthly(latitude, longitude);
                string html = BuildSolarCard(data);
                return new[] { html };
            }
            catch (Exception ex)
            {
                return new[] { "Error: " + System.Web.HttpUtility.HtmlEncode(ex.Message) };
            }
        }

        private sealed class SolarMonthly
        {
            public decimal Lat { get; set; }
            public decimal Lon { get; set; }
            public decimal AnnualGhi { get; set; }
            public decimal AnnualDni { get; set; }
            public List<(string mon, decimal ghi, decimal dni)> Rows { get; set; } = new List<(string, decimal, decimal)>();
        }

        // Get monthly data
        private SolarMonthly FetchSolarMonthly(decimal lat, decimal lon)
        {
            // try to make sure key is accessible
            string apiKey = ConfigurationManager.AppSettings["NrelApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Error: Missing NREL API key.");

            string url = string.Format(
                CultureInfo.InvariantCulture,
                ApiUrlMonthly, lat, lon, apiKey
            );

            string json = DownloadString(url);

            var js = new JavaScriptSerializer();
            var root = js.Deserialize<Dictionary<string, object>>(json);
            if (root == null) throw new Exception("Error: Empty response from NREL.");

            // errors
            if (root.TryGetValue("errors", out var errObj) && errObj is object[] errs && errs.Length > 0)
                throw new Exception(string.Join("; ", Array.ConvertAll(errs, e => e?.ToString() ?? "")));

            var outp = new SolarMonthly { Lat = lat, Lon = lon };

            // outputs - avg_ghi/avg_dni
            var outputs = GetDict(root, "outputs");
            if (outputs == null) throw new Exception("Error: Missing outputs in NREL response.");

            var avgGhi = GetDict(outputs, "avg_ghi");
            var avgDni = GetDict(outputs, "avg_dni");

            if (avgGhi != null && avgGhi.TryGetValue("annual", out var aG) && TryParseDecimal(aG, out var gAnn))
                outp.AnnualGhi = gAnn;
            if (avgDni != null && avgDni.TryGetValue("annual", out var aD) && TryParseDecimal(aD, out var dAnn))
                outp.AnnualDni = dAnn;

            var mGhi = avgGhi != null ? GetDict(avgGhi, "monthly") : null;
            var mDni = avgDni != null ? GetDict(avgDni, "monthly") : null;

            string[] keys = { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };
            string[] labels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            for (int i = 0; i < 12; i++)
            {
                string mk = keys[i];

                decimal ghi = 0m, dni = 0m;

                if (mGhi != null && mGhi.TryGetValue(mk, out var gVal)) TryParseDecimal(gVal, out ghi);
                if (mDni != null && mDni.TryGetValue(mk, out var dVal)) TryParseDecimal(dVal, out dni);

                outp.Rows.Add((labels[i], ghi, dni));
            }

            return outp;
        }

        // Build the card to display the data
        private string BuildSolarCard(SolarMonthly m)
        {
            string H(string s)
            {
                return System.Web.HttpUtility.HtmlEncode(s ?? "");
            }

            decimal maxBar = 0m;

            foreach (var row in m.Rows)
            {
                if (row.ghi > maxBar)
                {
                    maxBar = row.ghi;
                }

                if (row.dni > maxBar)
                {
                    maxBar = row.dni;
                }
            }

            if (maxBar <= 0m)
            {
                maxBar = 1m;
            }

            var sb = new StringBuilder();
            sb.Append(@"
<style>
  .card-solar { border:1px solid #ccc; border-radius:10px; padding:16px; font-family: Segoe UI, Arial, sans-serif; }
  .solar-head { display:flex; justify-content:space-between; align-items:baseline; margin-bottom:8px; }
  .solar-title { font-size:18px; font-weight:600; }
  .solar-sub   { font-size:12px; opacity:0.8; }
  .solar-annual{ margin:8px 0 12px 0; display:flex; gap:18px; flex-wrap:wrap; }
  .pill { background:#f3f3f3; padding:6px 10px; border-radius:999px; font-size:13px; }
  .solar-grid  { width:100%; border-collapse:collapse; table-layout:fixed; }
  .solar-grid th, .solar-grid td { padding:6px; text-align:left; }
  .barwrap { height:10px; background:#eee; border-radius:6px; overflow:hidden; }
  .barghi  { height:10px; background:#ffd54f; }
  .bardni  { height:10px; background:#ff8a65; }
  .mono    { font-family:Consolas, Menlo, monospace; }
</style>
<div class='card-solar'>
  <div class='solar-head'>
    <div class='solar-title'>Solar Intensity</div>
    <div class='solar-sub'>Lat " + H(m.Lat.ToString("0.#####")) + ", Lon " + H(m.Lon.ToString("0.#####")) + @"</div>
  </div>
  <div class='solar-annual'>
    <div class='pill'>Annual GHI: <span class='mono'>" + H(m.AnnualGhi.ToString("0.0")) + @"</span> kWh/m²/day</div>
    <div class='pill'>Annual DNI: <span class='mono'>" + H(m.AnnualDni.ToString("0.0")) + @"</span> kWh/m²/day</div>
    <div class='pill'>Sun Hours (~GHI): <span class='mono'>" + H(m.AnnualGhi.ToString("0.0")) + @"</span> h/day</div>
  </div>
  <table class='solar-grid'>
    <thead>
            <!-- Use below line to adjust the column width******** -->
      <tr><th>Month</th><th style='width:40%'>GHI (Global Horizontal Irradiance)</th><th style='width:40%'>DNI (Direct Normal Irradiance)</th></tr>
    </thead>
    <tbody>
");
            // Progress bar for display
            foreach (var row in m.Rows)
            {
                int wGHI = (int)Math.Round((double)(row.ghi / maxBar * 100m));
                int wDNI = (int)Math.Round((double)(row.dni / maxBar * 100m));

                sb.Append("<tr>");
                sb.Append("<td>" + H(row.mon) + "</td>");
                sb.Append("<td><div class='barwrap'><div class='barghi' style='width:" + wGHI + "%'></div></div> <span class='mono'>" + H(row.ghi.ToString("0.0")) + "</span></td>");
                sb.Append("<td><div class='barwrap'><div class='bardni' style='width:" + wDNI + "%'></div></div> <span class='mono'>" + H(row.dni.ToString("0.0")) + "</span></td>");
                sb.Append("</tr>");
            }

            sb.Append(@"</tbody></table>
  <div class='solar-sub' style='margin-top:8px;'>Source: NREL Solar Resource API</div>
</div>");

            return sb.ToString();
        }

        // helpers

        private static string DownloadString(string url)
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.UserAgent, "CSE445-Assignment6-SolarService");
                wc.Encoding = System.Text.Encoding.UTF8;
                return wc.DownloadString(url);
            }
        }

        // Use dictionary to access data
        private static Dictionary<string, object> GetDict(Dictionary<string, object> parent, string key)
        {
            // Check if empty
            if (parent != null && parent.TryGetValue(key, out var obj) && obj is Dictionary<string, object> d)
            {
                return d;
            }

            return null;
        }

        // Get annual data
        private static bool TryReadAnnual(Dictionary<string, object> parent, string key, out decimal value)
        {
            value = 0m;
            // Check if empty
            if (parent == null || !parent.TryGetValue(key, out var node) || node == null)
            {
                return false;
            }

            if (node is Dictionary<string, object> dict)
            {
                if (dict.TryGetValue("annual", out var annualObj) && TryParseDecimal(annualObj, out value))
                {
                    return true;
                }
            }

            return TryParseDecimal(node, out value);
        }

        // Parse values
        private static bool TryParseDecimal(object obj, out decimal value)
        {
            value = 0m;

            if (obj == null)
            {
                return false;
            }
            // try switch to pull any type of value
            switch (obj)
            {
                case decimal d:
                    value = d;
                    return true;
                case double db:
                    value = (decimal)db;
                    return true;
                case float f:
                    value = (decimal)f;
                    return true;
                case int i:
                    value = i;
                    return true;
                case long l:
                    value = l;
                    return true;
                case string s:
                    return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
                default:
                    return decimal.TryParse(obj.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out value);
            }
        }

        /// <summary>
        /// Convert a US ZIP to lat/lon
        /// </summary>
        private static (double lat, double lon)? GetLatLonFromZip(string zipcode)
        {
            string xml = DownloadString(string.Format(ZipToLatLonUrl, HttpUtility.UrlEncode(zipcode)));
            var doc = XDocument.Parse(xml);

            var latLonList = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "latLonList")?.Value?.Trim();

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
