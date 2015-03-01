using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.wpf.ViewModels;

namespace MyMagicCollection.wpf.DataSource
{
    public class StaticMagicDataDataSource : IMagicDataSource
    {
        public StaticMagicDataDataSource()
        {
        }

        public IEnumerable<FoundMagicCardViewModel> Lookup(CardLookup lookupOptions)
        {
            var result = StaticMagicData.CardDefinitions;

            // TODO: Optimize this
            var name = lookupOptions.SearchTerm.ToLowerInvariant();
            result = result.Where(c => c.NameEN.ToLowerInvariant().Contains(name));

            // TODO: Andere optionen


            return result.Select(c=>new FoundMagicCardViewModel(c)).ToList();
        }
    }
}
