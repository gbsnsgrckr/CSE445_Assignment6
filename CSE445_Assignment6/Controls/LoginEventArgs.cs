using System;

namespace CSE445_Assignment6.Controls
{
    /// <summary>
    /// login event arguments
    /// </summary>
    public class LoginEventArgs : EventArgs
    {
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Remember me
        /// </summary>
        public bool RememberMe { get; set; }

        public LoginEventArgs()
        {
        }

        public LoginEventArgs(string username, string password, bool rememberMe)
        {
            Username = username;
            Password = password;
            RememberMe = rememberMe;
        }
    }
}
