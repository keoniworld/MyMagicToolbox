using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MyMagicCollection.Shared.FileFormats.MyMagicCollection;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.Shared.ViewModels
{
	public class MagicBinderViewModel
	{
		private MagicBinder _magicCollection;
		private string _fileName;

		private IDictionary<string, MagicBinderCardViewModel> _sortedCards;
		private ObservableCollection<MagicBinderCardViewModel> _cards;

		public MagicBinderViewModel()
			: this(null)
		{
		}

		public MagicBinderViewModel(string name)
		{
			_cards = new ObservableCollection<MagicBinderCardViewModel>();
			_sortedCards = new Dictionary<string, MagicBinderCardViewModel>();
			if (!string.IsNullOrEmpty(name))
			{
				_magicCollection = new MagicBinder
				{
					Name = name,
				};
			}
		}

		public IEnumerable<MagicBinderCardViewModel> Cards => _cards;

		public string Name => _magicCollection.Name;

		public void ReadFile(string fileName)
		{
			var loader = new MyMagicCollectionCsv();
			_magicCollection = loader.ReadFile(fileName);

			// Now wrap every card with a view model:
			_cards = new ObservableCollection<MagicBinderCardViewModel>(_magicCollection.Cards
				.Select(c => new MagicBinderCardViewModel(StaticMagicData.CardDefinitionsByCardId[c.CardId], c)));

			_sortedCards = Cards.ToDictionary(c => c.RowId);
			_fileName = fileName;
		}

		public void WriteFile()
		{
			WriteFile(_fileName);
		}

		public void WriteFile(string fileName)
		{
			if (_magicCollection == null)
			{
				return;
			}

			var loader = new MyMagicCollectionCsv();
			loader.WriteFile(fileName, _magicCollection);
			_fileName = fileName;
		}

		public void AddCard(
			MagicCardDefinition cardDefinition,
			int quantity,
			MagicGrade grade,
			MagicLanguage language,
			bool isFoil)
		{
			var binderCard = new MagicBinderCard()
			{
				CardId = cardDefinition.CardId,
				Grade = grade,
				Language = language,
				IsFoil = isFoil,
				Quantity = quantity
			};

			var viewModel = new MagicBinderCardViewModel(cardDefinition, binderCard);
			_sortedCards.Add(binderCard.RowId, viewModel);
			_cards.Add(viewModel);
			_magicCollection.Cards.Add(binderCard);
		}

		// TODO: Modifikationsoperationen
	}
}