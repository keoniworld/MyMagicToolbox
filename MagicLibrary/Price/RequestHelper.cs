using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MagicLibrary;
using MyMagicCollection.Shared.Models;

namespace PriceLibrary
{
    public class RequestHelper
    {
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

        public RequestResult MakeRequest(string urlRequest)
        {
            String method = "GET";
            String url = string.Format("https://www.mkmapi.eu/ws/v1.1/{0}", urlRequest);

            var request = WebRequest.CreateHttp(url) as HttpWebRequest;
            var header = new OAuthHeader();
            request.Headers.Add(HttpRequestHeader.Authorization, header.getAuthorizationHeader(method, url));
            request.Method = method;

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            var document = XDocument.Load(response.GetResponseStream());
            // proceed further

            return new RequestResult
            {
                Response = document,
                HttpResponse = response,
            };
        }
    }
}