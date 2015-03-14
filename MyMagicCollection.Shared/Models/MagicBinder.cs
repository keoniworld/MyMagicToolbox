using System.Collections.Generic;

namespace MyMagicCollection.Shared.Models
{
    public class MagicBinder
    {
        public MagicBinder()
            : this(new List<MagicBinderCard>())
        {
        }

        public MagicBinder(IList<MagicBinderCard> cards)
        {
            Cards = cards;
            Version = 1;
        }

        public string Name { get; set; }

        public int Version { get; set; }

        public decimal PriceNonBulk { get; set; }

        public decimal PriceBulk { get; set; }

        public decimal PriceTotal { get; set; }

        public IList<MagicBinderCard> Cards { get; private set; }
    }
}