using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMagicCollection.Shared.Models
{
    public class CardLookupSetSourceAllSets : ICardLookupSetSource
    {
        public const string AllSetsSearchSetName = "All Sets";

		private MagicSetDefinition _allSets = new MagicSetDefinition()
		{
			Name = AllSetsSearchSetName,
		};

		public CardLookupSetSourceAllSets()
        {
            var allSets = StaticMagicData.SetDefinitions
                .OrderByDescending(s => s.ReleaseDateTime)
                .ThenBy(s => s.Name)
                .ToList();

            allSets.Insert(0, _allSets);
            AvailableSearchSets = allSets;
            SearchSet = _allSets;
        }

        public IEnumerable<MagicSetDefinition> AvailableSearchSets { get; private set; }

        public MagicSetDefinition SearchSet { get; set; }

        public bool IsAllSearchSet
        {
            get
            {
                return SearchSet.Name == AllSetsSearchSetName;
            }
        }
    }
}