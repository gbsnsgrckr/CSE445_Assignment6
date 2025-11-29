using CSE445_Assignment6.StockService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace CSE445_Assignment6.StockService
{
    public class StockService : IStockService
    {
        private const string StockURL = "https://finnhub.io/api/v1/quote?symbol={0}&token={1}";

        /// <summary>
        /// Downloads stock information for user-provided ticker symbol from Finnhub api
        /// </summary>
        /// <param name="symbol">Ticker symbol (e.g., AAPL, MSFT)</param>
        /// <returns>Formatted text</returns>
        public string DownloadStockInfo(string symbol)
        {
            try
            {
                // validate input
                if (string.IsNullOrWhiteSpace(symbol))
                {
                    return "Error: Missing ticker symbol.";
                }

                symbol = symbol.Trim().ToUpperInvariant();

                // read API key from Web.config
                string token = ConfigurationManager.AppSettings["FinnhubApiKey"];
                if (string.IsNullOrWhiteSpace(token))
                {
                    return "Error: Missing FinnhubApiKey.";
                }

                // build URL and download JSON
                string url = string.Format(CultureInfo.InvariantCulture, StockURL, symbol, token);
                string json = DownloadString(url);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return "Error: Empty response from Finnhub.";
                }

                // deserialize JSON into dictionary
                var serializer = new JavaScriptSerializer();
                var root = serializer.Deserialize<Dictionary<string, object>>(json);

                if (root == null)
                {
                    return "Error: Invalid or unrecognized JSON from Finnhub.";
                }

                // keys:
                // c = current price
                // d = change
                // dp = percent change
                // h = high
                // l = low
                // o = open
                // pc = previous close
                if (!TryGetDecimal(root, "c", out decimal c))
                {
                    return "Error: Finnhub response is missing current price (c).";
                }

                TryGetDecimal(root, "d", out decimal d);
                TryGetDecimal(root, "dp", out decimal dp);
                TryGetDecimal(root, "h", out decimal h);
                TryGetDecimal(root, "l", out decimal l);
                TryGetDecimal(root, "o", out decimal o);
                TryGetDecimal(root, "pc", out decimal pc);

                if (c == 0m && d == 0m && dp == 0m && h == 0m && l == 0m && o == 0m && pc == 0m)
                {
                    return $"Error: No quote data returned for symbol '{symbol}'. It may be invalid or unsupported.";
                }

                // build a summary string
                var sb = new StringBuilder();

                sb.AppendLine($"Symbol: {symbol}");
                sb.AppendLine($"<br />");
                sb.AppendLine($"Current Price: {c.ToString("0.00", CultureInfo.InvariantCulture)}");
                sb.AppendLine($"<br />");
                sb.AppendLine($"Change: {d.ToString("0.00", CultureInfo.InvariantCulture)}");
                sb.AppendLine($"<br />");
                sb.AppendLine($"Percent Change: {dp.ToString("0.00", CultureInfo.InvariantCulture)}%");
                sb.AppendLine($"<br />");
                sb.AppendLine($"High (day): {h.ToString("0.00", CultureInfo.InvariantCulture)}");
                sb.AppendLine($"<br />");
                sb.AppendLine($"Low (day): {l.ToString("0.00", CultureInfo.InvariantCulture)}");
                sb.AppendLine($"<br />");
                sb.AppendLine($"Open (day): {o.ToString("0.00", CultureInfo.InvariantCulture)}");
                sb.AppendLine($"<br />");
                sb.AppendLine($"Previous Close: {pc.ToString("0.00", CultureInfo.InvariantCulture)}");

                // append trading-rule analysis from StockInfo()
                sb.AppendLine();
                sb.AppendLine("Analysis:");
                sb.AppendLine(
                    StockInfo(
                        symbol,
                        (double)c,
                        (double)d,
                        (double)dp,
                        (double)h,
                        (double)l,
                        (double)o,
                        (double)pc
                    )
                );

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        /// <summary>
        /// Uses simple rules and returns an analysis message.
        /// </summary>
        public string StockInfo(string symbol, double c, double d, double dp, double h, double l, double o, double pc)
        {
            // input validation
            if (string.IsNullOrWhiteSpace(symbol))
                return "Error: Missing ticker symbol";
            symbol = symbol.ToUpper().Trim();

            // i will use several rules of thumb while trading and provide important information for traders to know, using the information
            // i can retrieve from finnhub (all the parameters) . the full names for each one are in the aspx
            string message = "";
            double volatility = (h - l) / c;
            double dayChange = c - o;        // used to show how much it changed in a day

            // sell - half rule is a common rule of thumb for traders that if the price of the stock doubles,
            // you should sell half your stock in it. checks this and notifies if it is relevant
            if (c >= pc * 2)
            {
                message += "<br />Sell-half rule: Stock doubled in price, so you should sell half your stock in it";
            }

            // prints out level of volatility 
            if (volatility < 0.02)
            {
                message += "<br />Low volatility";
            }
            else if (volatility < 0.05)
            {
                message += "<br />Medium volatility";
            }
            else
            {
                message += "<br />High volatility";
            }

            // will print information on trading compared to today's open
            if (dayChange > 0)
            {
                message += "<br />Trading higher than today's open";
            }
            else if (dayChange < 0)
            {
                message += "<br />Trading lower than today's open";
            }
            else
            {
                message += "<br />Trading equal to today's open";
            }

            // will print information on up and down since previous close
            if (d > 0)
            {
                message += "<br />Up since previous close";
            }
            else if (d < 0)
            {
                message += "<br />Down since previous close";
            }
            else
            {
                message += "<br />No change compared to previous close";
            }

            return message;
        }

        // download string from api
        private static string DownloadString(string url)
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.UserAgent, "CSE445-Assignment6-StockService");
                wc.Encoding = Encoding.UTF8;
                return wc.DownloadString(url);
            }
        }

        // tries to get decimal value from dictionary by key
        private static bool TryGetDecimal(Dictionary<string, object> dict, string key, out decimal value)
        {
            value = 0m;

            if (dict == null || !dict.TryGetValue(key, out var obj))
                return false;

            return TryParseDecimal(obj, out value);
        }

        // tries to parse object into a decimal value
        private static bool TryParseDecimal(object obj, out decimal value)
        {
            value = 0m;

            if (obj == null)
                return false;

            // parse based on type
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
    }
}
