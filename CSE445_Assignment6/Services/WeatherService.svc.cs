using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace CSE445_Assignment6.WeatherService
{
    /// <summary>
    /// WCF service that returns a 5-day 5-column forecast for user-selected US zipcode.
    /// **Modified from Assignment 4 submission
    /// </summary>
    public class WeatherService : IWeatherService
    {
        // Endpoints (DWML XML).
        private const string ZipToLatLonUrl =
            "https://graphical.weather.gov/xml/SOAP_server/ndfdXMLclient.php?listZipCodeList={0}";
        private const string NdfdTimeSeriesUrl =
            "https://graphical.weather.gov/xml/SOAP_server/ndfdXMLclient.php?lat={0}&lon={1}&product=time-series" +
            "&maxt=maxt&mint=mint&wx=wx&icons=icons&begin={2:yyyy-MM-dd}T00:00:00&end={3:yyyy-MM-dd}T23:59:59&Unit=e";

        /// <summary>
        /// Returns a 5-day weather forecast for the user-selected zip code within the U.S.
        /// </summary>
        /// <param name="zipcode">U.S. zip code</param>
        public string[] Weather5day(string zipcode)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(zipcode))
            {
                return new[] { "Error: zipcode is required." };
            }

            zipcode = zipcode.Trim();

            if (zipcode.Length != 5 || !zipcode.All(char.IsDigit))
            {
                return new[] { "Error: zipcode must be 5 digits." };
            }

            try
            {
                // Convert the zip code to lat/lon
                var latlon = GetLatLonFromZip(zipcode);

                if (latlon == null)
                {
                    return new[] { $"Error: The ZIP code '{zipcode}' is invalid or not supported. Please enter a valid 5-digit U.S. ZIP code." };
                }

                // Set start and end dates for 5-day forecast
                DateTime start = DateTime.Today;
                DateTime end = start.AddDays(4);

                // Retrieve DWML time-series data
                string dwml = DownloadString(string.Format(
                    CultureInfo.InvariantCulture, NdfdTimeSeriesUrl,
                    latlon.Value.lat.ToString(CultureInfo.InvariantCulture),
                    latlon.Value.lon.ToString(CultureInfo.InvariantCulture),
                    start, end));

                // Parse DWML
                var days = DwmlParser.ParseFiveDay(dwml, start);

                // Return a single HTML block that renders 5 columns (one per day) with EMOJIS
                return new[] { BuildFiveDayColumns(days) };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in WeatherService.Weather5day: " + ex.ToString());
                return new[] { "Error: " + ex.Message };
            }
        }

        /// <summary>
        /// Build a 5-day 5-column forecast. TV-style
        /// </summary>
        private static string BuildFiveDayColumns(IList<DwmlParser.DayForecast> days)
        {
            string H(string s)
            {
                return System.Web.HttpUtility.HtmlEncode(s ?? "");
            }

            var sb = new StringBuilder();

            // No inline <style> block anymore; styles are in Dark.css
            sb.Append(@"
<div class='wx-wrap'>
  <table class='wx-table'>
    <thead>
      <tr>
");

            // Rows as displayed (Change order here to change display order)
            // Date
            foreach (var d in days)
            {
                sb.Append($"<th>{H(d.Date.ToString("ddd"))}<br/>{H(d.Date.ToString("MM/dd"))}</th>");
            }

            sb.Append("</tr></thead><tbody>");

            // Hi
            sb.Append("<tr>");
            foreach (var d in days)
            {
                sb.Append($"<td><span class='wx-label'>Hi</span><span class='wx-high'>{d.HighF}°F</span></td>");
            }
            sb.Append("</tr>");

            // Lo
            sb.Append("<tr>");
            foreach (var d in days)
            {
                sb.Append($"<td><span class='wx-label'>Lo</span><span class='wx-low'>{d.LowF}°F</span></td>");
            }
            sb.Append("</tr>");

            // Emoji
            sb.Append("<tr>");
            foreach (var d in days)
            {
                sb.Append($"<td class='wx-emoji'>{H(d.Emoji)}</td>");
            }
            sb.Append("</tr>");

            // Summary
            sb.Append("<tr>");
            foreach (var d in days)
            {
                sb.Append($"<td>{H(d.Summary)}</td>");
            }
            sb.Append("</tr>");

            sb.Append("</tbody></table></div>");

            return sb.ToString();
        }

        /// <summary>
        /// Query the NWS to get lat/lon from a zip code
        /// </summary>
        /// <param name="zipcode">US ZIP code</param>
        /// <returns>(lat, lon) or null if the ZIP code is invalid</returns>
        private (double lat, double lon)? GetLatLonFromZip(string zipcode)
        {
            // Build URL request
            string xml = DownloadString(string.Format(ZipToLatLonUrl, HttpUtility.UrlEncode(zipcode)));
            var doc = XDocument.Parse(xml);

            // Retrieve latLonList element
            var latLonList = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "latLonList")?.Value?.Trim();

            if (string.IsNullOrWhiteSpace(latLonList))
            {
                return null;
            }

            // Split into lat and lon
            var parts = latLonList.Split(',');

            if (parts.Length != 2)
            {
                return null;
            }

            // Try using invariant culture parsing for decimal point
            double lat;
            double lon;

            bool okLat = double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out lat);
            bool okLon = double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out lon);

            if (okLat && okLon)
            {
                return (lat, lon);
            }

            return null;
        }

        /// <summary>
        /// Sets up a WebClient to download string from the specified URL.
        /// </summary>
        private static string DownloadString(string url)
        {
            using (var wc = new WebClient())
            {
                // Identify with user-agent header
                wc.Headers.Add(HttpRequestHeader.UserAgent, "CSE445-Assignment6-WeatherService");
                wc.Encoding = System.Text.Encoding.UTF8;
                return wc.DownloadString(url);
            }
        }
    }

    /// <summary>
    /// Use class to parse DWML XML for weather data.
    /// </summary>
    internal static class DwmlParser
    {
        internal sealed class DayForecast
        {
            public DateTime Date { get; set; }
            public int HighF { get; set; } = 0;
            public int LowF { get; set; } = 0;
            public string Summary { get; set; } = "N/A";
            public string Emoji { get; set; } = "";
            public string IconUrl { get; set; } = "";
        }

        public static List<DayForecast> ParseFiveDay(string dwmlXml, DateTime anchorDay)
        {
            // Parse document
            var doc = XDocument.Parse(dwmlXml);

            var parameters = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "parameters");
            var resultDates = Enumerable.Range(0, 5).Select(i => anchorDay.AddDays(i).Date).ToList();

            // Daily temps
            var maxT = parameters?.Elements().FirstOrDefault(e => e.Name.LocalName == "temperature" && (string)e.Attribute("type") == "maximum");
            var minT = parameters?.Elements().FirstOrDefault(e => e.Name.LocalName == "temperature" && (string)e.Attribute("type") == "minimum");

            var maxDates = DatesFromLayout(doc, maxT?.Attribute("time-layout")?.Value).Select(d => d.Date).ToList();
            var minDates = DatesFromLayout(doc, minT?.Attribute("time-layout")?.Value).Select(d => d.Date).ToList();
            var highs = IntValues(maxT).ToList();
            var lows = IntValues(minT).ToList();

            // Map temps by day
            var hiByDay = MapValuesByDate(maxDates, highs);
            var loByDay = MapValuesByDate(minDates, lows);

            // Weather conditions and icons
            var wx = parameters?.Elements().FirstOrDefault(e => e.Name.LocalName == "weather");
            var wxLayout = wx?.Attribute("time-layout")?.Value;

            var wxPeriods = WeatherPeriods(doc, wx, wxLayout); // (DateTime start, string summary)

            // Icons for daily weather
            var iconNode = parameters?.Elements().FirstOrDefault(e => e.Name.LocalName == "conditions-icon");
            var iconLayout = iconNode?.Attribute("time-layout")?.Value;
            var iconPeriods = IconPeriods(doc, iconNode, iconLayout); // (DateTime start, string iconUrl)

            // Group weather/icon periods by day
            var summaryByDay = wxPeriods
                .GroupBy(p => p.start.Date)
                .ToDictionary(g => g.Key, g => PickDailySummary(g.Select(x => x.summary)));

            var iconByDay = iconPeriods
                .GroupBy(p => p.start.Date)
                .ToDictionary(g => g.Key, g => PickDailyIcon(g.Select(x => x.iconUrl)));

            // Build 5-day list
            var outList = new List<DayForecast>();
            // Iterate through each to make sure each is aligned to a day
            foreach (var day in resultDates)
            {
                int hi;

                if (!hiByDay.TryGetValue(day, out hi))
                {
                    hi = 0;
                }

                int lo;

                if (!loByDay.TryGetValue(day, out lo))
                {
                    lo = 0;
                }

                string iconUrl;

                if (!iconByDay.TryGetValue(day, out iconUrl))
                {
                    iconUrl = null;
                }

                string summary;
                string pickedSummary;

                if (summaryByDay.TryGetValue(day, out pickedSummary) && !string.IsNullOrWhiteSpace(pickedSummary))
                {
                    summary = pickedSummary;
                }
                else
                {
                    summary = IconToLabel(iconUrl);

                    if (string.IsNullOrWhiteSpace(summary))
                    {
                        summary = "N/A";
                    }
                }

                string emoji = SummaryToEmoji(summary, iconUrl);

                var df = new DayForecast
                {
                    Date = day,
                    HighF = hi,
                    LowF = lo,
                    Summary = summary,
                    Emoji = emoji,
                    IconUrl = iconUrl ?? ""
                };

                outList.Add(df);
            }

            // try to make sure only 5 entries
            return outList.Take(5).ToList();
        }

        // Get dates using the layoutKey
        private static IEnumerable<DateTime> DatesFromLayout(XDocument doc, string layoutKey)
        {
            if (string.IsNullOrWhiteSpace(layoutKey))
            {
                return Enumerable.Empty<DateTime>();
            }

            var tl = doc.Descendants().FirstOrDefault(e =>
                e.Name.LocalName == "time-layout" &&
                e.Elements().Any(k => k.Name.LocalName == "layout-key" && k.Value.Trim() == layoutKey));

            if (tl == null)
            {
                return Enumerable.Empty<DateTime>();
            }

            return tl.Elements().Where(e => e.Name.LocalName == "start-valid-time")
                     .Select(x => ParseDate(x.Value));
        }

        // Parse the dates
        private static DateTime ParseDate(string s)
        {
            DateTime dt;
            bool ok = DateTime.TryParse(s, null, DateTimeStyles.AssumeLocal, out dt);

            if (ok)
            {
                return dt;
            }
            else
            {
                return DateTime.Today;
            }
        }

        // Use dictionary to get values by dates
        private static Dictionary<DateTime, int> MapValuesByDate(List<DateTime> dates, List<int> vals)
        {
            var dict = new Dictionary<DateTime, int>();

            for (int i = 0; i < dates.Count && i < vals.Count; i++)
            {
                var d = dates[i].Date;

                if (!dict.ContainsKey(d))
                {
                    dict[d] = vals[i];
                }
            }

            return dict;
        }

        // Built from NWS examples
        private static IEnumerable<int> IntValues(XElement tempNode)
        {
            if (tempNode == null)
            {
                yield break;
            }

            foreach (var v in tempNode.Elements().Where(e => e.Name.LocalName == "value"))
            {
                int n;

                if (int.TryParse(v.Value, out n))
                {
                    yield return n;
                }
            }
        }

        // Weather summaries matched with their period start times - built from NWS examples
        private static IEnumerable<(DateTime start, string summary)> WeatherPeriods(XDocument doc, XElement wxNode, string layoutKey)
        {
            if (wxNode == null)
            {
                yield break;
            }

            var dates = DatesFromLayout(doc, layoutKey).ToList();
            var wxConds = wxNode.Elements().Where(e => e.Name.LocalName == "weather-conditions").ToList();

            for (int i = 0; i < wxConds.Count && i < dates.Count; i++)
            {
                string raw = (string)wxConds[i].Attribute("weather-summary");
                string sum;

                if (!string.IsNullOrWhiteSpace(raw))
                {
                    sum = raw.Trim();
                }
                else
                {
                    sum = null;
                }

                yield return (dates[i], sum);
            }
        }

        // Match icon link with period start times - built from NWS examples
        private static IEnumerable<(DateTime start, string iconUrl)> IconPeriods(XDocument doc, XElement iconNode, string layoutKey)
        {
            if (iconNode == null)
            {
                yield break;
            }

            var dates = DatesFromLayout(doc, layoutKey).ToList();
            var links = iconNode.Elements().Where(e => e.Name.LocalName == "icon-link").Select(x => x.Value?.Trim()).ToList();

            for (int i = 0; i < links.Count && i < dates.Count; i++)
            {
                yield return (dates[i], links[i]);
            }
        }

        // Choose the daily summary
        private static string PickDailySummary(IEnumerable<string> summaries)
        {
            var list = summaries.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            if (!list.Any())
            {
                return null;
            }
            // Sort
            return list.GroupBy(s => s)
                       .OrderByDescending(g => g.Count())
                       .ThenBy(g => g.Key)
                       .First().Key;
        }

        // Choose the daily icon
        private static string PickDailyIcon(IEnumerable<string> iconUrls)
        {
            var list = iconUrls.Where(u => !string.IsNullOrWhiteSpace(u)).ToList();

            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }

        // Build summary from icon
        private static string IconToLabel(string iconUrl)
        {
            if (string.IsNullOrWhiteSpace(iconUrl))
            {
                return null;
            }

            var u = iconUrl.ToLowerInvariant();

            if (u.Contains("tsra"))
            {
                return "T-storms";
            }

            if (u.Contains("ra") || u.Contains("shra"))
            {
                return "Rain";
            }

            if (u.Contains("sn") || u.Contains("sleet"))
            {
                return "Snow";
            }

            if (u.Contains("fg"))
            {
                return "Fog";
            }

            if (u.Contains("ovc"))
            {
                return "Overcast";
            }

            if (u.Contains("bkn") || u.Contains("sct") || u.Contains("few"))
            {
                return "Partly cloudy";
            }

            if (u.Contains("skc") || u.Contains("clear"))
            {
                return "Clear";
            }

            return "N/A";
        }

        // Assign emoji to each summary for visual
        private static string SummaryToEmoji(string summary, string iconUrl)
        {
            string s = (summary ?? "").ToLowerInvariant();
            string u = (iconUrl ?? "").ToLowerInvariant();

            if (s.Contains("thunder") || u.Contains("tsra"))
            {
                return "⛈️";
            }

            if (s.Contains("snow") || u.Contains("sn"))
            {
                return "❄️";
            }

            if (s.Contains("rain") || s.Contains("showers") || u.Contains("ra") || u.Contains("shra"))
            {
                return "🌧️";
            }

            if (s.Contains("fog") || u.Contains("fg"))
            {
                return "🌫️";
            }

            if (s.Contains("overcast") || u.Contains("ovc"))
            {
                return "☁️";
            }

            if (s.Contains("cloud") || u.Contains("bkn") || u.Contains("sct") || u.Contains("few"))
            {
                return "⛅";
            }

            if (s.Contains("clear") || u.Contains("skc") || u.Contains("clear"))
            {
                return "☀️";
            }

            return "🌡️";
        }
    }
}
