using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace CSE445_Assignment6
{
    public class Global : System.Web.HttpApplication
    {
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