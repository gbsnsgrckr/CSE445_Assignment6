using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CSE445_Assignment6.Controls
{
    /// <summary>
    /// login user control
    /// </summary>
    public partial class LoginPanel : UserControl
    {
        public event EventHandler<LoginEventArgs> Login;
        public event EventHandler Register;

        // cookie
        private const string LoginCookie = "LoginOK";
        private const int LoginDays = 7;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // If cookie exists, hide captcha panel.
                var c = Request.Cookies[LoginCookie];

                // If you want CAPTCHA every time, just set captchaPanel.Visible = true.
                captchaPanel.Visible = true;
                // captchaPanel.Visible = (c == null);
            }
        }

        // login button handler
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string user = (txtUser.Text ?? string.Empty).Trim();
            string pass = txtPass.Text ?? string.Empty;

            // input validation
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                litMsg.Text = "<span style='color:#b00;'>Error: A valid Username and Password are required to login.</span>";
                return;
            }

            // validate captcha if visible
            if (captchaPanel.Visible && !ValidateCaptcha())
            {
                litMsg.Text = "<span style='color:#b00;'>Error: CAPTCHA incorrect.</span>";
                return;
            }

            // clear old message
            litMsg.Text = string.Empty;

            // set cookie
            var cookie = new HttpCookie(LoginCookie, "1")
            {
                Expires = DateTime.UtcNow.AddDays(LoginDays)
            };
            Response.Cookies.Add(cookie);

            // login event
            Login?.Invoke(this, new LoginEventArgs(
                txtUser.Text.Trim(),
                txtPass.Text,
                chkRememberMe.Checked
            ));
        }

        // validate captcha input
        private bool ValidateCaptcha()
        {
            // user input
            string user = (txtCaptcha.Text ?? string.Empty).Trim();

            // expected code
            string expect = Session[CSE445_Assignment6.Captcha.SessionKeyName] as string;

            // clear session value after checking
            Session[CSE445_Assignment6.Captcha.SessionKeyName] = null;

            if (string.IsNullOrEmpty(expect) || string.IsNullOrEmpty(user))
            {
                return false;
            }

            return string.Equals(user, expect, StringComparison.OrdinalIgnoreCase);
        }

        // register button
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            Register?.Invoke(this, EventArgs.Empty);
        }
    }
}
