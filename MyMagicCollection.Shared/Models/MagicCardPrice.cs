using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minimod.NotificationObject;

namespace MyMagicCollection.Shared.Models
{
    public class MagicCardPrice : NotificationObject
    {
        private string _cardId;
        private decimal? _priceTrend;

        private decimal? _priceLow;

        private decimal? _priceAvg;

        private decimal? _priceFoilLow;

        private DateTime? _updateUtc;

        private string _webSite;

        private string _imagePath;

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
    }
}