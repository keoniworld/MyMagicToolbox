using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMagicCollection.Shared.Price
{
    public class MkmSellerArticleData
    {
        public string SellerName { get; set; }

        public decimal Price { get; set; }

        public string SellerCountry { get; set; }

        public string CardCondition { get; set; }

        public bool IsFoil { get; set; }

        public bool IsPlayset { get; set; }
    }
}