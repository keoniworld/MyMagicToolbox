using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMagicCollection.Shared.ViewModels;

namespace MyMagicCollection.Shared.Models
{
    public class CardLookupSetSourceActiveBinder : ICardLookupSetSource
    {
        public const string AllSetsSearchSetName = "All Sets";

        public CardLookupSetSourceActiveBinder(MagicBinderViewModel binder)
        {
            var allSets = binder.Cards
                .Select(c => c.SetCode)
                .Distinct()
                .Select(s => StaticMagicData.SetDefinitionsBySetCode[s])
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