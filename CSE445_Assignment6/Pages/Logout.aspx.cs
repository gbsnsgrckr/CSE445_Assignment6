using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace CSE445_Assignment6
{
    public partial class Logout : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // sign out
            FormsAuthentication.SignOut();

            // clear session
            Session.Clear();
            Session.Abandon();

            // clear cookies
            ClearCookie("LoginOK");
            //ClearCookie("LastUser");
            ClearCookie("LastZip");
            ClearCookie("LastHash");
            ClearCookie("LastStock");
            ClearCookie(".ASPXAUTH");

            // try make sure they are clear
            if (Request.Cookies[".ASPXAUTH"] != null)
            {
                var authCookie = new HttpCookie(".ASPXAUTH");
                authCookie.Expires = DateTime.UtcNow.AddDays(-1);
                Response.Cookies.Add(authCookie);
            }

            if (Request.Cookies["LoginOK"] != null)
            {
                var loginCookie = new HttpCookie("LoginOK");
                loginCookie.Expires = DateTime.UtcNow.AddDays(-1);
                Response.Cookies.Add(loginCookie);
            }

            /* Keep last user for Remember Me functionality
            if (Request.Cookies["LastUser"] != null)
            {
                var lastUserCookie = new HttpCookie("LastUser");
                lastUserCookie.Expires = DateTime.UtcNow.AddDays(-1);
                Response.Cookies.Add(lastUserCookie);
            }
            */

            if (Request.Cookies["LastZip"] != null)
            {
                var lastZipCookie = new HttpCookie("LastZip");
                lastZipCookie.Expires = DateTime.UtcNow.AddDays(-1);
                Response.Cookies.Add(lastZipCookie);
            }

            if (Request.Cookies["LastHash"] != null)
            {
                var lastHashCookie = new HttpCookie("LastHash");
                lastHashCookie.Expires = DateTime.UtcNow.AddDays(-1);
                Response.Cookies.Add(lastHashCookie);
            }

            Response.Redirect("~/Pages/Default.aspx");
        }

        private void ClearCookie(string name)
        {
            var c = new HttpCookie(name, "")
            {
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            Response.Cookies.Add(c);
        }
    }
}
