namespace MagicContracts
{
    public class CardPrice
    {
        public string CardId { get; set; }

        public decimal? PriceTrend { get; set; }

        public decimal? PriceLow { get; set; }

        public decimal? PriceAvg { get; set; }

        public decimal? PriceFoilLow { get; set; }

        public string ImagePath { get; set; }

        public string WebSite { get; set; }
    }
}