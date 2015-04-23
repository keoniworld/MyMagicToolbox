using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyMagicCollection.Shared.Price
{
    public class RequestResult : IDisposable
    {
		~RequestResult()
		{
			Dispose();
		}

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
