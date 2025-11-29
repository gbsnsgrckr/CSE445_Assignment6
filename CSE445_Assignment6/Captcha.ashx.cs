using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.SessionState;


namespace CSE445_Assignment6
{
    // Local captcha to attempt to stop automated access
    public class Captcha : IHttpHandler, IRequiresSessionState // try
    {
        // Set session key
        private const string SessionKey = "CaptchaOK";

        public void ProcessRequest(HttpContext context)
        {
            // Generate and set random code
            string captchaCode = GenerateCode(5);
            context.Session[SessionKey] = captchaCode;

            // Draw box
            int imgWidth = 175, imgHeight = 40;
            using (var bitmap = new Bitmap(imgWidth, imgHeight))
            using (var graphics = Graphics.FromImage(bitmap))
            using (var memStream = new MemoryStream())
            {
                // test
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                graphics.Clear(Color.FromArgb(250, 250, 250));

                var rnd = new Random();

                // Draw lines to obscure - Light
                for (int i = 0; i < 10; i++)
                {
                    // Pick points to draw a line between
                    var point1 = new Point(rnd.Next(0, imgWidth), rnd.Next(0, imgHeight));
                    var point2 = new Point(rnd.Next(0, imgWidth), rnd.Next(0, imgHeight));

                    using (var pen = new Pen(Color.FromArgb(200, 200, 200), 3))
                    {
                        graphics.DrawLine(pen, point1, point2);
                    }
                }

                // Draw lines to obscure - Dark
                for (int i = 0; i < 10; i++)
                {
                    // Pick points to draw a line between
                    var point1 = new Point(rnd.Next(0, imgWidth), rnd.Next(0, imgHeight));
                    var point2 = new Point(rnd.Next(0, imgWidth), rnd.Next(0, imgHeight));

                    using (var pen = new Pen(Color.FromArgb(60, 60, 60), 1))
                    {
                        graphics.DrawLine(pen, point1, point2);
                    }
                }

                // Variables to position letters
                int baseY = (int)(imgHeight * 0.08);
                int xOffset = 10;
                // Horizontal steps from letter to letter
                int step = 32;

                // For each letter in the code
                for (int i = 0; i < captchaCode.Length; i++)
                {
                    using (var font = new Font("Segoe UI", 30, FontStyle.Bold))
                    using (var path = new GraphicsPath())
                    {
                        string code = captchaCode[i].ToString();
                        float angle = rnd.Next(-20, 20);

                        // Save
                        var state = graphics.Save();

                        // Move right and manipulate
                        graphics.TranslateTransform(xOffset, baseY);
                        graphics.RotateTransform(angle);

                        path.AddString(code, font.FontFamily, (int)FontStyle.Bold, 30,
                                       new Point(0, 0), StringFormat.GenericDefault);

                        using (var brush = new SolidBrush(Color.FromArgb(60, 60, 60)))
                            graphics.FillPath(brush, path);

                        graphics.Restore(state);
                        xOffset += step;
                    }
                }

                // Draw dots to obscure
                for (int i = 0; i < 250; i++)
                {
                    bitmap.SetPixel(rnd.Next(imgWidth), rnd.Next(imgHeight), Color.FromArgb(120, 120, 120));
                }

                // Output as PNG
                var response = HttpContext.Current.Response;
                response.ContentType = "image/png";
                response.Cache.SetCacheability(HttpCacheability.NoCache);
                response.Cache.SetNoStore();
                // Save
                bitmap.Save(memStream, ImageFormat.Png);

                memStream.WriteTo(response.OutputStream);
            }
        }

        // Make sure a new Captcha is generated each request
        public bool IsReusable => false;

        // Generate code for captcha using specific characters
        private static string GenerateCode(int length)
        {
            // Removed characters that may cause problems - 0/O/1/I
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var rnd = new Random();
            var arr = new char[length];
            
            // Loop as many times as necessary for requested length
            for (int i = 0; i < length; i++)
            {
                arr[i] = chars[rnd.Next(chars.Length)];
            }
            return new string(arr);
        }

        // try expose the SessionKey so login code can reference it - for A6
        public static string SessionKeyName => SessionKey;
    }
}
