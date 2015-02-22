using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using MagicLibrary;
using MagicDatabase;
using MagicLibrary;
using PropertyChanged;

namespace MyMagicCollection.Caliburn
{
    [Export]
    [ImplementPropertyChanged]
    public sealed class CardDatabaseViewModel
    {
        private readonly ICardDatabase _cardDatabase;
        private readonly INotificationCenter _notificationCenter;

        [ImportingConstructor]
        public CardDatabaseViewModel(
            CardDatabaseSearchViewModel searchViewModel,
            ICardDatabase cardDatabase,
            INotificationCenter notificationCenter)
        {
            _notificationCenter = notificationCenter;
            _cardDatabase = cardDatabase;

            CardDatabaseSearchViewModel = searchViewModel;

            CardDatabaseSearchViewModel.SearchTriggered += (sender, e) =>
                {
                    _notificationCenter.FireNotification("", "Searching for cards...");
                    var found = _cardDatabase.FindCards(sender as ICardSearchModel)
                        .Select(c => new CardViewModel(c))
                        .ToArray();

                    SelectedCards = found;
                    _notificationCenter.FireNotification("", "Found " + found.Count() + " cards.");
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