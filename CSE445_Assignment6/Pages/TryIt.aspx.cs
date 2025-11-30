using System;
using System.Web;
using System.Linq;
using CSE445_Assignment6.WeatherService;
using CSE445_Assignment6.SolarService;
using CSE445_Assignment6.WindService;
using CSE445_Assignment6.SecurityLib;
using CSE445_Assignment6.NewsService;
using CSE445_Assignment6.StockService;

namespace CSE445_Assignment6
{
    public partial class TryIt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                lblWeatherUrl.Text = HttpUtility.HtmlEncode(baseUrl + "/Pages/WeatherService/WeatherService.svc");
                //lblSolarUrl.Text = HttpUtility.HtmlEncode(baseUrl + "/Pages/SolarService/SolarService.svc");
                //lblWindUrl.Text = HttpUtility.HtmlEncode(baseUrl + "/Pages/WindService/WindService.svc");
                lblStockUrl.Text = HttpUtility.HtmlEncode(baseUrl + "/Pages/StockService/StockService.svc");
                lblNewsUrl.Text = HttpUtility.HtmlEncode(baseUrl + "/Pages/NewsService/NewsService.svc");
            }

            // Prefill ZIPs from cookie if present
            var c = Request.Cookies["LastZip"];
            if (c != null && !string.IsNullOrWhiteSpace(c.Value))
            {
                if (string.IsNullOrWhiteSpace(txtZip.Text))
                {
                    txtZip.Text = c.Value;
                }
            }

            // Prefill SecurityLib/Hashing input from cookie if present
            var cH = Request.Cookies["LastHash"];
            if (cH != null && !string.IsNullOrWhiteSpace(cH.Value))
            {
                if (string.IsNullOrWhiteSpace(txtHashInput.Text))
                {
                    txtHashInput.Text = cH.Value;
                }
            }

            // Prefill StockService input from cookie if present
            var cS = Request.Cookies["LastStock"];
            if (cS != null && !string.IsNullOrWhiteSpace(cS.Value))
            {
                if (string.IsNullOrWhiteSpace(TextBox2.Text))
                {
                    TextBox2.Text = cS.Value;
                    TextBox1.Text = cS.Value;
                }
            }
        }

        // Weather service
        protected void btnWeather_Click(object sender, EventArgs e)
        {
            try
            {
                var svc = new WeatherService.WeatherService();
                string zip = txtZip.Text?.Trim() ?? "";

                // Check if input is empty, if so, error
                if (string.IsNullOrWhiteSpace(zip) || zip.Length != 5)
                {
                    litWeather.Text = "<div style='color:#b00;'>Error: enter a 5-digit ZIP code.</div>";
                    return;
                }

                // Set cookies and add expiration
                Response.Cookies["LastZip"].Value = zip;
                Response.Cookies["LastZip"].Expires = DateTime.UtcNow.AddDays(7);

                // Get weather data in nice display
                string[] result = svc.Weather5day(zip);

                if (result != null && result.Length > 0)
                {
                    litWeather.Text = string.Join("", result);
                }
                else
                {
                    litWeather.Text = "<div>No result.</div>";
                }
            }
            catch (Exception ex)
            {
                litWeather.Text = "<div style='color:#b00;'>Error: " + Server.HtmlEncode(ex.Message) + "</div>";
            }
        }
        /* Not used for Assignment 6
        // Solar service
        protected void btnSolar_Click(object sender, EventArgs e)
        {
            try
            {
                string zip = txtSolarZip.Text?.Trim();

                // Check if input is empty, if so, error
                if (string.IsNullOrWhiteSpace(zip) || zip.Length != 5)
                {
                    litSolar.Text = "<div style='color:#b00;'>Error: enter a 5-digit ZIP code.</div>";
                    return;
                }

                // Set cookies and add expiration
                Response.Cookies["LastZip"].Value = zip;
                Response.Cookies["LastZip"].Expires = DateTime.UtcNow.AddDays(7);

                var svc = new SolarService.SolarService();

                // Get annual intensity
                decimal annual = svc.SolarIntensityByZip(zip);

                // Get annual data in nice display
                string[] html = svc.SolarDetailsHtmlByZip(zip);

                litSolar.Text =
                    "<div class='pill' style='display:inline-block;margin-bottom:8px;background:#f3f3f3;padding:6px 10px;border-radius:999px;'>Annual (spec): <b>" +
                    Server.HtmlEncode(annual.ToString("0.0")) + " kWh/m²/day</b></div>" +
                    string.Join("", html);
            }
            catch (Exception ex)
            {
                litSolar.Text = "<div style='color:#b00;'>Error: " + Server.HtmlEncode(ex.Message) + "</div>";
            }
        }
        // Wind service
        protected void btnWind_Click(object sender, EventArgs e)
        {
            try
            {
                string zip = txtWindZip.Text?.Trim();

                // Check if input is empty, if so, error
                if (string.IsNullOrWhiteSpace(zip) || zip.Length != 5)
                {
                    litWind.Text = "<div style='color:#b00;'>Error: enter a 5-digit ZIP code.</div>";
                    return;
                }

                // Set cookies and add expiration
                Response.Cookies["LastZip"].Value = zip;
                Response.Cookies["LastZip"].Expires = DateTime.UtcNow.AddDays(7);

                var svc = new WindService.WindService();

                // Get wind intensity value
                decimal annual = svc.WindIntensityByZip(zip);

                // Get Wind data in nice display
                string[] html = svc.WindDetailsHtmlByZip(zip);

                litWind.Text =
                    "<div class='pill' style='display:inline-block;margin-bottom:8px;background:#f3f3f3;padding:6px 10px;border-radius:999px;'>Annual WS10m (spec): <b>" +
                    Server.HtmlEncode(annual.ToString("0.0")) + " m/s</b></div>" +
                    string.Join("", html);
            }
            catch (Exception ex)
            {
                litWind.Text = "<div style='color:#b00;'>Error: " + Server.HtmlEncode(ex.Message) + "</div>";
            }
        }
        */
                // Hashing TryIt
                // Hex button
        protected void btnHashHex_Click(object sender, EventArgs e)
        {
            try
            {
                var input = txtHashInput.Text?.Trim() ?? "";

                // Check if input is empty, if so, error
                if (string.IsNullOrWhiteSpace(input))
                {
                    litHash.Text = "<div style='color:#b00;'>Error: enter an input string.</div>";
                    return;
                }

                // Set cookies and add expiration
                Response.Cookies["LastHash"].Value = input;
                Response.Cookies["LastHash"].Expires = DateTime.UtcNow.AddDays(7);

                string hex = Encryption.Sha256Hex(input);

                litHash.Text = "<code>" + Server.HtmlEncode(hex) + "</code>";
            }
            catch (Exception ex)
            {
                litHash.Text = "<div style='color:#b00;'>Error: " + Server.HtmlEncode(ex.Message) + "</div>";
            }
        }


        // Base64 button
        protected void btnHashB64_Click(object sender, EventArgs e)
        {
            try
            {
                var input = txtHashInput.Text?.Trim() ?? "";

                // Check if input is empty, if so, error
                if (string.IsNullOrWhiteSpace(input))
                {
                    litHash.Text = "<div style='color:#b00;'>Error: enter an input string.</div>";
                    return;
                }

                // Set cookies and add expiration
                Response.Cookies["LastHash"].Value = input;
                Response.Cookies["LastHash"].Expires = DateTime.UtcNow.AddDays(7);

                // Encrypt
                string b64 = Encryption.Sha256Base64(input);

                litHash.Text = "<code>" + Server.HtmlEncode(b64) + "</code>";
            }
            catch (Exception ex)
            {
                litHash.Text = "<div style='color:#b00;'>Error: " + Server.HtmlEncode(ex.Message) + "</div>";
            }
        }

        // if the button is clicked it output the tracked number of users from global event handler
        protected void Button3_Click(object sender, EventArgs e)
        {
            if (Application["usernumber"] != null)
            {
                Label3.Text = "Total visits (tracked by Global.asax event handler): " + Application["usernumber"].ToString();
            }
            else
            {
                Label3.Text = "Global counter not initialized.";
            }

        }

        // saves a cookie and outputs it 
        protected void Button2_Click(object sender, EventArgs e)
        {
            String symbol = TextBox1.Text.Trim().ToUpper();
            if (symbol == "")
            {
                Label4.Text = " enter a cookie first";
            }
            HttpCookie lastStockCookie = new HttpCookie("LastStock", symbol);
            lastStockCookie.Expires = DateTime.Now.AddDays(7);
            Response.Cookies.Add(lastStockCookie);

            Label4.Text = $"Cookie saved! Last Stock Symbol: {symbol}";

        }

        // Use StockService for stock advice
        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string symbol = TextBox2.Text.Trim().ToUpper();

                if (string.IsNullOrWhiteSpace(symbol))
                {
                    Label1.Text = "";
                    Label2.Text = "ERROR: Please enter a stock symbol!";
                    return;
                }

                // Set cookies and add expiration
                Response.Cookies["LastStock"].Value = symbol;
                Response.Cookies["LastStock"].Expires = DateTime.UtcNow.AddDays(7);

                // call service
                var svc = new StockService.StockService();
                string result = svc.DownloadStockInfo(symbol);

                if (string.IsNullOrWhiteSpace(result))
                {
                    Label1.Text = "";
                    Label2.Text = "ERROR: No data returned from StockService.";
                    return;
                }

                // if the service returned an error string, show it in Label2
                if (result.StartsWith("Error:", StringComparison.OrdinalIgnoreCase))
                {
                    Label1.Text = "";
                    Label2.Text = result;
                    return;
                }

                string headerPart = result;
                string analysisPart = string.Empty;

                int idx = result.IndexOf("Analysis:", StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                {
                    headerPart = result.Substring(0, idx).TrimEnd();
                    analysisPart = result.Substring(idx).TrimStart();
                }

                Label1.Text = headerPart;
                Label2.Text = analysisPart;
            }
            catch (Exception ex)
            {
                Label1.Text = "";
                Label2.Text = "ERROR: " + Server.HtmlEncode(ex.Message);
            }
        }

        // News service
        protected void btnNews_Click(object sender, EventArgs e)
        {
            try
            {
                var svc = new NewsService.NewsService();

                // Split topics by comma
                string rawTopics = txtNewsTopics.Text ?? string.Empty;
                string[] topics = rawTopics
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .ToArray();

                if (topics.Length == 0)
                {
                    litNews.Text = "<ul class='news-list'><li>Please enter at least one topic.</li></ul>";
                    return;
                }

                // Call the service – this returns HTML fragments for each article
                string[] items = svc.NewsFocus(topics);   // each item is something like: <li>...</li>

                if (items == null || items.Length == 0)
                {
                    litNews.Text = "<ul class='news-list'><li>No articles found. Try another topic!</li></ul>";
                    return;
                }

                // Wrap in a UL so the <li> fragments are valid HTML
                litNews.Text = "<ul class='news-list'>" + string.Join("", items) + "</ul>";
            }
            catch (Exception ex)
            {
                litNews.Text = "<ul class='news-list'><li style='color:#f66;'>Error: "
                               + Server.HtmlEncode(ex.Message) + "</li></ul>";
            }
        }


    }
}
