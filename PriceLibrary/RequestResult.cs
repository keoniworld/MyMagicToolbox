using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PriceLibrary
{
    public class RequestResult : IDisposable
    {
        public XDocument Response { get; set; }
        public HttpWebResponse HttpResponse { get; set; }

        public void Dispose()
        {
            if (HttpResponse != null)
            {
                HttpResponse.Dispose();
                HttpResponse = null;
            }

            Response = null;
        }
    }
}
