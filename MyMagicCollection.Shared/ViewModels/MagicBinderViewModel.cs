﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Minimod.NotificationObject;
using MoreLinq;
using MyMagicCollection.Shared.FileFormats.coll2;
using MyMagicCollection.Shared.FileFormats.MyMagicCollection;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.Price;
using NLog;
using System.Globalization;

namespace MyMagicCollection.Shared.ViewModels
{
    public class MagicBinderViewModel : NotificationObject
    {
        private readonly INotificationCenter _notificationCenter;
        private MagicBinder _magicCollection;
        private string _fileName;

        private IDictionary<string, MagicBinderCardViewModel> _sortedCards;
        private ObservableCollection<MagicBinderCardViewModel> _cards;

        public MagicBinderViewModel(INotificationCenter notificationCenter)
            : this(null, notificationCenter)
        {
        }

        public MagicBinderViewModel(string name, INotificationCenter notificationCenter)
        {
            _notificationCenter = notificationCenter;
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

        public decimal PriceNonBulk
        {
            get
            {
                return _magicCollection.PriceNonBulk;
            }
            set
            {
                _magicCollection.PriceNonBulk = value;
                RaisePropertyChanged(() => PriceNonBulk);
            }
        }

        public decimal PriceBulk
        {
            get
            {
                return _magicCollection.PriceBulk;
            }
            set
            {
                _magicCollection.PriceBulk = value;
                RaisePropertyChanged(() => PriceBulk);
            }
        }

        public decimal PriceTotal
        {
            get
            {
                return _magicCollection.PriceTotal;
            }
            set
            {
                _magicCollection.PriceTotal = value;
                RaisePropertyChanged(() => PriceTotal);
            }
        }

        public int TotalNumberOfCards { get; private set; }

		public string TotalNumberOfCardsSummary { get; private set; }

		public int TotalNumberOfTradeCards { get; private set; }

        public int TotalNumberOfWantCards { get; private set; }

        public void ReadFile(string fileName)
        {
            var loader = new MyMagicCollectionCsv();
            _magicCollection = loader.ReadFile(fileName);

            if (_magicCollection != null)
            {
                _cards = new ObservableCollection<MagicBinderCardViewModel>(_magicCollection.Cards
                    .Select(c =>
                        {
                            MagicCardDefinition definition;
                            if (StaticMagicData.CardDefinitionsByCardId.TryGetValue(c.CardId, out definition))
                            {
                                return new MagicBinderCardViewModel(definition, c);
                            }
                            else
                            {
                                return null;
                            }
                        }).Where(c => c != null));
            }
            else
            {
                _cards = new ObservableCollection<MagicBinderCardViewModel>();
            }

            // Now wrap every card with a view model:

            foreach (var card in _cards)
            {
                card.PriceChanged += Card_PriceChanged;                
            }

            _sortedCards = Cards.ToDictionary(c => c.RowId);
            _fileName = fileName;

            CalculateTotals();
        }

        private void Card_PriceChanged(object sender, System.EventArgs e)
        {
            CalculateTotals();
            WriteFile();
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

            var collectionWriter = new MyMagicCollectionCsv();
            collectionWriter.WriteFile(fileName, _magicCollection);
            _fileName = fileName;

            // TODO: make this optional:
            var colWriter = new Coll2Writer(_notificationCenter);
            colWriter.Write(_fileName + ".coll2", _magicCollection.Cards);
        }

        public void AddCard(
            IMagicCardDefinition cardDefinition,
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

            viewModel.PriceChanged += Card_PriceChanged;

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
                    card.Quantity,
                    card.Grade.HasValue ? card.Grade.Value : grade,
                    card.Language.HasValue ? card.Language.Value : language,
                    isFoil,
                    false);
            }

            CalculateTotals();
        }

        public void PriceBinder()
        {
            Task.Factory.StartNew(() =>
            {
                var stopwatch = Stopwatch.StartNew();

                _notificationCenter.FireNotification(LogLevel.Info, "Starting price current binder...");

                var cards = _cards.DistinctBy(c => c.CardId).ToList();

                var request = new CardPriceRequest(_notificationCenter);
                request.PerformRequest(cards);
                StaticPriceDatabase.Write();
                CalculateTotals();

                stopwatch.Stop();
                _notificationCenter.FireNotification(LogLevel.Info, string.Format("Done price current binder ({0}).", stopwatch.Elapsed));
            });
        }

   

        private void CalculateTotals()
        {
            var stopwatch = Stopwatch.StartNew();
            TotalNumberOfCards = 0;
            TotalNumberOfTradeCards = 0;
            TotalNumberOfWantCards = 0;

            var priceNonBulk = 0m;
            var priceBulk = 0m;
            var totalNumberOfCards = 0;
            var totalNumberOfTradeCards = 0;
            var totalNumberOfWantCards = 0;

            foreach (var card in _cards)
            {
                totalNumberOfCards += card.Quantity;
                totalNumberOfTradeCards += card.QuantityTrade;
                totalNumberOfWantCards += card.QuantityWanted;

                if (card.Price.HasValue)
                {
                    if (card.Price.Value <= 0.49m)
                    {
                        priceBulk += card.Quantity * card.Price.Value;
                    }
                    else
                    {
                        priceNonBulk += card.Quantity * card.Price.Value;
                    }
                }
            }

			TotalNumberOfCardsSummary = string.Format(
				CultureInfo.CurrentUICulture,
				"{0} ({1} distict)",
				totalNumberOfCards,
				_cards.DistinctBy(c=>c.CardId).Count());

			stopwatch.Stop();
            _notificationCenter.FireNotification(LogLevel.Debug, "CalculateTotals took " + stopwatch.Elapsed);

            PriceBulk = priceBulk;
            PriceNonBulk = priceNonBulk;
            PriceTotal = priceBulk + priceNonBulk;

            TotalNumberOfCards = totalNumberOfCards;
            TotalNumberOfTradeCards = totalNumberOfTradeCards;
            TotalNumberOfWantCards = totalNumberOfWantCards;

            RaisePropertyChanged(() => TotalNumberOfCards);
            RaisePropertyChanged(() => TotalNumberOfTradeCards);
            RaisePropertyChanged(() => TotalNumberOfWantCards);
            RaisePropertyChanged(() => TotalNumberOfCardsSummary);
        }
    }
}