using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace CSE445_Assignment6
{
    public class Global : System.Web.HttpApplication
    {
        // builds the principal with roles from the auth cookie
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // get auth cookie
            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null) return;

            FormsAuthenticationTicket ticket;
            try
            {
                ticket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch
            {
                return;
            }

            if (ticket == null) return;

            // separate roles
            string[] roles = (ticket.UserData ?? "")
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // create principal
            var identity = new FormsIdentity(ticket);
            var principal = new GenericPrincipal(identity, roles);

            Context.User = principal;
            System.Threading.Thread.CurrentPrincipal = principal;
        }

        // sets the counter to zero when app starts
        protected void Application_Start(object sender, EventArgs e)
        {
            Application["usernumber"] = 0;
        }

        // when every session starts increment counter by 1
        void Session_Start(object sender, EventArgs e)
        {
            Application.Lock();
            Application["usernumber"] = (int)Application["usernumber"] + 1;
            Application.UnLock();
        }
    }
}