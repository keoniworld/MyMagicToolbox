using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MyMagicCollection.Shared.Models;
using NLog;

namespace MyMagicCollection.Shared.Price
{
    public class RequestHelper
    {
        private INotificationCenter _notificationCenter;
        private readonly string _additionalLogText;
        public RequestHelper(INotificationCenter notificationCenter, string additionalLogText)
        {
            _notificationCenter = notificationCenter;
            _additionalLogText = additionalLogText;
        }

        public static string CreateGetProductsUrl(
            string name,
            MagicLanguage language,
            bool isExact,
            int? startIndex)
        {
            var builder = new StringBuilder();
            builder.Append("products/");
            builder.Append(WebUtility.UrlEncode(name) + "/1/");
            builder.Append((int)language + "/");
            builder.Append(isExact ? "true" : "false");

            if (startIndex.HasValue)
            {
                builder.Append("/" + startIndex.Value);
            }

            return builder.ToString();
        }

        public static string CreateGetArticlesUrl(
             string name,
             MagicLanguage language,
             bool isExact,
             int? startIndex)
        {
            var builder = new StringBuilder();
            builder.Append("articles/");
            builder.Append(WebUtility.UrlEncode(name));

            if (startIndex.HasValue)
            {
                builder.Append("/" + startIndex.Value);
            }

            return builder.ToString();
        }

        public RequestResult MakeRequest(string urlRequest)
        {
            String method = "GET";
            String url = string.Format("https://www.mkmapi.eu/ws/v1.1/{0}", urlRequest);

            var stopwatch = Stopwatch.StartNew();

            _notificationCenter.FireNotification(
               LogLevel.Debug,
               string.Format("Request '{0}' started...", url) + _additionalLogText);

            var request = WebRequest.CreateHttp(url) as HttpWebRequest;
            request.Timeout = 1000 * 30;

            var header = new OAuthHeader();
            request.Headers.Add(HttpRequestHeader.Authorization, header.getAuthorizationHeader(method, url));
            request.Method = method;

            HttpWebResponse response = null;
            XDocument document = null;

            try
            {
                response = request.GetResponse() as HttpWebResponse;
                document = response.StatusCode != HttpStatusCode.NoContent
                    ? XDocument.Load(response.GetResponseStream())
                    : null;
            }
            catch (Exception error)
            {
				var seconds = 30;

				var message = error.Message;
				if (message.Contains("(429)"))
				{
					// Too many requests
					message = message.Replace("(429)", "(429 - Too Many requests)");
                    seconds = 0;
                }

                _notificationCenter.FireNotification(
                    LogLevel.Error,
                    string.Format("Request '{0}' throwed error: {1} (waiting {2} seconds now to avoid timeout)", url, error.Message, seconds) + _additionalLogText);

                Thread.Sleep(seconds * 1000);

                throw;
            }

            stopwatch.Stop();

            _notificationCenter.FireNotification(
                LogLevel.Debug,
                string.Format("Request '{0}' took {1}", url, stopwatch.Elapsed) + _additionalLogText);

            // proceed further

            return new RequestResult
            {
                Response = document,
                HttpResponse = response,
            };
        }
    }
}