using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicLibrary;

namespace MagicFileFormats
{
    [DebuggerDisplay("{CardId}, {Quantity}, {Name}, {Location}, {SetCode}, IsPromo={IsPromo}")]
    public class DeckCard : IDeckCard
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string CardId { get; set; }
        public string Location { get; set; }

        public string SetCode { get; set; }
        public string Set { get; set; }
        public bool IsFoil { get; set; }
        public int? NumberInSet { get; set; }
    }
}
