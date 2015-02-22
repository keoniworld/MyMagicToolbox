using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using MagicDatabase;
using MagicLibrary;
using PropertyChanged;

namespace MyMagicCollection.Caliburn
{
    public class CardSearchViewModel : PropertyChangedBase, ICardSearchModel
    {
        private Set _allSetsMarker;
        public CardSearchViewModel(ICardDatabase cardDatabase)
        {
            DistinctNames = true;

            _allSetsMarker=new Set { Ncode = "AllSets", Nname = "All Sets" };
            var sets = cardDatabase.GetAllSets().ToList();
            sets.Insert(0, _allSetsMarker);
            Sets = sets;
            SelectedSet = _allSetsMarker;
        }

        public event EventHandler SearchTriggered;

        public string SearchTerm { get; set; }

        public bool SearchName { get; set; }

        public bool SearchRulesText { get; set; }

        public bool DistinctNames { get; set; }

        public IEnumerable<Set> Sets { get; private set; }

        public Set SelectedSet
        {
            get;
            set;
        }

        public bool IsFilterBySetActive
        {
            get
            {
                return SelectedSet != null && SelectedSet != _allSetsMarker;
            }
        }

        public void SearchButton()
        {
            if (SearchTriggered != null)
            {
                SearchTriggered(this, EventArgs.Empty);
            }
        }
    }
}