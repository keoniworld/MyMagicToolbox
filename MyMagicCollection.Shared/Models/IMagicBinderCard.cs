using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMagicCollection.Shared.Models
{
    public interface IMagicBinderCard
    {
        int Quantity { get; }

        int QuantityTrade { get; }

        int QuantityWanted { get; }

        bool IsFoil { get; }

        MagicLanguage? Language { get; }

        string CardId { get; }

        MagicGrade? Grade { get; }
    }
}