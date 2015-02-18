using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using MagicDatabase;
using MagicFileContracts;
using PropertyChanged;

namespace MyMagicCollection.Caliburn
{
    [Export]
    [ImplementPropertyChanged]
    public class CardDatabaseViewModel
    {
        private readonly ICardDatabase _cardDatabase;

        [ImportingConstructor]
        public CardDatabaseViewModel(
            CardDatabaseSearchViewModel searchViewModel,
            ICardDatabase cardDatabase)
        {
            _cardDatabase = cardDatabase;

            CardDatabaseSearchViewModel = searchViewModel;

            CardDatabaseSearchViewModel.SearchTriggered += (sender, e) =>
                {
                    var found = _cardDatabase.FindCards(sender as ICardSearchModel)
                        .Select(c => new CardViewModel(c))
                        .ToArray();

                    SelectedCards = found;
                };
        }

        
        public CardViewModel SelectedCard
        {
            get;
            set;
        }

        public IEnumerable<CardViewModel> SelectedCards { get; set; }

        public CardDatabaseSearchViewModel CardDatabaseSearchViewModel { get; private set; }
    }
}