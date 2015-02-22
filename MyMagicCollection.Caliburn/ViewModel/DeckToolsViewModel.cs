using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using MagicDatabase;
using MagicFileFormats.CSV;
using MagicFileFormats.Dec;
using Microsoft.Win32;
using PriceLibrary;
using PropertyChanged;

namespace MyMagicCollection.Caliburn
{
    [Export]
    [ImplementPropertyChanged]
    public sealed class DeckToolsViewModel
    {
        private readonly ICardDatabase _cardDatabase;

        [ImportingConstructor]
        public DeckToolsViewModel(ICardDatabase cardDatabase)
        {
            _cardDatabase = cardDatabase;

            AvailableLanguages = (IEnumerable<MagicLibrary.Language>)Enum.GetValues(typeof(MagicLibrary.Language));
            Language = MagicLibrary.Language.English;
        }

        public IEnumerable<MagicLibrary.Language> AvailableLanguages { get; private set; }

        public MagicLibrary.Language Language { get; set; }

        public bool IsFoil { get; set; }

        public IEnumerable<CardViewModel> CurrentCards { get; private set; }

        public void OnOpenDeckFile()
        {
            var openDialog = new OpenFileDialog()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = ".dec",
                Filter = "Deck Files | *.dec",
                Multiselect = false,
                Title = "Select file to load",
            };

            if (true == openDialog.ShowDialog())
            {
                OpenDecFile(openDialog.FileName);
            }
        }

        public void OpenDecFile(string fileName)
        {
            var reader = new DecReader();
            var cards = reader.ReadFile(fileName);

            var sets = _cardDatabase.GetAllSets().ToDictionary(s => s.Ncode);

            foreach (var card in cards)
            {
                if (!string.IsNullOrWhiteSpace(card.CardId))
                {
                    var found = _cardDatabase.FindCardById(card.CardId);
                    if (found != null)
                    {
                        card.SetCode = found.SetCode;
                        if (sets.ContainsKey(card.SetCode))
                        {
                            card.Set = sets[card.SetCode].Nname;
                        }
                    }
                }
                else
                {
                    // TODO: Suche nach namen
                }
            }

            CurrentCards = cards.Select(c => new CardViewModel(c)).ToList();
        }

        public void SaveDeckboxCsvFile(string fileName)
        {
            var writer = new DeckBoxCsvWriter();
            writer.Write(fileName, CurrentCards, IsFoil, Language);
        }

        public void UpdatePriceData()
        {
            if (CurrentCards != null && CurrentCards.Any())
            {
                Task.Factory.StartNew(() =>
                    {
                        var all = CurrentCards.ToList();
                        foreach (var card in all)
                        {
                            var request = new CardPriceRequest(card);
                            request.PerformRequest();
                            card.Price = request.PriceLow;
                            card.ImagePathMkm = request.ImagePath;
                            card.WebSiteMkm = request.WebSite;
                        }
                    });
            }
        }
    }
}