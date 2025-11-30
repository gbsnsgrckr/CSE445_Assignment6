using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Xml.Linq;

namespace CSE445_Assignment6
{
    public partial class Staff : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // check if user is authenticated and in Staff role
            if (!User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!User.IsInRole("Staff"))
            {
                // send to member page
                Response.Redirect("~/Pages/Member.aspx");

                return;
            }

            if (!IsPostBack)
            {
                string name = Context.User != null && Context.User.Identity != null && Context.User.Identity.IsAuthenticated
                    ? Context.User.Identity.Name
                    : "(unknown)";

                litUser.Text = Server.HtmlEncode(name);

                LoadMemberList();
                LoadStaffList();
                LoadUsersDropDown();
            }
        }

        private void LoadMemberList()
        {
            try
            {
                string path = Server.MapPath("~/App_Data/Member.xml");
                var doc = XDocument.Load(path);

                var names = doc.Root
                    .Elements()
                    .Select(x => (x.Name == "User" ? (string)x : (string)x.Value))
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList();

                if (names.Count == 0)
                {
                    litMembersList.Text = "<span class='small'>(No members found.)</span>";
                    return;
                }

                litMembersList.Text = "<ul>" + string.Join("", names.Select(n => "<li>" + Server.HtmlEncode(n) + "</li>")) + "</ul>";
            }
            catch (Exception ex)
            {
                litMembersList.Text = "<span style='color:#b00;'>Error reading Member.xml: "
                    + Server.HtmlEncode(ex.Message) + "</span>";
            }
        }

        private void LoadStaffList()
        {
            try
            {
                string path = Server.MapPath("~/App_Data/Staff.xml");
                var doc = XDocument.Load(path);

                var names = doc.Root
                    .Elements()
                    .Select(x => (x.Name == "User" ? (string)x : (string)x.Value))
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList();

                if (names.Count == 0)
                {
                    litStaffList.Text = "<span class='small'>(No staff found.)</span>";
                    return;
                }

                litStaffList.Text = "<ul>" + string.Join("", names.Select(n => "<li>" + Server.HtmlEncode(n) + "</li>")) + "</ul>";
            }
            catch (Exception ex)
            {
                litStaffList.Text = "<span style='color:#b00;'>Error reading Staff.xml: "
                    + Server.HtmlEncode(ex.Message) + "</span>";
            }
        }

        // load dropdown for users
        private void LoadUsersDropDown()
        {
            try
            {
                string path = Server.MapPath("~/App_Data/Users.xml");
                var doc = XDocument.Load(path);

                var names = doc.Root
                    .Elements("User").Select(u => (string)u.Element("Username")).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(s => s).ToList();

                ddlUsers.Items.Clear();

                if (names.Count == 0)
                {
                    ddlUsers.Items.Add("(no users)");
                    ddlUsers.Enabled = false;
                }
                else
                {
                    ddlUsers.Enabled = true;
                    foreach (var n in names)
                    {
                        ddlUsers.Items.Add(n);
                    }
                }
            }
            catch (Exception ex)
            {
                ddlUsers.Items.Clear();
                ddlUsers.Items.Add("(error loading users)");
                ddlUsers.Enabled = false;

                litResetMsg.Text = "<span style='color:#b00;'>Error reading Users.xml: "
                    + Server.HtmlEncode(ex.Message) + "</span>";
            }
        }

        // Staff reset password handler
        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            litResetMsg.Text = "";

            if (!ddlUsers.Enabled || ddlUsers.Items.Count == 0)
            {
                litResetMsg.Text = "<span style='color:#b00;'>No users found.</span>";

                return;
            }

            string targetUser = ddlUsers.SelectedValue;
            string newPwd = txtNewPassStaff.Text ?? "";
            string confirm = txtConfirmPassStaff.Text ?? "";

            if (string.IsNullOrWhiteSpace(targetUser) ||
                targetUser.StartsWith("("))
            {
                litResetMsg.Text = "<span style='color:#b00;'>Please select a valid user.</span>";

                return;
            }

            if (string.IsNullOrWhiteSpace(newPwd) || string.IsNullOrWhiteSpace(confirm))
            {
                litResetMsg.Text = "<span style='color:#b00;'>New password and confirmation are required.</span>";

                return;
            }

            if (!string.Equals(newPwd, confirm, StringComparison.Ordinal))
            {
                litResetMsg.Text = "<span style='color:#b00;'>New password and confirmation do not match.</span>";

                return;
            }

            try
            {
                string path = Server.MapPath("~/App_Data/Users.xml");
                var doc = XDocument.Load(path);

                var userElem = doc.Root
                    .Elements("User").FirstOrDefault(u => (string)u.Element("Username") == targetUser);

                if (userElem == null)
                {
                    litResetMsg.Text = "<span style='color:#b00;'>User not found.</span>";

                    return;
                }

                string hash = SecurityLib.Encryption.Sha256Hex(newPwd);
                userElem.SetElementValue("PasswordHash", hash);

                doc.Save(path);

                txtNewPassStaff.Text = "";
                txtConfirmPassStaff.Text = "";

                litResetMsg.Text = "<span style='color:#0a0;'>Password reset successfull for " + Server.HtmlEncode(targetUser) + ".</span>";
            }
            catch (Exception ex)
            {
                litResetMsg.Text = "<span style='color:#b00;'>Error resetting password: " + Server.HtmlEncode(ex.Message) + "</span>";
            }
        }
    }
}
