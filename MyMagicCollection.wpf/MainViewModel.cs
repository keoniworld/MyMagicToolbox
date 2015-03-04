﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Minimod.NotificationObject;
using MyMagicCollection.Shared;
using MyMagicCollection.Shared.DataSource;
using MyMagicCollection.Shared.FileFormats.DeckBoxCsv;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.ViewModels;
using MyMagicCollection.wpf.Settings;
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

        private bool _canAddSelected;

        private bool _selectedCardIsFoil;

        private IEnumerable<MagicBinderCardViewModel> _selectedCardsInBinder;

        private LookupSource _lookupSource;

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
                LoadBinder(new DirectoryInfo(PathHelper.UserDataFolder).MakeAbsolutePath(_settings.LoadedBinder));
            }
        }

        public IEnumerable<MagicLanguage> AvailableLanguages { get; } = (IEnumerable<MagicLanguage>)Enum.GetValues(typeof(MagicLanguage));

        public IEnumerable<MagicGrade> AvailableGrades { get; } = (IEnumerable<MagicGrade>)Enum.GetValues(typeof(MagicGrade));

        public CardLookup CardLookup { get; private set; }

        public string DatabaseSummary { get; set; }

        public IEnumerable<MagicBinderCardViewModel> CardsInBinder => _activeBinder?.Cards;

        public IEnumerable<MagicBinderCardViewModel> SelectedCardsInBinder => _selectedCardsInBinder;

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
                        break;

                    case LookupSource.CardDatabase:
                    default:
                        _currentDataSource = new StaticMagicDataDataSource();
                        break;
                }

                RaisePropertyChanged(() => LookupSource);
            }
        }

        public void ImportCardList(string fileName)
        {
            var info = new FileInfo(fileName);
            _notificationCenter.FireNotification(null, string.Format("Loading '{0}'...", info.Name));

            var reader = new DeckBoxInventoryCsvReader(_notificationCenter);
            var found = reader.ReadCsvFile(fileName);

            CardCollection = found.Select(c => new FoundMagicCardViewModel(c));
            // TODO: Analyse file formats later
        }

        public void AddSelectedCardToBinder()
        {
            if (_activeBinder == null || _selectedCard == null)
            {
                return;
            }

            _activeBinder.AddCard(_selectedCard.Definition, 1, SelectedGrade, SelectedLanguage, SelectedCardIsFoil);
            _activeBinder.WriteFile();

            _notificationCenter.FireNotification(null, string.Format("Added card '{0}' to binder.", _selectedCard.NameEN));

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
            _activeBinder = new MagicBinderViewModel(name.Name.Substring(0, name.Name.Length - name.Extension.Length));
            _activeBinder.WriteFile(fileName);
            _settings.LoadedBinder = name.GetRelativePathFrom(new DirectoryInfo(PathHelper.UserDataFolder));
            SaveSettings();

            RaisePropertyChanged(() => CardsInBinder);
            RaisePropertyChanged(() => IsActiveBinder);
            RaisePropertyChanged(() => ActiveBinder);

            _notificationCenter.FireNotification(null, string.Format("Created binder '{0}'.", _activeBinder.Name));
        }

        public void LoadBinder(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Exists)
            {
                _notificationCenter.FireNotification(null, string.Format("Binder file '{0}' does not exist.", fileInfo.Name));
                return;
            }

            _activeBinder = new MagicBinderViewModel();
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