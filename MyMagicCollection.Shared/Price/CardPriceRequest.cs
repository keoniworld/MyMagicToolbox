using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.ViewModels;
using NLog;

namespace MyMagicCollection.Shared.Price
{
    public class CardPriceRequest
    {
        private static MkmRequestCounter _requestCounter = new MkmRequestCounter();

        private static int _timeoutHelper = 0;

        private static object _timeoutSync = new object();

        private readonly INotificationCenter _notificationCenter;

        public CardPriceRequest(INotificationCenter notificationCenter)
        {
            _notificationCenter = notificationCenter;
        }

        public void PerformRequest(
            IMagicCardDefinition card,
            MagicCardPrice cardPrice,
            bool notifyOfPriceUpdate,
            string additionalLogText)
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

                    case "Magic 2014 Core Set":
                        setName = "Magic 2014";
                        break;

                    case "Magic 2015 Core Set":
                        setName = "Magic 2015";
                        break;
                }

                var cardName = card.NameEN;

                if (!CheckRequestCount(false, true))
                {
                    return;
                }

                WorkaroundTimeout(_notificationCenter, additionalLogText);
                var helper = new RequestHelper(_notificationCenter, additionalLogText);
                using (var result = helper.MakeRequest(RequestHelper.CreateGetProductsUrl(cardName, MagicLanguage.English, card.MagicCardType != MagicCardType.Token, null)))
                {
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
                                cardPrice.PriceSell = decimal.Parse(priceGuide.Element("SELL").Value, CultureInfo.InvariantCulture);

                                cardPrice.ImagePath = productNode.Element("image").Value;
                                cardPrice.WebSite = productNode.Element("website").Value;

                                // Get lowest actual price from german vendor
                                var mkmId = productNode.Element("idProduct");
                                if (mkmId != null)
                                {
                                    RequestActualVendorPrice(mkmId.Value, card.NameEN, cardPrice, additionalLogText);
                                }
                            }
                            finally
                            {
                                cardPrice.IsNotifying = true;
                            }
                        }

                        cardPrice.UpdateUtc = DateTime.UtcNow;
                        _notificationCenter.FireNotification(
                            LogLevel.Debug,
                            string.Format("Downloaded price data for '{0}({1})': {2}/{3}/{4}", card.NameEN, card.SetCode, cardPrice.PriceLow, cardPrice.PriceAvg, cardPrice.PriceTrend) + additionalLogText);
                    }

                    if (!foundSet)
                    {
                        _notificationCenter.FireNotification(
                            LogLevel.Debug,
                            string.Format("Cannot find price data for '{0}({1})'. Request: {2}", card.NameEN, setDefinition.Name, result.Response.Root) + additionalLogText);
                    }

                    if (notifyOfPriceUpdate)
                    {
                        cardPrice.RaisePriceChanged();
                        _requestCounter.Save();
                    }
                }
            }
            catch (Exception error)
            {
                if (notifyOfPriceUpdate)
                {
                    _requestCounter.Save();
                }

                _notificationCenter.FireNotification(
                    LogLevel.Error,
                    string.Format("Error downloading price data for '{0}({1})': {2}", card.NameEN, card.SetCode, error.Message) + additionalLogText);
            }
        }

        public void PerformRequest(
            IEnumerable<MagicBinderCardViewModel> viewModels)
        {
            try
            {
                foreach (var viewModel in viewModels)
                {
                    // Thread.Sleep(1 * 1000);

                    PerformRequest(
                        viewModel.Definition,
                        viewModel.CardPrice,
                        false,
                        "");
                }

                _requestCounter.Save();

                ////    var grouped = viewModels.GroupBy(vm => vm.CardNameEN);
                ////var helper = new RequestHelper();

                ////foreach (var cardsForName in grouped)
                ////{
                ////    var card = cardsForName.First();
                ////    var result = helper.MakeRequest(RequestHelper.CreateGetProductsUrl(card.CardNameEN, MagicLanguage.English, true, null));

                ////    var setCodes = cardsForName.ToDictionary(c => c.Definition.SetCode);

                ////    var productNodes = result.Response.Root.Elements("product");
                ////    foreach (var productNode in productNodes)
                ////    {
                ////        var expansion = productNode.Element("expansion");
                ////        MagicSetDefinition definition;
                ////        if (!StaticMagicData.SetDefinitionsBySetName.TryGetValue(expansion.Value, out definition)
                ////            || !setCodes.ContainsKey(definition.Code))
                ////        {
                ////            continue;
                ////        }

                ////        var cardPrice = setCodes[definition.Code].CardPrice;
                ////        var priceGuide = productNode.Element("priceGuide");
                ////        if (priceGuide != null)
                ////        {
                ////            cardPrice.IsNotifying = false;
                ////            try
                ////            {
                ////                cardPrice.PriceLow = decimal.Parse(priceGuide.Element("LOW").Value, CultureInfo.InvariantCulture);
                ////                cardPrice.PriceAvg = decimal.Parse(priceGuide.Element("AVG").Value, CultureInfo.InvariantCulture);
                ////                cardPrice.PriceFoilLow = decimal.Parse(priceGuide.Element("LOWFOIL").Value, CultureInfo.InvariantCulture);
                ////                cardPrice.PriceTrend = decimal.Parse(priceGuide.Element("TREND").Value, CultureInfo.InvariantCulture);

                ////                cardPrice.ImagePath = productNode.Element("image").Value;
                ////                cardPrice.WebSite = productNode.Element("website").Value;
                ////            }
                ////            finally
                ////            {
                ////                cardPrice.IsNotifying = true;
                ////            }
                ////        }

                ////        cardPrice.UpdateUtc = DateTime.UtcNow;
                ////    }

                ////    var downloadedList = new List<string>();
                ////    var errorList = new List<string>();

                ////    foreach (var singleCard in cardsForName)
                ////    {
                ////        var definition = StaticMagicData.SetDefinitionsBySetCode[singleCard.Definition.SetCode];
                ////        if (!singleCard.CardPrice.UpdateUtc.HasValue)
                ////        {
                ////            errorList.Add(
                ////                string.Format("Cannot find price data for '{0}({1})'. Request: {2}", singleCard.CardNameEN, definition.Name, result.Response.Root));
                ////        }
                ////        else
                ////        {
                ////            downloadedList.Add(
                ////                string.Format(
                ////                        "Downloaded price data for '{0}({1})': {2}/{3}/{4}",
                ////                        singleCard.CardNameEN,
                ////                        definition.Name,
                ////                        singleCard.CardPrice.PriceLow,
                ////                        singleCard.CardPrice.PriceAvg,
                ////                        singleCard.CardPrice.PriceTrend));
                ////        }
                ////    }

                ////    if (downloadedList.Any())
                ////    {
                ////        _notificationCenter.FireNotification(
                ////            LogLevel.Debug,
                ////            string.Join(Environment.NewLine, downloadedList));
                ////    }

                ////    if (errorList.Any())
                ////    {
                ////        _notificationCenter.FireNotification(
                ////            LogLevel.Error,
                ////            string.Join(Environment.NewLine, errorList));
                ////    }
                ////}
            }
            catch (Exception error)
            {
                _notificationCenter.FireNotification(
                    LogLevel.Error,
                    string.Format("Error downloading price data: '{0}'", error.Message));
            }
        }

        private static void WorkaroundTimeout(INotificationCenter notificationCenter, string additionalLogText)
        {
            ////lock (_timeoutSync)
            ////{
            ////    _timeoutHelper += 1;
            ////    if (_timeoutHelper > 50)
            ////    {
            ////        _timeoutHelper = 0;

            ////        notificationCenter.FireNotification(
            ////            LogLevel.Debug,
            ////            "Waiting to get around MKM timeout " + additionalLogText);

            ////        // Warte kurz zwischen den Requests, damit wir nicht von
            ////        // MKM einen Timeout bekommen
            ////        Thread.Sleep(5 * 1000);
            ////    }
            ////}
        }

        private bool CheckRequestCount(
            bool saveRequestCounterFile,
            bool saveRequestCounterFileIfRequestFailed)
        {
            var result = _requestCounter.AddRequest();

            if (saveRequestCounterFile || (!result && saveRequestCounterFileIfRequestFailed))
            {
                _requestCounter.Save();
            }

            if (!result)
            {
                _notificationCenter.FireNotification(
                    LogLevel.Debug,
                    "Already reached maximum requests for today for MKM. Skipping request.");
            }

            return result;
        }

        private void RequestActualVendorPrice(
            string mkmId,
            string cardName,
            MagicCardPrice cardPrice,
            string additionalLogText)
        {
            if (string.IsNullOrWhiteSpace(mkmId))
            {
                return;
            }

            List<MkmSellerArticleData> allSellerData = new List<MkmSellerArticleData>();
            var maxLoops = 1;
            var maxExtraLoopsForMissingFoil = 2;

            var helper = new RequestHelper(_notificationCenter, additionalLogText);
            var startIndex = 1;

            do
            {
                _notificationCenter.FireNotification(
                   LogLevel.Debug,
                   string.Format("Requesting seller data for {0} ({2}) at index {1}... ", mkmId, startIndex, cardName) + additionalLogText);

                if (!CheckRequestCount(false, true))
                {
                    maxLoops = 0;
                    continue;
                }

                WorkaroundTimeout(_notificationCenter, additionalLogText);
                var resultSpecialLogic = helper.MakeRequest(RequestHelper.CreateGetArticlesUrl(
                     mkmId,
                     MagicLanguage.English,
                     true,
                     startIndex));

                var countries = new[] { "D", };
                var conditions = new[] { "NM", "M" };
                var languages = new[] { "German", "English" };

                var foundSellers = AnalyseMkmSellerRequestResult
                    .Analyse(
                        resultSpecialLogic.Response,
                        countries,
                        conditions,
                        languages)
                    .OrderBy(s => s.Price)
                    .ToList();

                allSellerData = allSellerData.Union(foundSellers).ToList();

                if (resultSpecialLogic.HttpResponse.StatusCode == System.Net.HttpStatusCode.PartialContent)
                {
                    startIndex += 100;
                    maxLoops -= 1;

                    if (maxExtraLoopsForMissingFoil == 0)
                    {
                        var foundFoilSeller = allSellerData.FirstOrDefault(s => s.IsFoil == true);
                        if (foundFoilSeller != null)
                        {
                            // Found a foil -> Abort
                            maxLoops = 0;
                        }
                    }

                    if (maxLoops == 0)
                    {
                        var foundFoilSeller = allSellerData.FirstOrDefault(s => s.IsFoil == true);
                        maxLoops = foundFoilSeller == null ? maxExtraLoopsForMissingFoil : 0;

                        maxExtraLoopsForMissingFoil = 0;
                    }
                }
                else
                {
                    maxLoops = 0;
                }
            }
            while (maxLoops > 0);

            var seller = allSellerData.FirstOrDefault(s => s.IsFoil == false);
            var sellerFoil = allSellerData.FirstOrDefault(s => s.IsFoil == true);

            cardPrice.CheapestPrice = seller != null ? seller.Price : (decimal?)null;
            cardPrice.CheapestPriceFoil = sellerFoil != null ? sellerFoil.Price : (decimal?)null;

            cardPrice.CheapestSeller = seller != null ? seller.SellerName : null;
            cardPrice.CheapestSellerFoil = sellerFoil != null ? sellerFoil.SellerName : null;

            _notificationCenter.FireNotification(
                LogLevel.Debug,
                string.Format(
                        "Downloading seller price data for '{0} ({6})' found {1} sellers. Cheapest = {2} ({3}), Foil = {4} ({5}) ",
                        mkmId,
                        allSellerData.Count(),
                        seller != null ? seller.SellerName : "<none>",
                        seller != null ? seller.Price : 0.00m,
                        sellerFoil != null ? sellerFoil.SellerName : "<none>",
                        sellerFoil != null ? sellerFoil.Price : 0.00m,
                        cardName) + additionalLogText);
        }
    }
}