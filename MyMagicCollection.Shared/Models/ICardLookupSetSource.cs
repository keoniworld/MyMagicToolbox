using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMagicCollection.Shared.Models
{
    public interface ICardLookupSetSource
    {
        IEnumerable<MagicSetDefinition> AvailableSearchSets { get;  }

		MagicSetDefinition SearchSet { get; set; }

        bool IsAllSearchSet { get; }
    }
}
