using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minimod.NotificationObject;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.VieModels;

namespace MyMagicCollection.Shared.ViewModels
{
    public class MagicBinderCardViewModel : NotificationObject, IMagicCardDefinition, IMagicBinderCardViewModel
    {
        private readonly MagicBinderCard _card;
        private readonly MagicCardPrice _price;
        private IMagicCardDefinition _definition;
        private decimal? _cardPrice;

        private IEnumerable<MagicCardDefinition> _reprints;

        public MagicBinderCardViewModel(
            IMagicCardDefinition definition,
            MagicBinderCard card)
        {
            _definition = definition;
            _card = card;
            _price = StaticPriceDatabase.FindPrice(_definition, false, false);
            _price.PropertyChanged += OnPricePropertyChanged;

            UpdatePrice();
        }

        ~MagicBinderCardViewModel()
        {
            _price.PropertyChanged -= OnPricePropertyChanged;
        }

        public IMagicCardDefinition Definition
        {
            get
            {
                return _definition;
            }

            set
            {
                _definition = value;
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

        public void UpdatePriceData(bool writeDatabase, bool async)
        {
            var action = new Action(() => StaticPriceDatabase.UpdatePrice(_definition, _price, writeDatabase));

            if (async)
            {
                Task.Factory.StartNew(action);
            }
            else
            {
                action();
            }
        }

        private void OnPricePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdatePrice();
        }

        private void UpdatePrice()
        {
            Price = IsFoil ? _price.PriceFoilLow : _price.PriceLow;
            RaisePropertyChanged(() => Price);
        }
    }
}