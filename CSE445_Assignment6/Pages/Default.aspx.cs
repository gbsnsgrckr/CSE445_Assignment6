using System;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CSE445_Assignment6
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Providers
                litProvider1.Text = "Kyle Pierce";
                litProvider2.Text = "Diya Jim";
                litProvider3.Text = "Elani Zarraga";

                // local URLs
                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string appPath = Request.ApplicationPath;

                if (!appPath.EndsWith("/"))
                {
                    appPath += "/";
                }

                // deployment url
                litDeployUrl.Text = HttpUtility.HtmlEncode(baseUrl + appPath + "Default.aspx");

                // Weather/Wind/Solar service endpoints
                litWxUrl.Text = HttpUtility.HtmlEncode(baseUrl + appPath + "WeatherService/WeatherService.svc");
                //litWindUrl.Text = HttpUtility.HtmlEncode(baseUrl + appPath + "WindService/WindService.svc");
                //litSolarUrl.Text = HttpUtility.HtmlEncode(baseUrl + appPath + "SolarService/SolarService.svc");

                // Stock service endpoint
                litStockUrl.Text = HttpUtility.HtmlEncode(baseUrl + appPath + "StockService/StockService.svc");

                // News service endpoint (Elani)
                litNewsUrl.Text = HttpUtility.HtmlEncode(baseUrl + appPath + "NewsService/NewsService.svc");

                // cookies
                var cZip = Request.Cookies["LastZip"];
                var cUser = Request.Cookies["LastUser"];
                var cHash = Request.Cookies["LastHash"];

                litLastZip.Text = HttpUtility.HtmlEncode(cZip?.Value ?? "(none)");
                litHashInput.Text = HttpUtility.HtmlEncode(cHash?.Value ?? "(none)");

                litLastUser.Text = HttpUtility.HtmlEncode(cUser?.Value ?? "(none)");
            }
        }

        // Login event handler
        protected void Login(object sender, Controls.LoginEventArgs e)
        {
            // try clear old cookies first
            Response.Cookies["LastUser"].Expires = DateTime.UtcNow.AddDays(-1);
            Response.Cookies["RememberUser"].Expires = DateTime.UtcNow.AddDays(-1);

            if (e.RememberMe && !string.IsNullOrWhiteSpace(e.Username))
            {
                string uname = e.Username.Trim();

                // remembered username
                Response.Cookies["LastUser"].Value = uname;
                Response.Cookies["LastUser"].Expires = DateTime.UtcNow.AddDays(7);

                // remember flag
                Response.Cookies["RememberUser"].Value = "1";
                Response.Cookies["RememberUser"].Expires = DateTime.UtcNow.AddDays(7);
            }

            string username = (e.Username ?? string.Empty).Trim();
            string password = e.Password ?? string.Empty;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                var litMsg = LoginPanel.FindControl("litMsg") as Literal;

                if (litMsg != null)
                {
                    litMsg.Text = "<span style='color:#b00;'>Error: A valid Username and Password are required to login.</span>";
                }

                return;
            }

            bool isStaff = false;

            // see if they are staff
            if (Account.TryValidateStaff(username, password))
            {
                isStaff = true;
            }
            // if not staff, see if they are member
            else if (!Account.TryValidateMember(username, password))
            {
                // invalid
                var litMsg = LoginPanel.FindControl("litMsg") as Literal;

                if (litMsg != null)
                {
                    litMsg.Text = "<span style='color:#b00;'>Error: Invalid username or password.</span>";
                }

                return;
            }

            // auth cookie
            IssueAuthCookie(username, e.RememberMe, isStaff);

            // redirect based on role
            if (isStaff)
            {
                Response.Redirect("~/Pages/Staff.aspx");
            }
            else
            {
                Response.Redirect("~/Pages/Member.aspx");
            }
        }

        // Register button event handler
        protected void Register(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Register.aspx");
        }

        // issue forms auth cookie including role info
        private void IssueAuthCookie(string username, bool rememberMe, bool isStaff)
        {
            // keep to not break other stuff
            Session["IsStaff"] = isStaff;

            string roles = isStaff ? "Staff" : "Member";

            // build auth ticket with roles
            var ticket = new FormsAuthenticationTicket(
                1,
                username,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                rememberMe,
                roles,
                FormsAuthentication.FormsCookiePath
            );

            string encrypted = FormsAuthentication.Encrypt(ticket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);

            // try
            if (rememberMe)
            {
                authCookie.Expires = ticket.Expiration;
            }

            Response.Cookies.Add(authCookie);
        }
    }
}
