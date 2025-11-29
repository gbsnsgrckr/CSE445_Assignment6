using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;


namespace CSE445_Assignment6.SecurityLib
{
    /// <summary>
    /// Security/Hashing functions for login information encryption - SHA256 hashing to hex/Base64
    /// </summary>
    public static class Encryption
    {
        // SHA256 encryption - Hex
        public static string Sha256Hex(string input)
        {
            // If null set empty
            if (input == null)
            {
                input = string.Empty;
            }

            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(bytes.Length * 2);

                foreach (var b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        // SHA256 encryption - Base64
        public static string Sha256Base64(string input)
        {
            // if null set empty
            if (input == null)
            {
                input = string.Empty;
            }

            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));

                return Convert.ToBase64String(bytes);
            }
        }
    }
}
