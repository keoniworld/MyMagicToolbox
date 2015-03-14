using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minimod.NotificationObject;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.Shared.ViewModels
{
	public class BinderStatisticsPerSet
	{
		private readonly IEnumerable<MagicBinderCardViewModel> _cardsOfSet;

		public BinderStatisticsPerSet(
			MagicSetDefinition setDefinition,
			IEnumerable<MagicBinderCardViewModel> cardsOfSet)
		{
			SetDefinition = setDefinition;
			_cardsOfSet = cardsOfSet;

			foreach (var card in _cardsOfSet)
			{
				NumberOfCards += card.Quantity;

				switch (card.Definition.Rarity)
				{
					case MagicRarity.Mythic:
						Mythic += card.Quantity;
						MythicValue += card.Quantity * (card.Price.HasValue ? card.Price : 0m).Value;
						break;

					case MagicRarity.Rare:
						Rare += card.Quantity;
						RareValue += card.Quantity * (card.Price.HasValue ? card.Price : 0m).Value;
						break;

					case MagicRarity.Uncommon:
						Uncommon += card.Quantity;
						UncommonValue += card.Quantity * (card.Price.HasValue ? card.Price : 0m).Value;
						break;

					case MagicRarity.Common:
						Common += card.Quantity;
						CommonValue += card.Quantity * (card.Price.HasValue ? card.Price : 0m).Value;
						break;
				}
			}
		}

		public MagicSetDefinition SetDefinition { get; private set; }

		public int NumberOfCards { get; private set; }

		public int Mythic { get; private set; }

		public int Rare { get; private set; }

		public int Uncommon { get; private set; }

		public int Common { get; private set; }

		public decimal MythicValue { get; private set; }

		public decimal RareValue { get; private set; }

		public decimal UncommonValue { get; private set; }

		public decimal CommonValue { get; private set; }
	}
}