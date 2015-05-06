using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minimod.NotificationObject;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.VieModels;
using NLog;

namespace MyMagicCollection.Shared.ViewModels
{
    public class MagicBinderCardViewModel : NotificationObject, IMagicCardDefinition, IMagicBinderCardViewModel
    {
        private readonly MagicBinderCard _card;
        private MagicCardPrice _price;
        private IMagicCardDefinition _definition;
        private decimal? _cardPrice;

        private IEnumerable<MagicCardDefinition> _reprints;

        private INotificationCenter _notificationCenter;

        public MagicBinderCardViewModel(
            IMagicCardDefinition definition,
            MagicBinderCard card)
        {
            _notificationCenter = NotificationCenter.Instance;
            _definition = definition;
            _card = card;
            _price = StaticPriceDatabase.FindPrice(_definition, false, false, "", false);
            _price.PriceChanged += OnPricePriceChanged;

            UpdatePrice();
        }

        ~MagicBinderCardViewModel()
        {
            _price.PriceChanged -= OnPricePriceChanged;
        }

        public event EventHandler<EventArgs> PriceChanged;

        public IMagicCardDefinition Definition
        {
            get
            {
                return _definition;
            }

            set
            {
                _definition = value;
                if (value != null)
                {
                    _card.CardId = value.CardId;
                    _price.PriceChanged-= OnPricePriceChanged;
                    _price = StaticPriceDatabase.FindPrice(_definition, true, true, "", false);
                    _price.PriceChanged += OnPricePriceChanged;
                    RaisePropertyChanged(() => CardPrice);
                }

                RaisePropertyChanged(() => Definition);
                UpdatePrice();
            }
        }

        public string CardId
        {
            get
            {
                return _card.CardId;
            }

            set
            {
                _card.CardId = value;
                RaisePropertyChanged(() => CardId);

                UpdatePrice();
            }
        }

        public string NameEN => _definition?.NameEN;

        public string NameDE => _definition?.NameDE;

        public string NameMkm => _definition?.NameMkm;
        public string DisplayNameEn => _definition?.DisplayNameEn;

        public string RulesText => _definition?.RulesText;

        public string RulesTextDE => _definition?.RulesTextDE;

        public MagicRarity? Rarity => _definition?.Rarity;

        public string ManaCost => _definition?.ManaCost;

        public int? ConvertedManaCost => _definition?.ConvertedManaCost;

        public int Quantity
        {
            get
            {
                return _card.Quantity;
            }

            set
            {
                _card.Quantity = value;
                RaisePropertyChanged(() => Quantity);
            }
        }

        public int QuantityTrade
        {
            get
            {
                return _card.QuantityTrade;
            }

            set
            {
                _card.QuantityTrade = value;
                RaisePropertyChanged(() => QuantityTrade);
            }
        }

        public int QuantityWanted
        {
            get
            {
                return _card.QuantityWanted;
            }

            set
            {
                _card.QuantityWanted = value;
                RaisePropertyChanged(() => QuantityWanted);
            }
        }

        public string Comment
        {
            get
            {
                return _card.Comment;
            }

            set
            {
                _card.Comment = value;
                RaisePropertyChanged(() => Comment);
            }
        }

        public bool IsFoil
        {
            get
            {
                return _card.IsFoil;
            }

            set
            {
                _card.IsFoil = value;
                RaisePropertyChanged(() => IsFoil);

                UpdatePrice();
            }
        }

        public MagicLanguage? Language
        {
            get
            {
                return _card.Language;
            }

            set
            {
                _card.Language = value;
                RaisePropertyChanged(() => Language);
            }
        }

        public MagicCardType MagicCardType => _definition != null ? _definition.MagicCardType : MagicCardType.Unknown;

        public MagicGrade? Grade
        {
            get
            {
                return _card.Grade;
            }

            set
            {
                _card.Grade = value;
                RaisePropertyChanged(() => Grade);
            }
        }

        public decimal? Price
        {
            get
            {
                return _cardPrice;
            }

            set
            {
                _cardPrice = value;
                RaisePropertyChanged(() => Price);
            }
        }

        public MagicCardPrice CardPrice => _price;

        public string CardNameEN => _definition.NameEN;

        public string CardNameDE => _definition.NameDE;

        public string RowId => _card.RowId;

        public IEnumerable<MagicCardDefinition> Reprints
        {
            get
            {
                if (_reprints == null)
                {
                    _reprints = StaticMagicData.CardDefinitions
                        .Where(c => c.NameEN == _definition.NameEN)
                        .ToArray();
                }

                return _reprints;
            }
        }

        public string SetCode => _definition.SetCode;

        public void UpdatePriceData(bool writeDatabase, bool async, string additionalLogText, bool forcePriceUpdate)
        {
            var action = new Action(() =>
                                    {
                                        var update = !_price.IsPriceUpToDate();
                                        if (forcePriceUpdate || update)
                                        {
                                            StaticPriceDatabase.UpdatePrice(_definition, _price, writeDatabase, additionalLogText, forcePriceUpdate);
                                            UpdatePrice();
                                        }
                                        else
                                        {
                                            _notificationCenter.FireNotification(
                                                LogLevel.Debug,
                                                "Skipping price update for " + _definition.DisplayNameEn + " " + additionalLogText);
                                        }
                                    });

            if (async)
            {
                Task.Factory.StartNew(action);
            }
            else
            {
                action();
            }
        }

        private void OnPricePriceChanged(object sender, EventArgs e)
        {
            UpdatePrice();
        }

        private void UpdatePrice()
        {
            Price = IsFoil
                ? _price.CheapestPriceFoil.HasValue ? _price.CheapestPriceFoil.Value : _price.PriceFoilLow
                : _price.CheapestPrice.HasValue ? _price.CheapestPrice.Value : _price.PriceLow;

            RaisePropertyChanged(() => Price);

            var price = PriceChanged;
            if (price != null)
            {
                price(this, EventArgs.Empty);
            }
        }
    }
}