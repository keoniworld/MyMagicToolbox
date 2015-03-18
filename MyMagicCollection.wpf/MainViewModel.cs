using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Minimod.NotificationObject;
using MoreLinq;
using MyMagicCollection.Shared;
using MyMagicCollection.Shared.DataSource;
using MyMagicCollection.Shared.FileFormats;
using MyMagicCollection.Shared.FileFormats.DeckBoxCsv;
using MyMagicCollection.Shared.Helper;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.VieModels;
using MyMagicCollection.Shared.ViewModels;
using MyMagicCollection.wpf.Settings;
using NLog;
using SettingsProviderNet;

namespace MyMagicCollection.wpf
{
    public class MainViewModel : NotificationObject
    {
        private readonly INotificationCenter _notificationCenter;
        private readonly SettingsData _settings;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private string _statusBarText;

        private MagicBinderViewModel _activeBinder;

        private IMagicDataSource _currentDataSource;

        private IEnumerable<FoundMagicCardViewModel> _searchResult;

        private FoundMagicCardViewModel _selectedCard;

        private bool _canAddSelected;

        private bool _selectedCardIsFoil;

        private IEnumerable<MagicBinderCardViewModel> _selectedCardsInBinder;

        private LookupSource _lookupSource;

        private LogLevel _maxLevel = LogLevel.Debug;

        private MagicBinderCardViewModel _selectedCardInBinder;

        public MainViewModel(INotificationCenter notificationCenter)
        {
            _logger.Log(LogLevel.Info, "============================= NEW APP START ============================= ");
            _notificationCenter = notificationCenter;
            CardLookup = new CardLookup();
            CardLookup.PropertyChanged += (sender, e) =>
            {
                if (CardLookup.SearchAsYouType)
                {
                    LookupCards();
                }
            };

            DatabaseSummary = string.Format(
                CultureInfo.CurrentUICulture,
                "{0} cards from {1} sets",
                StaticMagicData.CardDefinitions.Count(),
                StaticMagicData.SetDefinitions.Count());

            notificationCenter.NotificationFired += (sender, e) =>
              {
                  _logger.Log(e.LogLevel, e.Message);

                  if (e.LogLevel >= _maxLevel)
                  {
                      StatusBarText = e.Message;
                  }
              };

            // Load the settings
            var provider = new SettingsProvider(new LocalAppDataStorage("AppSettings"));
            _settings = provider.GetSettings<SettingsData>();
            _maxLevel = LogLevel.FromString(_settings.LogLevel);

            _currentDataSource = new StaticMagicDataDataSource();

            if (!string.IsNullOrEmpty(_settings.LoadedBinder))
            {
                LoadBinder(new DirectoryInfo(PathHelper.UserDataFolder).MakeAbsolutePath(_settings.LoadedBinder));
            }

            // TODO: Better logic for this
            ////Task.Factory.StartNew(() =>
            ////{
            ////    var downloader = new SymbolDownload();
            ////    downloader.Download(PathHelper.SymbolCacheFolder);

            ////    var setDownload = new SetDownload(_notificationCenter);
            ////    setDownload.Download(PathHelper.SetCacheFolder, StaticMagicData.SetDefinitions);
            ////});
        }

        public static IEnumerable<MagicLanguage> AvailableLanguages { get; } = (IEnumerable<MagicLanguage>)Enum.GetValues(typeof(MagicLanguage));

        public static IEnumerable<MagicGrade> AvailableGrades { get; } = (IEnumerable<MagicGrade>)Enum.GetValues(typeof(MagicGrade));

        public CardLookup CardLookup { get; private set; }

        public string DatabaseSummary { get; set; }

        public IEnumerable<MagicBinderCardViewModel> CardsInBinder => _activeBinder?.Cards;

        public IEnumerable<MagicBinderCardViewModel> SelectedCardsInBinder => _selectedCardsInBinder;

        public MagicBinderCardViewModel SelectedCardInBinder
        {
            get
            {
                return _selectedCardInBinder;
            }

            set
            {
                _selectedCardInBinder = value;
                RaisePropertyChanged(() => SelectedCardInBinder);
            }
        }

        public MagicLanguage SelectedLanguage
        {
            get
            {
                return _settings.SelectedLanguage;
            }

            set
            {
                _settings.SelectedLanguage = value;
                RaisePropertyChanged(() => SelectedLanguage);
                SaveSettings();
            }
        }

        public bool SelectedCardIsFoil
        {
            get
            {
                return _selectedCardIsFoil;
            }

            set
            {
                _selectedCardIsFoil = value;
                RaisePropertyChanged(() => SelectedCardIsFoil);
            }
        }

        public MagicGrade SelectedGrade
        {
            get
            {
                return _settings.SelectedGrade;
            }

            set
            {
                _settings.SelectedGrade = value;
                SaveSettings();
                RaisePropertyChanged(() => SelectedGrade);
            }
        }

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

        public bool CanAddSelected
        {
            get
            {
                return _canAddSelected;
            }
            private set
            {
                _canAddSelected = value;
                RaisePropertyChanged(() => CanAddSelected);
            }
        }

        public bool IsActiveBinder => _activeBinder != null;

        public MagicBinderViewModel ActiveBinder => _activeBinder;

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
                CanAddSelected = value != null;
                RaisePropertyChanged(() => SelectedCard);

                if (_selectedCard != null)
                {
                    _selectedCard.UpdatePriceData(true);
                }

                UpdateSelectedCardFromBinder();
            }
        }

        public LookupSource LookupSource
        {
            get
            {
                return _lookupSource;
            }

            set
            {
                _lookupSource = value;
                switch (_lookupSource)
                {
                    case LookupSource.ActiveBinder:
                        _currentDataSource = new ActiveBinderDataSource(_activeBinder);
                        CardLookup.SetSource = new CardLookupSetSourceActiveBinder(_activeBinder);
                        break;

                    case LookupSource.CardDatabase:
                    default:
                        _currentDataSource = new StaticMagicDataDataSource();
                        CardLookup.SetSource = new CardLookupSetSourceAllSets();
                        break;
                }

                RaisePropertyChanged(() => LookupSource);
            }
        }

        public void ExportActiveBinder(string fileName)
        {
            InternalExport(fileName, _activeBinder.Cards, new Func<IMagicBinderCardViewModel, int>((card) => card.Quantity));
        }

        public void ExportActiveBinderTrade(string fileName)
        {
            InternalExport(fileName, _activeBinder.Cards.Where(c => c.QuantityTrade > 0), new Func<IMagicBinderCardViewModel, int>((card) => card.QuantityTrade));
        }

        public void ExportActiveBinderWants(string fileName)
        {
            InternalExport(fileName, _activeBinder.Cards.Where(c => c.QuantityWanted > 0), new Func<IMagicBinderCardViewModel, int>((card) => card.QuantityWanted));
        }

        public void ImportCards(string fileName)
        {
            var info = new FileInfo(fileName);
            _notificationCenter.FireNotification(null, string.Format("Loading '{0}'...", info.Name));
            var watch = Stopwatch.StartNew();

            var content = File.ReadAllText(fileName);

            var detect = new DetectFileFormat(_notificationCenter);
            var reader = detect.Detect(fileName, content);
            var found = reader.ReadFileContent(content)
                .AsParallel()
                .Select(c => new FoundMagicCardViewModel(c))
                .ToList();

            watch.Stop();
            _notificationCenter.FireNotification(null, string.Format("Loaded '{0}' in {1}", info.Name, watch.Elapsed));
            CardCollection = found;
        }

        public void ExportDisplayedCardsList(string fileName)
        {
            InternalExport(fileName, _searchResult, new Func<IMagicBinderCardViewModel, int>((card) => card.Quantity));
        }

        public void AddSelectedCardToBinder()
        {
            if (_activeBinder == null || _selectedCard == null)
            {
                return;
            }

            _activeBinder.AddCard(_selectedCard.Definition, 1, SelectedGrade, SelectedLanguage, SelectedCardIsFoil, true);
            _activeBinder.WriteFile();

            _notificationCenter.FireNotification(null, string.Format("Added card '{0}' to binder.", _selectedCard.NameEN));

            RaisePropertyChanged(() => CardsInBinder);
            UpdateSelectedCardFromBinder();
        }

        public void AddCurrentDisplayedList()
        {
            if (_activeBinder == null || _searchResult == null || !_searchResult.Any())
            {
                return;
            }

            _activeBinder.AddCards(_searchResult, SelectedGrade, SelectedLanguage, SelectedCardIsFoil);
            _activeBinder.WriteFile();
            _notificationCenter.FireNotification(null, "Added cards to binder.");

            RaisePropertyChanged(() => CardsInBinder);
            UpdateSelectedCardFromBinder();
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
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            _notificationCenter.FireNotification(null, "Performing card search...");

            var found = _currentDataSource.Lookup(CardLookup).ToList();

            stopwatch.Stop();
            _notificationCenter.FireNotification(null, "Card search returned " + found.Count() + " and took " + stopwatch.Elapsed);
            CardCollection = found;
        }

        public void CreateAndSetNewBinder(string fileName)
        {
            var name = new FileInfo(fileName);
            _activeBinder = new MagicBinderViewModel(name.Name.Substring(0, name.Name.Length - name.Extension.Length), _notificationCenter);
            _activeBinder.WriteFile(fileName);
            _settings.LoadedBinder = name.GetRelativePathFrom(new DirectoryInfo(PathHelper.UserDataFolder));
            SaveSettings();

            RaisePropertyChanged(() => CardsInBinder);
            RaisePropertyChanged(() => IsActiveBinder);
            RaisePropertyChanged(() => ActiveBinder);

            _notificationCenter.FireNotification(null, string.Format("Created binder '{0}'.", _activeBinder.Name));
        }

        public void OpenBinder(string fileName)
        {
            try
            {
                var name = new FileInfo(fileName);
                _activeBinder = new MagicBinderViewModel(name.Name.Substring(0, name.Name.Length - name.Extension.Length), _notificationCenter);
                _activeBinder.ReadFile(fileName);

                _settings.LoadedBinder = name.GetRelativePathFrom(new DirectoryInfo(PathHelper.UserDataFolder));
                SaveSettings();

                _notificationCenter.FireNotification(LogLevel.Info, string.Format("Opened binder '{0}'.", _activeBinder.Name));
            }
            catch (Exception error)
            {
                _activeBinder = null;
                _notificationCenter.FireNotification(LogLevel.Error, string.Format("Error opening binder '{0}': {1}", _activeBinder.Name, error.Message));
            }
            finally
            {
                RaisePropertyChanged(() => CardsInBinder);
                RaisePropertyChanged(() => IsActiveBinder);
                RaisePropertyChanged(() => ActiveBinder);
            }
        }

        public void LoadBinder(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Exists)
            {
                _notificationCenter.FireNotification(null, string.Format("Binder file '{0}' does not exist.", fileInfo.Name));
                return;
            }

            _activeBinder = new MagicBinderViewModel(_notificationCenter);
            _activeBinder.ReadFile(fileName);
            _settings.LoadedBinder = fileInfo.GetRelativePathFrom(new DirectoryInfo(PathHelper.UserDataFolder));
            SaveSettings();

            RaisePropertyChanged(() => CardsInBinder);
            RaisePropertyChanged(() => IsActiveBinder);
            RaisePropertyChanged(() => ActiveBinder);

            _notificationCenter.FireNotification(null, string.Format("Loaded binder '{0}'.", _activeBinder.Name));
        }

        public void ShowCollectionCards()
        {
            if (_activeBinder == null)
            {
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            _notificationCenter.FireNotification(null, "Collecting cards from binder...");

            var cards = _activeBinder.Cards.Select(c => new FoundMagicCardViewModel(c)).ToList();

            stopwatch.Stop();
            _notificationCenter.FireNotification(null, "Binder card collection took " + stopwatch.Elapsed);

            CardCollection = cards;
        }

        public void ShowTradeBinderCards()
        {
            if (_activeBinder == null)
            {
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            _notificationCenter.FireNotification(null, "Collecting trade cards from binder...");

            var cards = _activeBinder.Cards.Where(c => c.QuantityTrade > 0).Select(c => new FoundMagicCardViewModel(c)).ToList();

            stopwatch.Stop();
            _notificationCenter.FireNotification(null, "Trade card collection took " + stopwatch.Elapsed);

            CardCollection = cards;
        }

        public void ShowWantBinderCards()
        {
            if (_activeBinder == null)
            {
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            _notificationCenter.FireNotification(null, "Collecting want cards from binder...");

            var cards = _activeBinder.Cards.Where(c => c.QuantityWanted > 0).Select(c => new FoundMagicCardViewModel(c)).ToList();

            stopwatch.Stop();
            _notificationCenter.FireNotification(null, "Want card collection took " + stopwatch.Elapsed);

            CardCollection = cards;
        }

        public void PriceActiveBinder()
        {
            if (_activeBinder == null)
            {
                return;
            }

            _activeBinder.PriceBinder();
        }

        public void PriceSearchResult()
        {
            var searchResult = _searchResult;
            if (searchResult == null || !searchResult.Any())
            {
                return;
            }

            searchResult = searchResult.DistinctBy(c => c.NameEN).ToList();

            _notificationCenter.FireNotification(
                LogLevel.Info,
                string.Format("Starting price lookup for {0} cards", searchResult.Count()));

            Task.Factory.StartNew(() =>
            {
                var stopwatch = Stopwatch.StartNew();
                foreach (var card in searchResult)
                {
                    card.UpdatePriceData(false);
                }

                StaticPriceDatabase.Write();

                stopwatch.Stop();

                _notificationCenter.FireNotification(
                    LogLevel.Info,
                    string.Format("Price lookup for {0} cards took {1}", searchResult.Count(), stopwatch.Elapsed));
            });
        }

        private void InternalExport(
                                                                                                            string fileName,
            IEnumerable<IMagicBinderCardViewModel> cardsToExport,
            Func<IMagicBinderCardViewModel, int> quantitySelector)
        {
            if (cardsToExport == null || !cardsToExport.Any())
            {
                return;
            }

            var info = new FileInfo(fileName);
            _notificationCenter.FireNotification(null, string.Format("Exporting '{0}'...", info.Name));
            var watch = Stopwatch.StartNew();

            var writer = new DeckBoxCsvWriter();
            writer.Write(fileName, cardsToExport, SelectedCardIsFoil, SelectedLanguage, SelectedGrade, quantitySelector);

            watch.Stop();
            _notificationCenter.FireNotification(null, string.Format("Exported '{0}' in {1}", info.Name, watch.Elapsed));
        }

        private void UpdateSelectedCardFromBinder()
        {
            if (_activeBinder == null || _selectedCard == null)
            {
                _selectedCardsInBinder = null;
            }
            else
            {
                var selectedName = _selectedCard.NameEN;
                var foundCards = _activeBinder.Cards.Where(c => c.CardNameEN == selectedName).ToList();
                _selectedCardsInBinder = new ObservableCollection<MagicBinderCardViewModel>(foundCards);
            }

            RaisePropertyChanged(() => SelectedCardsInBinder);
        }
    }
}