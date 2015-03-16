using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.ViewModels;
using NLog;

namespace MyMagicCollection.Shared.Price
{
    public class CardPriceRequest
    {
        private readonly INotificationCenter _notificationCenter;

        public CardPriceRequest(INotificationCenter notificationCenter)
        {
            _notificationCenter = notificationCenter;
        }

        public void PerformRequest(
            IMagicCardDefinition card,
            MagicCardPrice cardPrice)
        {
            try
            {
                var foundSet = false;
                var setDefinition = StaticMagicData.SetDefinitionsBySetCode[card.SetCode];

                var setName = setDefinition.Name;
                switch (setName)
                {
                    case "Time Spiral \"Timeshifted\"":
                        setName = "Time Spiral";
                        break;

                    case "Friday Night Magic":
                        setName = "Friday Night Magic Promos";
                        break;
                }

                var helper = new RequestHelper();
                var result = helper.MakeRequest(RequestHelper.CreateGetProductsUrl(card.NameEN, MagicLanguage.English, true, null));

                var productNodes = result.Response.Root.Elements("product");
                foreach (var productNode in productNodes)
                {
                    var expansion = productNode.Element("expansion");
                    if (expansion == null || !expansion.Value.Equals(setName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    foundSet = true;
                    var priceGuide = productNode.Element("priceGuide");
                    if (priceGuide != null)
                    {
                        cardPrice.IsNotifying = false;
                        try
                        {
                            cardPrice.PriceLow = decimal.Parse(priceGuide.Element("LOW").Value, CultureInfo.InvariantCulture);
                            cardPrice.PriceAvg = decimal.Parse(priceGuide.Element("AVG").Value, CultureInfo.InvariantCulture);
                            cardPrice.PriceFoilLow = decimal.Parse(priceGuide.Element("LOWFOIL").Value, CultureInfo.InvariantCulture);
                            cardPrice.PriceTrend = decimal.Parse(priceGuide.Element("TREND").Value, CultureInfo.InvariantCulture);

                            cardPrice.ImagePath = productNode.Element("image").Value;
                            cardPrice.WebSite = productNode.Element("website").Value;
                        }
                        finally
                        {
                            cardPrice.IsNotifying = true;
                        }
                    }

                    cardPrice.UpdateUtc = DateTime.UtcNow;
                    _notificationCenter.FireNotification(LogLevel.Debug, string.Format("Downloaded price data for '{0}({1})': {2}/{3}/{4}", card.NameEN, card.SetCode, cardPrice.PriceLow, cardPrice.PriceAvg, cardPrice.PriceTrend));
                }

                if (!foundSet)
                {
                    _notificationCenter.FireNotification(LogLevel.Debug, string.Format("Cannot find price data for '{0}({1})'. Request: {2}", card.NameEN, setDefinition.Name, result.Response.Root));
                }
            }
            catch (Exception error)
            {
                _notificationCenter.FireNotification(LogLevel.Error, string.Format("Error downloading price data for '{0}({1})': {2}", card.NameEN, card.SetCode, error.Message));
            }
        }

        public void PerformRequest(
            IEnumerable<MagicBinderCardViewModel> viewModels)
        {
            try
            {
                var grouped = viewModels.GroupBy(vm => vm.CardNameEN);
                var helper = new RequestHelper();

                foreach (var cardsForName in grouped)
                {
                    var card = cardsForName.First();
                    var result = helper.MakeRequest(RequestHelper.CreateGetProductsUrl(card.CardNameEN, MagicLanguage.English, true, null));

                    var setCodes = cardsForName.ToDictionary(c => c.Definition.SetCode);

                    var productNodes = result.Response.Root.Elements("product");
                    foreach (var productNode in productNodes)
                    {
                        var expansion = productNode.Element("expansion");
                        MagicSetDefinition definition;
                        if (!StaticMagicData.SetDefinitionsBySetName.TryGetValue(expansion.Value, out definition)
                            || !setCodes.ContainsKey(definition.Code))
                        {
                            continue;
                        }

                        var cardPrice = setCodes[definition.Code].CardPrice;
                        var priceGuide = productNode.Element("priceGuide");
                        if (priceGuide != null)
                        {
                            cardPrice.IsNotifying = false;
                            try
                            {
                                cardPrice.PriceLow = decimal.Parse(priceGuide.Element("LOW").Value, CultureInfo.InvariantCulture);
                                cardPrice.PriceAvg = decimal.Parse(priceGuide.Element("AVG").Value, CultureInfo.InvariantCulture);
                                cardPrice.PriceFoilLow = decimal.Parse(priceGuide.Element("LOWFOIL").Value, CultureInfo.InvariantCulture);
                                cardPrice.PriceTrend = decimal.Parse(priceGuide.Element("TREND").Value, CultureInfo.InvariantCulture);

                                cardPrice.ImagePath = productNode.Element("image").Value;
                                cardPrice.WebSite = productNode.Element("website").Value;
                            }
                            finally
                            {
                                cardPrice.IsNotifying = true;
                            }
                        }

                        cardPrice.UpdateUtc = DateTime.UtcNow;
                    }

                    var downloadedList = new List<string>();
                    var errorList = new List<string>();

                    foreach (var singleCard in cardsForName)
                    {
                        var definition = StaticMagicData.SetDefinitionsBySetCode[singleCard.Definition.SetCode];
                        if (!singleCard.CardPrice.UpdateUtc.HasValue)
                        {
                            errorList.Add(
                                string.Format("Cannot find price data for '{0}({1})'. Request: {2}", singleCard.CardNameEN, definition.Name, result.Response.Root));
                        }
                        else
                        {
                            downloadedList.Add(
                                string.Format(
                                        "Downloaded price data for '{0}({1})': {2}/{3}/{4}",
                                        singleCard.CardNameEN,
                                        definition.Name,
                                        singleCard.CardPrice.PriceLow,
                                        singleCard.CardPrice.PriceAvg,
                                        singleCard.CardPrice.PriceTrend));
                        }
                    }

                    if (downloadedList.Any())
                    {
                        _notificationCenter.FireNotification(
                            LogLevel.Debug,
                            string.Join(Environment.NewLine, downloadedList));
                    }

                    if (errorList.Any())
                    {
                        _notificationCenter.FireNotification(
                            LogLevel.Error,
                            string.Join(Environment.NewLine, errorList));
                    }
                }
            }
            catch (Exception error)
            {
                // _notificationCenter.FireNotification(LogLevel.Error, string.Format("Error downloading price data for '{0}({1})': {2}", card.NameEN, card.SetCode, error.Message));
            }
        }
    }
}