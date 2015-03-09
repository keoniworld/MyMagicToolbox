using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minimod.NotificationObject;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.Shared.ViewModels
{
    public class MagicBinderCardViewModel : NotificationObject
    {
        private readonly MagicCardDefinition _definition;
        private readonly MagicBinderCard _card;
        private readonly MagicCardPrice _price;

        private decimal? _cardPrice;

        public MagicBinderCardViewModel(
            MagicCardDefinition definition,
            MagicBinderCard card)
        {
            _definition = definition;
            _card = card;
            _price = StaticPriceDatabase.FindPrice(_definition, false, false);
            _price.PropertyChanged += (sender, e) =>
            {
                UpdatePrice();
            };

            UpdatePrice();
        }

        public MagicCardDefinition Definition => _definition;

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
            }
        }

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

        public MagicCardPrice CardPrice
        {
            get
            {
                return _price;
            }
        }
        public string CardNameEN => _definition.NameEN;

        public string CardNameDE => _definition.NameDE;

        public string RowId => _card.RowId;

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

        private void UpdatePrice()
        {
            Price = IsFoil ? _price.PriceFoilLow : _price.PriceLow;
        }
    }
}