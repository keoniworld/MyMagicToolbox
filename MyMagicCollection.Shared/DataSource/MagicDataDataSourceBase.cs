using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.ViewModels;

namespace MyMagicCollection.Shared.DataSource
{
    public abstract class MagicDataDataSourceBase : IMagicDataSource
    {
		public abstract IEnumerable<MagicCardDefinition> CardDefinitions { get; }

        public IEnumerable<FoundMagicCardViewModel> Lookup(CardLookup lookupOptions)
        {
            var result = CardDefinitions;

            // TODO: Optimize this
            var name = lookupOptions.SearchTerm.ToLowerInvariant();
            result = result.Where(c => c.NameEN.ToLowerInvariant().Contains(name));

            // TODO: Andere optionen

            return result.Select(c => new FoundMagicCardViewModel(c)).OrderBy(c => c.NameEN).ToList();
        }
    }
}