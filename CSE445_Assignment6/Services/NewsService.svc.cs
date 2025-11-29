using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace CSE445_Assignment6.NewsService
{
    public class NewsService : INewsService
    {
        private const string NewsUrl =
            "https://newsdata.io/api/1/latest?apikey={0}&q={1}&language=en";

        public string[] NewsFocus(string[] topics)
        {
            var cleanTopics = (topics ?? new string[0])
                .Select(t => (t ?? "").Trim())
                .Where(t => t.Length > 0)
                .ToArray();

            if (cleanTopics.Length == 0)
                return new[] { "<li>Please enter at least one topic.</li>" };

            string query = string.Join(" OR ", cleanTopics);
            string apiKey = ConfigurationManager.AppSettings["NewsDataApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
                return new[] { "<li>Error: NewsDataApiKey missing in Web.config</li>" };

            string url =
                "https://newsdata.io/api/1/latest?apikey=" +
                HttpUtility.UrlEncode(apiKey) +
                "&q=" + HttpUtility.UrlEncode(query) +
                "&language=en";

            using (var client = new WebClient())
            {
                string json = client.DownloadString(url);

                // simple filtering
                if (json == null || !json.Contains("results"))
                    return new[] { $"<li>No articles found for {HttpUtility.HtmlEncode(query)}.</li>" };

                // manually extract "link", "title", and "image_url"
                var seenLinks = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var items = new List<string>();

                int index = 0;
                while (items.Count < 3)
                {
                    int linkStart = json.IndexOf("\"link\":\"", index);
                    if (linkStart == -1) break;
                    linkStart += 8;
                    int linkEnd = json.IndexOf("\"", linkStart);
                    if (linkEnd == -1) break;

                    string link = json.Substring(linkStart, linkEnd - linkStart).Replace("\\/", "/");

                    int titleStart = json.IndexOf("\"title\":\"", linkEnd);
                    if (titleStart == -1) break;
                    titleStart += 9;
                    int titleEnd = json.IndexOf("\"", titleStart);
                    if (titleEnd == -1) break;

                    string title = json.Substring(titleStart, titleEnd - titleStart);

                    int imgStart = json.IndexOf("\"image_url\":\"", titleEnd);
                    string image = "";
                    if (imgStart != -1)
                    {
                        imgStart += 13;
                        int imgEnd = json.IndexOf("\"", imgStart);
                        if (imgEnd != -1)
                            image = json.Substring(imgStart, imgEnd - imgStart).Replace("\\/", "/");
                    }

                    // Skip duplicates
                    if (!seenLinks.Add(link))
                    {
                        index = linkEnd;
                        continue;
                    }

                    // Construct HTML
                    string safeLink = HttpUtility.HtmlAttributeEncode(link);
                    string safeTitle = HttpUtility.HtmlEncode(title);

                    string imgHtml = "";
                    if (!string.IsNullOrWhiteSpace(image))
                    {
                        string safeImg = HttpUtility.HtmlAttributeEncode(image);
                        imgHtml = $"<div><img src=\"{safeImg}\" alt=\"{safeTitle}\" " +
                                  "style=\"max-width:220px;height:auto;border-radius:6px;margin-bottom:4px;\" /></div>";
                    }

                    string li = $"<li>{imgHtml}<a href=\"{safeLink}\" target=\"_blank\">{safeTitle}</a></li>";
                    items.Add(li);

                    // move search forward
                    index = linkEnd;
                }

                if (items.Count == 0)
                    return new[] { $"<li>No non-duplicate articles for {HttpUtility.HtmlEncode(query)}.</li>" };

                return items.ToArray();
            }
        }



        private static string DownloadString(string url)
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.UserAgent,
                    "CSE445-Assignment6-NewsService");
                wc.Encoding = System.Text.Encoding.UTF8;
                return wc.DownloadString(url);
            }
        }

        public class NewsRoot
        {
            public string status { get; set; }
            public int totalResults { get; set; }
            public List<NewsArticle> results { get; set; }
        }

        public class NewsArticle
        {
            public string article_id { get; set; }
            public string link { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string image_url { get; set; }
        }
    }
}
