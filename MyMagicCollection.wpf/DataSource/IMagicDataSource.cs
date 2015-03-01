using System.Collections.Generic;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.wpf.ViewModels;

namespace MyMagicCollection.wpf.DataSource
{
    public interface IMagicDataSource
    {
        IEnumerable<FoundMagicCardViewModel> Lookup(CardLookup lookupOptions);
    }
}