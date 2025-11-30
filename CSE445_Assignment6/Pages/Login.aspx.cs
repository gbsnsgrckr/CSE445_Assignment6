using CSE445_Assignment6.Controls;
using CSE445_Assignment6;
using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CSE445_Assignment6
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        // called by LoginPanel when captcha and input validation pass
        protected void LoginPanel_Login(object sender, LoginEventArgs e)
        {
            string username = (e.Username ?? "").Trim();
            string password = e.Password ?? "";

            // check staff role
            if (Account.TryValidateStaff(username, password))
            {
                IssueAuthCookie(username, e.RememberMe, isStaff: true);
                return;
            }

            // check normal member
            if (Account.TryValidateMember(username, password))
            {
                IssueAuthCookie(username, e.RememberMe, isStaff: false);
                return;
            }

            // wrong credentials
            var litMsg = LoginPanel1.FindControl("litMsg") as Literal;

            if (litMsg != null)
            {
                litMsg.Text = "<span style='color:#b00;'>Error: Invalid username or password.</span>";
            }
        }

        // to registration page
        protected void LoginPanel_Register(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Register.aspx");
        }

        private void IssueAuthCookie(string username, bool rememberMe, bool isStaff)
        {
            // try clear old remember cookies
            Response.Cookies["LastUser"].Expires = DateTime.UtcNow.AddDays(-1);
            Response.Cookies["RememberUser"].Expires = DateTime.UtcNow.AddDays(-1);

            if (rememberMe && !string.IsNullOrWhiteSpace(username))
            {
                string uname = username.Trim();

                // store username
                Response.Cookies["LastUser"].Value = uname;
                Response.Cookies["LastUser"].Expires = DateTime.UtcNow.AddDays(7);

                // remember flag
                Response.Cookies["RememberUser"].Value = "1";
                Response.Cookies["RememberUser"].Expires = DateTime.UtcNow.AddDays(7);
            }

            // mark staff flag in session
            Session["IsStaff"] = isStaff;

            // roles string
            string roles = isStaff ? "Staff" : "Member";

            // build a proper forms ticket with roles
            var ticket = new FormsAuthenticationTicket(
                1,
                username,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                rememberMe,
                roles,
                FormsAuthentication.FormsCookiePath
            );

            // encrypt into auth cookie
            string encrypted = FormsAuthentication.Encrypt(ticket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);

            if (rememberMe)
            {
                authCookie.Expires = ticket.Expiration;
            }

            Response.Cookies.Add(authCookie);

            // use ReturnUrl if present
            string redirectUrl = FormsAuthentication.GetRedirectUrl(username, rememberMe);
            Response.Redirect(redirectUrl);


        }
    }
}
