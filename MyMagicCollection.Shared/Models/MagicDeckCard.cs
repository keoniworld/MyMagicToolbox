using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMagicCollection.Shared.Models
{
    public class MagicDeckCard
    {
        public string Name { get; set; }

        public int Quantity { get; set; }

        public string CardId { get; set; }

        public MagicDeckLocation Location { get; set; }
    }
}