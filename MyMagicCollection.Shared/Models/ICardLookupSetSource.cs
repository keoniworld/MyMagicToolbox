using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMagicCollection.Shared.Models
{
    public interface ICardLookupSetSource
    {
        IEnumerable<string> AvailableSearchSets { get;  }

        string SearchSet { get; set; }

        bool IsAllSearchSet { get; }
    }
}
