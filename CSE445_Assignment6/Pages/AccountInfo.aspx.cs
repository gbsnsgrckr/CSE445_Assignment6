using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Xml.Linq;

namespace CSE445_Assignment6
{
    public partial class AccountInfo : Page
    {
        private string UsersXmlPath => Server.MapPath("~/App_Data/Users.xml");
        private string MemberXmlPath => Server.MapPath("~/App_Data/Member.xml");
        private string StaffXmlPath => Server.MapPath("~/App_Data/Staff.xml");

        protected void Page_Load(object sender, EventArgs e)
        {
            // try make sure they've logged in again
            if (!User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                string uname = User.Identity.Name ?? "(unknown)";

                litCurrentUser.Text = Server.HtmlEncode(uname);
                litUserReadonly.Text = Server.HtmlEncode(uname);

                // Load current PrefZip from Users.xml for display
                try
                {
                    var usersDoc = XDocument.Load(UsersXmlPath);

                    var userElem = usersDoc.Root
                        .Elements("User")
                        .FirstOrDefault(u => string.Equals(
                            (string)u.Element("Username"),
                            uname,
                            StringComparison.OrdinalIgnoreCase));

                    if (userElem != null)
                    {
                        var zipElem = userElem.Element("PrefZip");
                        if (zipElem != null && !string.IsNullOrWhiteSpace(zipElem.Value))
                        {
                            litZipReadOnly.Text = Server.HtmlEncode(zipElem.Value.Trim());
                        }
                        else
                        {
                            litZipReadOnly.Text = "(none)";
                        }
                    }
                    else
                    {
                        litZipReadOnly.Text = "(not found)";
                    }
                }
                catch (Exception ex)
                {
                    // If something goes wrong, don't crash the page; just show a placeholder
                    litZipReadOnly.Text = "(error loading zip)";
                    System.Diagnostics.Debug.WriteLine("Error loading PrefZip: " + ex.Message);
                }
            }
        }

        // change password handler
        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            litPwdMsg.Text = "";

            if (!User.Identity.IsAuthenticated)
            {
                litPwdMsg.Text = "<span class='msg-err'>Error: You must be logged in.</span>";
                return;
            }

            string username = User.Identity.Name ?? "";
            string currentPwd = txtCurrentPassword.Text ?? "";
            string newPwd = txtNewPassword.Text ?? "";
            string confirm = txtConfirmPassword.Text ?? "";

            if (string.IsNullOrWhiteSpace(currentPwd) ||
                string.IsNullOrWhiteSpace(newPwd) ||
                string.IsNullOrWhiteSpace(confirm))
            {
                litPwdMsg.Text = "<span class='msg-err'>Error: All password fields are required.</span>";
                return;
            }

            if (!string.Equals(newPwd, confirm, StringComparison.Ordinal))
            {
                litPwdMsg.Text = "<span class='msg-err'>Error: New password and confirmation do not match.</span>";
                return;
            }

            string error;

            if (Account.ChangeMemberPassword(username, currentPwd, newPwd, out error))
            {
                litPwdMsg.Text = "<span class='msg-ok'>Password changed successfully.</span>";

                // clear text boxes
                txtCurrentPassword.Text = "";
                txtNewPassword.Text = "";
                txtConfirmPassword.Text = "";
            }
            else
            {
                litPwdMsg.Text = "<span class='msg-err'>" + Server.HtmlEncode(error) + "</span>";
            }
        }

        // change username handler
        protected void btnChangeUsername_Click(object sender, EventArgs e)
        {
            litUserMsg.Text = "";

            if (!User.Identity.IsAuthenticated)
            {
                litUserMsg.Text = "<span class='msg-err'>Error: You must be logged in.</span>";
                return;
            }

            string current = User.Identity.Name ?? "";
            string requested = (txtNewUsername.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(requested))
            {
                litUserMsg.Text = "<span class='msg-err'>Error: New username is required.</span>";
                return;
            }

            if (string.Equals(current, requested, StringComparison.Ordinal))
            {
                litUserMsg.Text = "<span class='msg-err'>Error: New username must be different from current.</span>";
                return;
            }

            try
            {
                // update Users.xml
                var usersDoc = XDocument.Load(UsersXmlPath);

                bool existsNew = usersDoc.Root
                    .Elements("User")
                    .Any(u => (string)u.Element("Username") == requested);

                if (existsNew)
                {
                    litUserMsg.Text = "<span class='msg-err'>Error: That username is already in use.</span>";
                    return;
                }

                var userElem = usersDoc.Root
                    .Elements("User")
                    .FirstOrDefault(u => (string)u.Element("Username") == current);

                if (userElem == null)
                {
                    litUserMsg.Text = "<span class='msg-err'>Error: Current user not found in Users.xml.</span>";
                    return;
                }

                var usernameNode = userElem.Element("Username");
                if (usernameNode == null)
                {
                    litUserMsg.Text = "<span class='msg-err'>Error: Users.xml has no Username element for this user.</span>";
                    return;
                }

                usernameNode.Value = requested;
                usersDoc.Save(UsersXmlPath);

                // update Member.xml
                if (System.IO.File.Exists(MemberXmlPath))
                {
                    var memberDoc = XDocument.Load(MemberXmlPath);
                    foreach (var node in memberDoc.Root.Elements())
                    {
                        // Member.xml is usually <Members><User>username</User>...
                        if (string.Equals((string)node, current, StringComparison.Ordinal))
                        {
                            node.Value = requested;
                        }
                    }
                    memberDoc.Save(MemberXmlPath);
                }

                // update Staff.xml
                if (System.IO.File.Exists(StaffXmlPath))
                {
                    var staffDoc = XDocument.Load(StaffXmlPath);
                    foreach (var node in staffDoc.Root.Elements())
                    {
                        if (string.Equals((string)node, current, StringComparison.Ordinal))
                        {
                            node.Value = requested;
                        }
                    }
                    staffDoc.Save(StaffXmlPath);
                }

                // cookies / auth ticket
                FormsAuthentication.SignOut();
                FormsAuthentication.SetAuthCookie(requested, false);

                litCurrentUser.Text = Server.HtmlEncode(requested);
                litUserReadonly.Text = Server.HtmlEncode(requested);
                txtNewUsername.Text = "";

                litUserMsg.Text = "<span class='msg-ok'>Username changed successfully.</span>";
            }
            catch (Exception ex)
            {
                litUserMsg.Text = "<span class='msg-err'>Error updating username: "
                                  + Server.HtmlEncode(ex.Message) + "</span>";
            }
        }

        // change preferred Zip code handler
        protected void btnChangeZip_Click(object sender, EventArgs e)
        {
            litZipMsg.Text = "";

            if (!User.Identity.IsAuthenticated)
            {
                litZipMsg.Text = "<span class='msg-err'>Error: You must be logged in.</span>";
                return;
            }

            string username = User.Identity.Name ?? "";
            string newZip = (txtNewZip.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(newZip))
            {
                litZipMsg.Text = "<span class='msg-err'>Error: New Zip code is required.</span>";
                return;
            }

            if (newZip.Length != 5 || !newZip.All(char.IsDigit))
            {
                litZipMsg.Text = "<span class='msg-err'>Error: Zip code must be exactly 5 digits.</span>";
                return;
            }

            try
            {
                var usersDoc = XDocument.Load(UsersXmlPath);

                var userElem = usersDoc.Root
                    .Elements("User")
                    .FirstOrDefault(u => string.Equals(
                        (string)u.Element("Username"),
                        username,
                        StringComparison.OrdinalIgnoreCase));

                if (userElem == null)
                {
                    litZipMsg.Text = "<span class='msg-err'>Error: Current user not found in Users.xml.</span>";
                    return;
                }

                var zipElem = userElem.Element("PrefZip");
                if (zipElem == null)
                {
                    zipElem = new XElement("PrefZip", newZip);
                    userElem.Add(zipElem);
                }
                else
                {
                    zipElem.Value = newZip;
                }

                usersDoc.Save(UsersXmlPath);

                litZipReadOnly.Text = Server.HtmlEncode(newZip);
                txtNewZip.Text = "";

                litZipMsg.Text = "<span class='msg-ok'>Zip code updated successfully.</span>";
            }
            catch (Exception ex)
            {
                litZipMsg.Text = "<span class='msg-err'>Error updating Zip code: "
                                 + Server.HtmlEncode(ex.Message) + "</span>";
            }
        }
    }
}
