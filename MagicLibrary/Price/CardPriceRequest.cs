using System;
using System.Globalization;
using MagicLibrary;
using MyMagicCollection.Shared.Models;

namespace PriceLibrary
{
    public class CardPriceRequest : CardPrice
    {
        private IDeckCard _card;

        public CardPriceRequest(IDeckCard card)
        {
            _card = card;
            CardId = card.CardId;
        }

        public void PerformRequest()
        {
            try
            {
                var helper = new RequestHelper();
                var result = helper.MakeRequest(RequestHelper.CreateGetProductsUrl(_card.Name, MagicLanguage.English, true, null));

                var productNode = result.Response.Root.Element("product");
                var priceGuide = productNode.Element("priceGuide");
                if (priceGuide != null)
                {
                    PriceLow = decimal.Parse(priceGuide.Element("LOW").Value, CultureInfo.InvariantCulture);
                    PriceAvg = decimal.Parse(priceGuide.Element("AVG").Value, CultureInfo.InvariantCulture);
                    PriceFoilLow = decimal.Parse(priceGuide.Element("LOWFOIL").Value, CultureInfo.InvariantCulture);
                    PriceTrend = decimal.Parse(priceGuide.Element("TREND").Value, CultureInfo.InvariantCulture);
                }

                ImagePath = productNode.Element("image").Value;
                WebSite = productNode.Element("website").Value;
            }
            catch (Exception error)
            {
                // TODO
            }
        }
    }
}