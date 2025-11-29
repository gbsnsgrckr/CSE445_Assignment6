using System;
using System.Web;
using System.Web.UI;

namespace CSE445_Assignment6
{
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string username = (txtUser.Text ?? string.Empty).Trim();
            string password = txtPass.Text ?? string.Empty;
            string confirm = txtConfirm.Text ?? string.Empty;

            // input validation
            if (string.IsNullOrWhiteSpace(username))
            {
                litMsg.Text = "<span style='color:#b00;'>Error: Username is required.</span>";
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                litMsg.Text = "<span style='color:#b00;'>Error: Password is required.</span>";
                return;
            }

            if (!string.Equals(password, confirm, StringComparison.Ordinal))
            {
                litMsg.Text = "<span style='color:#b00;'>Error: Password and Confirm Password do not match.</span>";
                return;
            }

            // validate captcha
            string userCaptcha = (txtCaptcha.Text ?? string.Empty).Trim();
            string expected = Session[CSE445_Assignment6.Captcha.SessionKeyName] as string;

            // clear captcha
            Session[CSE445_Assignment6.Captcha.SessionKeyName] = null;

            if (string.IsNullOrEmpty(expected) || string.IsNullOrEmpty(userCaptcha) ||
                !string.Equals(userCaptcha, expected, StringComparison.OrdinalIgnoreCase))
            {
                litMsg.Text = "<span style='color:#b00;'>Error: Captcha is incorrect.</span>";

                return;
            }

            // save in Users.xml/Member.xml
            string error;

            if (!Account.RegisterMember(username, password, out error))
            {
                litMsg.Text = "<span style='color:#b00;'>Error: " + Server.HtmlEncode(error ?? "Registration failed.") +
                              "</span>";

                return;
            }

            // save cookie
            Response.Cookies["LastUser"].Value = username;
            Response.Cookies["LastUser"].Expires = DateTime.UtcNow.AddDays(7);

            // debug - success message
            litMsg.Text =
                "<span style='color:green;'>Registration successful. " +
                "You may now <a href='Login.aspx'>log in</a> with your new account.</span>";

            // clear fields
            txtPass.Text = string.Empty;
            txtConfirm.Text = string.Empty;
            txtCaptcha.Text = string.Empty;
        }
    }
}
