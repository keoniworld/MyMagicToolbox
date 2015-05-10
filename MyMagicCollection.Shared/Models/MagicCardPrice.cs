﻿using Minimod.NotificationObject;
using System;
using System.Globalization;

namespace MyMagicCollection.Shared.Models
{
    public class MagicCardPrice : NotificationObject
    {
        private string _cardId;
        private decimal? _priceTrend;

        private decimal? _priceLow;

        private decimal? _priceAvg;

        private decimal? _priceSell;

        private decimal? _priceFoilLow;

        private DateTime? _updateUtc;

        private string _webSite;

        private string _imagePath;

        private string _cheapestSeller;
        private string _cheapestSellerFoil;
        private decimal? _cheapestPrice;
        private decimal? _cheapestPriceFoil;

        private string _mkmId;

        public event EventHandler<EventArgs> PriceChanged;

        public void RaisePriceChanged()
        {
            var price = PriceChanged;
            if (price != null)
            {
                price(this, EventArgs.Empty);
            }
        }

        public string CardId
        {
            get
            {
                return _cardId;
            }
            set
            {
                _cardId = value;
                RaisePropertyChanged(() => CardId);
            }
        }

        public string MkmId
        {
            get
            {
                return _mkmId;
            }
            set
            {
                _mkmId = value;
                RaisePropertyChanged(() => MkmId);
            }
        }

        public decimal? PriceTrend
        {
            get
            {
                return _priceTrend;
            }
            set
            {
                _priceTrend = value;
                RaisePropertyChanged(() => PriceTrend);
            }
        }

        public decimal? PriceSell
        {
            get
            {
                return _priceSell;
            }
            set
            {
                _priceSell = value;
                RaisePropertyChanged(() => PriceSell);
            }
        }

        public decimal? PriceLow
        {
            get
            {
                return _priceLow;
            }
            set
            {
                _priceLow = value;
                RaisePropertyChanged(() => PriceLow);
            }
        }

        public decimal? PriceAvg
        {
            get
            {
                return _priceAvg;
            }
            set
            {
                _priceAvg = value;
                RaisePropertyChanged(() => PriceAvg);
            }
        }

        public decimal? PriceFoilLow
        {
            get
            {
                return _priceFoilLow;
            }
            set
            {
                _priceFoilLow = value;
                RaisePropertyChanged(() => PriceFoilLow);
            }
        }

        public DateTime? UpdateUtc
        {
            get
            {
                return _updateUtc;
            }
            set
            {
                _updateUtc = value;
                RaisePropertyChanged(() => UpdateUtc);
            }
        }

        public string WebSite
        {
            get
            {
                return _webSite;
            }
            set
            {
                _webSite = value;
                RaisePropertyChanged(() => WebSite);
            }
        }

        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                _imagePath = value;
                RaisePropertyChanged(() => ImagePath);
            }
        }

        public string CheapestSeller
        {
            get
            {
                return _cheapestSeller;
            }
            set
            {
                _cheapestSeller = value;
                RaisePropertyChanged(() => CheapestSeller);
            }
        }

        public string CheapestSellerFoil
        {
            get
            {
                return _cheapestSellerFoil;
            }
            set
            {
                _cheapestSellerFoil = value;
                RaisePropertyChanged(() => CheapestSellerFoil);
            }
        }

        public decimal? CheapestPrice
        {
            get
            {
                return _cheapestPrice;
            }
            set
            {
                _cheapestPrice = value;
                RaisePropertyChanged(() => CheapestPrice);
            }
        }

        public decimal? CheapestPriceFoil
        {
            get
            {
                return _cheapestPriceFoil;
            }
            set
            {
                _cheapestPriceFoil = value;
                RaisePropertyChanged(() => CheapestPriceFoil);
            }
        }

        public bool IsPriceUpToDate()
        {
            const int hoursSinceLastCall = 7 * 24;

            return CheapestPrice.HasValue
                && UpdateUtc.HasValue
                && UpdateUtc.Value.AddHours(hoursSinceLastCall) > DateTime.UtcNow;
        }

        public bool IsPriceUpOfToday()
        {
            return UpdateUtc.HasValue
                && UpdateUtc.Value.Date >= DateTime.UtcNow.Date;
        }

        public void BuildDefaultMkmImagePath(IMagicCardDefinition definition)
        {
            if (string.IsNullOrEmpty(_imagePath))
            {
                var setDefinition = StaticMagicData.SetDefinitionsBySetCode[definition.SetCode];

                var setName = setDefinition.Name;
                var anthology = setName.IndexOf("Duel Decks Anthology", 0, StringComparison.InvariantCultureIgnoreCase);
                if (anthology == 0)
                {
                    setName = setName.Substring(0, "Duel Decks Anthology".Length);
                }


                _imagePath = string.Format(
                    CultureInfo.InvariantCulture,
                    "./img/cards/{0}/{1}.jpg",
                    setName
                        .Replace(" ", "_")
                        .Replace(":", "")
                        .Replace("-", "_")
                        .Replace(",", "")
                        .Replace("'", ""),
                    definition.NameMkm
                        .Replace("Æ", "ae")
                        .Replace("û", "u")
                        .Replace("â", "a")
                        .Replace("á", "a")
                        .Replace("'", "")                        
                        .Replace(".", "")
                        .Replace(":", "")
                        .Replace(" // ", "_")
                        .ToLowerInvariant()
                        .Replace(", planeswalker", "")
                        .Replace(" (version 1)", "1")
                        .Replace(" (version 2)", "2")
                        .Replace(" (version 3)", "3")
                        .Replace(" (version 4)", "4")
                        .Replace(" (version 5)", "5")
                        .Replace(" (version 6)", "6")
                        .Replace(" (version 7)", "7")
                        .Replace(" (version 8)", "8")
                        .Replace(" (version 9)", "9")
                        .Replace(" ", "_")
                        .Replace("-", "_")                        
                        .Replace(",", ""));
            }
        }
    }
}