using CSE445_Assignment6.SecurityLib;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace CSE445_Assignment6
{
    /// <summary>
    /// helper methods for validating and registering users.
    /// </summary>
    public static class Account
    {
        private static string MapPath(string relative)
        {
            return HttpContext.Current.Server.MapPath(relative);
        }

        private static string UsersXmlPath
        {
            get
            {
                return MapPath("~/App_Data/Users.xml");
            }
        }

        private static string MemberXmlPath
        {
            get
            {
                return MapPath("~/App_Data/Member.xml");
            }
        }

        private static string StaffXmlPath
        {
            get
            {
                return MapPath("~/App_Data/Staff.xml");
            }
        }

        private static string HashPassword(string plain)
        {
            return Encryption.Sha256Hex(plain ?? string.Empty);
        }

        /// <summary>
        /// validates username/password against Users.xml
        /// </summary>
        private static bool TryValidateUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            username = username.Trim();
            string hash = HashPassword(password);

            var doc = LoadOrCreateUsersDoc();

            return doc.Root
                      .Elements("User")
                      .Any(u =>
                           (string)u.Element("Username") == username &&
                           (string)u.Element("PasswordHash") == hash);
        }

        /// <summary>
        /// registers a new user in Users.xml and adds to Member.xml
        /// </summary>
        public static bool RegisterMember(string username, string password, out string error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(username))
            {
                error = "Username is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                error = "Password is required.";
                return false;
            }

            username = username.Trim();

            var usersDoc = LoadOrCreateUsersDoc();
            var memberDoc = LoadOrCreateMemberDoc();

            // check if username already in Users.xml
            bool exists = usersDoc.Root
                                  .Elements("User")
                                  .Any(u => (string)u.Element("Username") == username);

            if (exists)
            {
                error = "That username already exists.";
                return false;
            }

            // add to Users.xml
            usersDoc.Root.Add(
                new XElement("User",
                    new XElement("Username", username),
                    new XElement("PasswordHash", HashPassword(password))
                ));
            SaveUsersDoc(usersDoc);

            // add to Member.xml
            if (!IsMember(username))
            {
                memberDoc.Root.Add(new XElement("User", username));
                SaveMemberDoc(memberDoc);
            }

            return true;
        }

        /// <summary>
        /// change password in Users.xml
        /// </summary>
        public static bool ChangeMemberPassword(string username, string oldPassword, string newPassword, out string error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(username))
            {
                error = "No username provided.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                error = "Old and new passwords are required.";
                return false;
            }

            username = username.Trim();

            var usersDoc = LoadOrCreateUsersDoc();

            var user = usersDoc.Root.Elements("User").FirstOrDefault(u => (string)u.Element("Username") == username);

            if (user == null)
            {
                error = "User not found.";
                return false;
            }

            string existingHash = (string)user.Element("PasswordHash") ?? string.Empty;
            string oldHash = HashPassword(oldPassword);

            if (!string.Equals(existingHash, oldHash, StringComparison.Ordinal))
            {
                error = "Old password is incorrect.";
                return false;
            }

            user.SetElementValue("PasswordHash", HashPassword(newPassword));
            SaveUsersDoc(usersDoc);
            return true;
        }

        /// <summary>
        /// validates username/password and checks member-role in Member.xml.
        /// </summary>
        public static bool TryValidateMember(string username, string password)
        {
            if (!TryValidateUser(username, password))
                return false;

            return IsMember(username);
        }

        /// <summary>
        /// validates username/password and checks staff-role in Staff.xml.
        /// </summary>
        public static bool TryValidateStaff(string username, string password)
        {
            if (!TryValidateUser(username, password))
                return false;

            return IsStaff(username);
        }

        /// <summary>
        /// checks member-role in Member.xml
        /// </summary>
        public static bool IsMember(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            username = username.Trim();

            var doc = LoadOrCreateMemberDoc();

            return doc.Root
                      .Elements("User")
                      .Any(u => (string)u == username);
        }

        /// <summary>
        /// checks staff-role in Staff.xml
        /// </summary>
        public static bool IsStaff(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            username = username.Trim();

            var doc = LoadOrCreateStaffDoc();

            return doc.Root
                      .Elements("User")
                      .Any(u => (string)u == username);
        }

        // xml helpers

        private static XDocument LoadOrCreateUsersDoc()
        {
            if (!File.Exists(UsersXmlPath))
            {
                var doc = new XDocument(new XElement("Users"));
                doc.Save(UsersXmlPath);
                return doc;
            }

            return XDocument.Load(UsersXmlPath);
        }

        private static void SaveUsersDoc(XDocument doc)
        {
            doc.Save(UsersXmlPath);
        }

        private static XDocument LoadOrCreateMemberDoc()
        {
            if (!File.Exists(MemberXmlPath))
            {
                var doc = new XDocument(new XElement("Members"));
                doc.Save(MemberXmlPath);
                return doc;
            }

            return XDocument.Load(MemberXmlPath);
        }

        private static void SaveMemberDoc(XDocument doc)
        {
            doc.Save(MemberXmlPath);
        }

        private static XDocument LoadOrCreateStaffDoc()
        {
            if (!File.Exists(StaffXmlPath))
            {
                var doc = new XDocument(new XElement("Staff"));
                doc.Save(StaffXmlPath);
                return doc;
            }

            return XDocument.Load(StaffXmlPath);
        }
    }
}
