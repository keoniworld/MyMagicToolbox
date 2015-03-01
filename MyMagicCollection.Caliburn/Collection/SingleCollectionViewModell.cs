using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicDatabase;
using MagicLibrary;
using MyMagicCollection.Shared;
using PropertyChanged;

namespace MyMagicCollection.Caliburn.Collection
{
    [ImplementPropertyChanged]
    public class SingleCollectionViewModell : ISingleCollectionViewModel
    {
        private readonly IUserDatabase _userDatabase;
        private readonly ICardDatabase _cardDatabase;
        private IEnumerable<MagicCollectionItem> _collectionItems;
        private readonly MagicCollection _databaseCollection;
        private readonly INotificationCenter _notification;

        public SingleCollectionViewModell(
            INotificationCenter notification,
            IUserDatabase userDatabase,
            ICardDatabase cardDatabase,
            MagicCollection databaseCollection)
        {
            _userDatabase = userDatabase;
            _notification = notification;
            _cardDatabase = cardDatabase;
            _databaseCollection = databaseCollection;

            CollectionName = _databaseCollection != null ? _databaseCollection.Name : "";
        }

        public string CollectionName
        {
            get;
            set;
        }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName == "CollectionName")
            {
                var value = after as string;
                _databaseCollection.Name = value;
            }
        }

        public IEnumerable<CollectionCardViewModel> CollectionItems { get; private set; }

        public void SaveHeader()
        {
            _userDatabase.InsertOrUpdateCollection(_databaseCollection);
        }
      

        ////public void SetCollection(MagicCollection collection, IEnumerable<MagicCollectionItem> collectionItems)
        ////{
        ////    _collection = collection;
        ////    _collectionItems = collectionItems;
        ////}
    }
}