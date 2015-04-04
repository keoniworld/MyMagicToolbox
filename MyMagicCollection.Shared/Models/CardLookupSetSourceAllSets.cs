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

        public CardLookupSetSourceAllSets()
        {
            var allSets = StaticMagicData.SetDefinitions
                .OrderByDescending(s => s.ReleaseDateTime)
                .ThenBy(s => s.Name)
                .Select(s => s.Name)
                .ToList();

            allSets.Insert(0, AllSetsSearchSetName);
            AvailableSearchSets = allSets;
            SearchSet = AllSetsSearchSetName;
        }

        public IEnumerable<string> AvailableSearchSets { get; private set; }

        public string SearchSet { get; set; }

        public bool IsAllSearchSet
        {
            get
            {
                return SearchSet == AllSetsSearchSetName;
            }
        }
    }
}