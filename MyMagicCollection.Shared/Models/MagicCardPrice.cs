using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMagicCollection.Shared.Models
{
    public class MagicCardPrice
    {
        public string CardId { get; set; }

        public decimal? PriceTrend { get; set; }

        public decimal? PriceLow { get; set; }

        public decimal? PriceAvg { get; set; }

        public decimal? PriceFoilLow { get; set; }
    }
}
