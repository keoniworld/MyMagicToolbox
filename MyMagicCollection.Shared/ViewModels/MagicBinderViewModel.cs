using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Minimod.NotificationObject;
using MyMagicCollection.Shared.FileFormats.MyMagicCollection;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.Shared.ViewModels
{
    public class MagicBinderViewModel : NotificationObject
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

            foreach (var card in _cards)
            {
                card.PropertyChanged += Card_PropertyChanged;
            }

            _sortedCards = Cards.ToDictionary(c => c.RowId);
            _fileName = fileName;

            CalculateTotals();
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
            bool isFoil,
            bool updateTotals)
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

            viewModel.PropertyChanged += Card_PropertyChanged;

            if (updateTotals)
            {
                CalculateTotals();
            }
        }

        public void AddCards(
            IEnumerable<FoundMagicCardViewModel> cards,
            MagicGrade grade,
            MagicLanguage language,
            bool isFoil)
        {
            foreach (var card in cards)
            {
                AddCard(
                    card.Definition,
                    card.Quantity.HasValue ? card.Quantity.Value : 0,
                    grade,
                    language,
                    isFoil,
                    false);
            }

            CalculateTotals();
        }

        public int TotalNumberOfCards { get; private set; }
        public int TotalNumberOfTradeCards { get; private set; }

        private void Card_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CalculateTotals();
            WriteFile();
        }

        private void CalculateTotals()
        {
            TotalNumberOfCards = 0;
            TotalNumberOfTradeCards = 0;
            foreach (var card in _cards)
            {
                TotalNumberOfCards += card.Quantity;
                TotalNumberOfTradeCards += card.QuantityTrade;
            }
            
            RaisePropertyChanged(() => TotalNumberOfCards);
            RaisePropertyChanged(() => TotalNumberOfTradeCards);
        }

        // TODO: Modifikationsoperationen
    }
}