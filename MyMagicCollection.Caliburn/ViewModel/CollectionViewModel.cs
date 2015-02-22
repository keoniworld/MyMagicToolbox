using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using MagicDatabase;
using MagicLibrary;
using MyMagicCollection.Caliburn.Collection;
using PropertyChanged;

namespace MyMagicCollection.Caliburn
{
    [Export]
    [Export(typeof(ICurrentCollectionProvider))]
    public class CollectionViewModel : PropertyChangedBase, ICurrentCollectionProvider
    {
        private readonly IUserDatabase _userDatabase;
        private readonly ICardDatabase _cardDatabase;
        private readonly IApplicationSettings _applicationSettings;
        private readonly INotificationCenter _notification;

        private ISingleCollectionViewModel _selectedCollection;

        private IEnumerable<ISingleCollectionViewModel> _collections;

        [ImportingConstructor]
        public CollectionViewModel(
            INotificationCenter notification,
            IUserDatabase userDatabase,
            ICardDatabase cardDatabase,
            IApplicationSettings applicationSettings)
        {
            _notification = notification;
            _userDatabase = userDatabase;
            _cardDatabase = cardDatabase;
            _applicationSettings = applicationSettings;
            Collections = _userDatabase
                .GetAllCollections()
                .Select(c => new SingleCollectionViewModell(notification, _userDatabase, _cardDatabase, c));

            ISingleCollectionViewModel loadedCollection = null;
            Task.Factory.StartNew(() =>
            {
                var currentCollectionName = _applicationSettings.GetCurrentCollection();
                var found = Collections.FirstOrDefault(c => c.CollectionName == currentCollectionName);
                loadedCollection = LoadCollection(found);
            }).ContinueWith((task) =>
            {
                SelectedCollection = loadedCollection;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Occurs when collection loading has been started
        /// </summary>
        public event EventHandler<EventArgs> CollectionLoading;

        /// <summary>
        /// Occurs when a collection has been fully loaded
        /// </summary>
        public event EventHandler<EventArgs> CollectionLoaded;

        public ISingleCollectionViewModel SelectedCollection
        {
            get
            {
                return _selectedCollection;
            }
            set
            {
                _selectedCollection = value;
                _applicationSettings.SetCurrentCollection(value != null ? value.CollectionName : "");
                NotifyOfPropertyChange();
                NotifyOfPropertyChange("SelectedCollectionName");
            }
        }

        public string SelectedCollectionName
        {
            get
            {
                return _selectedCollection != null ? _selectedCollection.CollectionName : "";
            }

            set
            {
                if (_selectedCollection == null)
                {
                    return;
                }

                _selectedCollection.CollectionName = value;
                _applicationSettings.SetCurrentCollection(value);
                _selectedCollection.SaveHeader();
                NotifyOfPropertyChange();                
            }
        }

        public IEnumerable<ISingleCollectionViewModel> Collections
        {
            get
            {
                return _collections;
            }
            private set
            {
                _collections = value;
                NotifyOfPropertyChange();
            }
        }

        public ISingleCollectionViewModel LoadCollection(ISingleCollectionViewModel collection)
        {
            try
            {
                if (collection == null)
                {
                    return null;
                }

                _notification.FireNotification("", "Loading collection " + collection.CollectionName);

                var loading = CollectionLoading;
                if (loading != null)
                {
                    loading(this, EventArgs.Empty);
                }

                // TODO: Load the cards here and merge from card database
                
            }
            catch (Exception)
            {
            }
            finally
            {
                var loading = CollectionLoaded;
                if (loading != null)
                {
                    loading(this, EventArgs.Empty);
                }

                _notification.FireNotification("", "Finished loading collection " + collection.CollectionName);
            }

            return collection; 
        }

        public void CreateNewCollection()
        {
            var collection = new MagicCollection()
            {
                Name = "new Collection"
            };

            _userDatabase.InsertOrUpdateCollection(collection);
            var viewModel = new SingleCollectionViewModell(
                _notification,
                _userDatabase,
                _cardDatabase,
                collection);

            var listCollection = Collections.ToList();

            listCollection.Add(viewModel);
            Collections = listCollection;
            SelectedCollection = viewModel;
        }
    }
}