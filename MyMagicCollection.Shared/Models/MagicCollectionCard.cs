using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMagicCollection.Shared.Models
{
    public class MagicCollectionCard
    {
        public MagicCollectionCard()
        {
            Language = MagicLanguage.English;
            RowId = Guid.NewGuid().ToString();
        }

        public string RowId { get; set; }

        public int Quantity { get; set; }

        public int QuantityTrade { get; set; }
        public int QuantityWanted { get; set; }

        public bool IsFoil { get; set; }

        public MagicLanguage? Language { get; set; }

        public string CardId { get; set; }

        public MagicGrade? Grade { get; set; }
    }
}