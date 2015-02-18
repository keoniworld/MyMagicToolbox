using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDatabase
{
    public class MagicCardItem
    {
        public int Id { get; set; }
        public int CollectionId { get; set; }
        public int Quantity { get; set; }
        public string CardId { get; set; }
        public int Language { get; set; }
        public bool IsFoil { get; set; }
    }
}
