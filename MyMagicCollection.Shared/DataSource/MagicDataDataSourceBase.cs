using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.ViewModels;

namespace MyMagicCollection.Shared.DataSource
{
    public abstract class MagicDataDataSourceBase : IMagicDataSource
    {
        public abstract IEnumerable<IMagicCardDefinition> CardDefinitions { get; }

        public static bool IsNameMatch(
            IMagicCardDefinition definition,
            CardLookup lookup)
        {
            if (definition.DisplayNameEn.IndexOf(lookup.SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return true;
            }

            if (lookup.SearchGerman && definition.NameDE.IndexOf(lookup.SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return true;
            }

            if (lookup.SearchRules && definition.RulesText.IndexOf(lookup.SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return true;
            }

            return false;
        }

        public IEnumerable<FoundMagicCardViewModel> Lookup(CardLookup lookupOptions)
        {
            var result = CardDefinitions;

            // AND set
            if (lookupOptions.SearchSet != null && !lookupOptions.SetSource.IsAllSearchSet)
            {
                var setDefinition = StaticMagicData.SetDefinitionsBySetName[lookupOptions.SearchSet.Name];
                result = result.Where(c => c.SetCode == setDefinition.Code);
            }

            // AND Name
            if (!string.IsNullOrEmpty(lookupOptions.SearchTerm))
            {
                result = result.Where(c => IsNameMatch(c, lookupOptions));
            }

            // TODO: Andere optionen

            if (lookupOptions.DisplayDistinct)
            {
                result = result.DistinctBy(c => c.DisplayNameEn);
            }

            return MapResult(result).ToList();
        }

        protected abstract IEnumerable<FoundMagicCardViewModel> MapResult(IEnumerable<IMagicCardDefinition> result);
    }
}