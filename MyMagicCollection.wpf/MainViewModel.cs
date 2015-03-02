using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Minimod.NotificationObject;
using MyMagicCollection.Shared;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.ViewModels;
using MyMagicCollection.wpf.DataSource;
using MyMagicCollection.wpf.Settings;
using MyMagicCollection.wpf.ViewModels;
using SettingsProviderNet;

namespace MyMagicCollection.wpf
{
    public class MainViewModel : NotificationObject
    {
        private readonly INotificationCenter _notificationCenter;
        private readonly SettingsData _settings;
        private string _statusBarText;

        private MagicBinderViewModel _activeBinder;
         
        private IMagicDataSource _currentDataSource;

        private IEnumerable<FoundMagicCardViewModel> _searchResult;

        private FoundMagicCardViewModel _selectedCard;

        public MainViewModel(INotificationCenter notificationCenter)
        {
            _notificationCenter = notificationCenter;
            CardLookup = new CardLookup();

            DatabaseSummary = string.Format(
                CultureInfo.CurrentUICulture,
                "{0} cards from {1} sets",
                StaticMagicData.CardDefinitions.Count(),
                StaticMagicData.SetDefinitions.Count());

            notificationCenter.NotificationFired += (sender, e) =>
              {
                  StatusBarText = e.Message;
              };

            // Load the settings
            var provider = new SettingsProvider(new LocalAppDataStorage("AppSettings"));
            _settings = provider.GetSettings<SettingsData>();

            _currentDataSource = new StaticMagicDataDataSource();

            if (!string.IsNullOrEmpty(_settings.LoadedBinder))
            {
                LoadBinder(_settings.LoadedBinder);
            }
        }

        public CardLookup CardLookup { get; private set; }

        public string DatabaseSummary { get; set; }

        public string StatusBarText
        {
            get
            {
                return _statusBarText;
            }
            set
            {
                _statusBarText = value;
                RaisePropertyChanged(() => StatusBarText);
            }
        }

        public bool IsActiveBinder { get { return _activeBinder != null; } }

        public MagicBinderViewModel ActiveBinder
        {
            get { return _activeBinder; }
        }

        public IEnumerable<FoundMagicCardViewModel> CardCollection
        {
            get
            {
                return _searchResult;
            }

            set
            {
                _searchResult = value;
                RaisePropertyChanged(() => CardCollection);
            }
        }

        public FoundMagicCardViewModel SelectedCard
        {
            get
            {
                return _selectedCard;
            }

            set
            {
                _selectedCard = value;
                RaisePropertyChanged(() => SelectedCard);
            }
        }

        public void SaveSettings()
        {
            var provider = new SettingsProvider(new LocalAppDataStorage("AppSettings"));
            provider.SaveSettings(_settings);
        }

        /// <summary>
        /// This method starts the card search using the current search options bound to
        /// <see cref="CardLookup"/>.
        /// </summary>
        public void LookupCards()
        {
            if (_currentDataSource == null)
            {
                _notificationCenter.FireNotification(null, "Cannot search cards");
            }

            var stopwatch = Stopwatch.StartNew();
            _notificationCenter.FireNotification(null, "Performing card search...");

            var found = _currentDataSource.Lookup(CardLookup);

            stopwatch.Stop();
            _notificationCenter.FireNotification(null, "Card search returned " + found.Count() + " and took " + stopwatch.Elapsed);
            CardCollection = found;
        }

        public void CreateAndSetNewBinder(string fileName)
        {
            var name = new FileInfo(fileName);
            _activeBinder = new MagicBinderViewModel(name.Name.Substring(0, name.Name.Length - name.Extension.Length));
            _activeBinder.WriteFile(fileName);
            _settings.LoadedBinder = fileName;
            SaveSettings();

            RaisePropertyChanged(() => IsActiveBinder);
            RaisePropertyChanged(() => ActiveBinder);

            _notificationCenter.FireNotification(null, string.Format("Created binder '{0}'.", _activeBinder.Name));
        }

        public void LoadBinder(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            if (!File.Exists(_settings.LoadedBinder))
            {
                _notificationCenter.FireNotification(null, string.Format("Binder file '{0}' does not exist.", fileInfo.Name));
                return;
            }

            _activeBinder = new MagicBinderViewModel();
            _activeBinder.ReadFile(fileName);
            _settings.LoadedBinder = fileName;
            SaveSettings();

            RaisePropertyChanged(() => IsActiveBinder);
            RaisePropertyChanged(() => ActiveBinder);

            _notificationCenter.FireNotification(null, string.Format("Loaded binder '{0}'.", _activeBinder.Name));
        }
    }
}