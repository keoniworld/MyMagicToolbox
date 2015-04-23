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

		private decimal? _priceSell;

		private decimal? _priceFoilLow;

        private DateTime? _updateUtc;

        private string _webSite;

        private string _imagePath;

        private string _cheapestSeller;
        private string _cheapestSellerFoil;
        private decimal? _cheapestPrice;
        private decimal? _cheapestPriceFoil;

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
                _cheapestPrice= value;
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

			return UpdateUtc.HasValue
				&& UpdateUtc.Value.AddHours(hoursSinceLastCall) > DateTime.UtcNow;
        }

		public bool IsPriceUpOfToday()
		{
			return UpdateUtc.HasValue
				&& UpdateUtc.Value.Date >= DateTime.UtcNow.Date;
		}

	}
}