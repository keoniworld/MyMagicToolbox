using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicDatabase;
using MagicLibrary;
using MyMagicCollection.Shared.Models;
using PropertyChanged;

namespace MyMagicCollection.Caliburn.Collection
{
    [ImplementPropertyChanged]
    public class CollectionCardViewModel : ICollectionCardViewModel
    {
        private readonly ICardDatabase _cardDatabase;
        private MagicCardDefinition _card;
        private readonly MagicCollectionItem _collectionItem;

        private CollectionCardViewModel(
            ICardDatabase cardDatabase,
            MagicCollectionItem collectionItem)
        {
            _cardDatabase = cardDatabase;
            _collectionItem = collectionItem;

            // TODO: Das später async on request laden
            _card = _cardDatabase.FindCardById(_collectionItem.CardId);
        }

        public string NameEN { get { return _card.NameEN; } }

        public string NameDE { get { return _card.NameDE; } }

        public int Quantity
        {
            // TODO: PropertyChanged
            get { return _collectionItem.Quantity; }
            set { _collectionItem.Quantity = value; }
        }
    }
}