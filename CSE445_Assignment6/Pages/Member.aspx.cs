using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace CSE445_Assignment6
{
    public partial class Member : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // welcome
                string name = Context.User != null && Context.User.Identity != null && Context.User.Identity.IsAuthenticated
                    ? Context.User.Identity.Name
                    : "(unknown)";

                litUser.Text = Server.HtmlEncode(name);

                // load preferred ZIP from Users.xml
                try
                {
                    string username = Context.User?.Identity?.Name;
                    if (!string.IsNullOrWhiteSpace(username))
                    {
                        string xmlPath = Server.MapPath("~/App_Data/Users.xml");
                        var doc = System.Xml.Linq.XDocument.Load(xmlPath);

                        // find the <User> element with matching <Username>
                        var userNode = doc.Descendants("User")
                            .FirstOrDefault(u =>
                                string.Equals((string)u.Element("Username"),
                                              username,
                                              StringComparison.OrdinalIgnoreCase));

                        if (userNode != null)
                        {
                            string prefZip = (string)userNode.Element("PrefZip");
                            if (!string.IsNullOrWhiteSpace(prefZip))
                            {
                                txtZip.Text = prefZip.Trim();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error loading PrefZip: " + ex.Message);
                }

                // default to GOOGL
                TextBox2.Text = "GOOGL";

                // If we want to use last used stock from cookie, uncomment this:
                /*
                var lastStockCookie = Request.Cookies["LastStock"];
                if (lastStockCookie != null && !string.IsNullOrWhiteSpace(lastStockCookie.Value))
                {
                    TextBox2.Text = lastStockCookie.Value;
                }
                */
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
