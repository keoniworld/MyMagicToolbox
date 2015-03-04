using System.Collections.Generic;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.ViewModels;

namespace MyMagicCollection.Shared.DataSource
{
    public interface IMagicDataSource
    {
        IEnumerable<FoundMagicCardViewModel> Lookup(CardLookup lookupOptions);
    }
}