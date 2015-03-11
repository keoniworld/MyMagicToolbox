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
		public abstract IEnumerable<MagicCardDefinition> CardDefinitions { get; }

		public static bool IsNameMatch(
			MagicCardDefinition definition,
			CardLookup lookup)
		{
			if (definition.NameEN.ToLowerInvariant().Contains(lookup.SearchTerm))
			{
				return true;
			}

			if (lookup.SearchGerman && definition.NameDE.ToLowerInvariant().Contains(lookup.SearchTerm))
			{
				return true;
			}

			if (lookup.SearchRules && definition.RulesText.ToLowerInvariant().Contains(lookup.SearchTerm))
			{
				return true;
			}


			return false;
		}

		public IEnumerable<FoundMagicCardViewModel> Lookup(CardLookup lookupOptions)
		{
			var result = CardDefinitions;

			// AND set
			if (lookupOptions.SearchSet != null && lookupOptions.SearchSet != CardLookup.AllSetsSearchSetName)
			{
				var setDefinition = StaticMagicData.SetDefinitionsBySetName[lookupOptions.SearchSet];
				result = result.Where(c => c.SetCode == setDefinition.Code);
			}

			// AND Name
			lookupOptions.SearchTerm = lookupOptions.SearchTerm?.ToLowerInvariant();
			if (!string.IsNullOrEmpty(lookupOptions.SearchTerm))
			{
				result = result.Where(c => IsNameMatch(c, lookupOptions));
			}

			// TODO: Andere optionen

			if (lookupOptions.DisplayDistinct)
			{
				result = result.DistinctBy(c => c.NameEN);
			}

			return result.Select(c => new FoundMagicCardViewModel(c)).OrderBy(c => c.NameEN).ToList();
		}
	}
}