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

        public MagicBinderCardViewModel(
			MagicCardDefinition definition,
			MagicBinderCard card)
        {
			_definition = definition;
			_card = card;
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

		public string CardNameEN => _definition.NameEN;
		public string CardNameDE => _definition.NameDE;

        public string RowId => _card.RowId;
    }
}
