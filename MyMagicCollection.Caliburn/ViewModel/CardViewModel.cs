using System.Diagnostics;
using MagicDatabase;
using MagicLibrary;
using MagicFileFormats;
using PropertyChanged;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.Caliburn
{
    [ImplementPropertyChanged]
    [DebuggerDisplay("{CardId}, {Quantity}, {Name}, {Location}, {SetCode}")]
    public sealed class CardViewModel : IDeckCard
    {
        public CardViewModel()
        {
        }

        public CardViewModel(MagicCardDefinition card)
        {
            Name = card.NameEN;
            CardId = card.CardId;
            SetCode = card.SetCode;
            // Set = card.Set;
            IsFoil = false;
            Price = null;
            NumberInSet = card.NumberInSet;

        }

        public CardViewModel(IDeckCard card)
        {
            Name = card.Name;
            Quantity = card.Quantity;
            CardId = card.CardId;
            Location = card.Location;
            SetCode = card.SetCode;
            Set = card.Set;
            IsFoil = card.IsFoil;
            Price = null;

            // TODO: Get this
            NumberInSet = null;
        }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public string CardId { get; set; }

        public string Location { get; set; }

        public string SetCode { get; set; }

        public string Set { get; set; }

        public bool IsFoil { get; set; }

        public decimal? Price { get; set; }

        public string ImagePathMkm { get; set; }

        public string WebSiteMkm { get; set; }

        public int? NumberInSet { get; set; }
    }
}