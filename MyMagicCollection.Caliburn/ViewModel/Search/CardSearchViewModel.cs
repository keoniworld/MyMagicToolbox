using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicFileContracts;
using PropertyChanged;

namespace MyMagicCollection.Caliburn
{
    [ImplementPropertyChanged]
    public class CardSearchViewModel : ICardSearchModel
    {
        public CardSearchViewModel()
        {
            DistinctNames = true;
        }

        public string SearchTerm { get; set; }

        public bool SearchName { get; set; }
        public bool SearchRulesText { get; set; }

        public bool DistinctNames { get; set; }

        public event EventHandler SearchTriggered;

        public void SearchButton()
        {
            if (SearchTriggered != null)
            {
                SearchTriggered(this, EventArgs.Empty);
            }
        }
    }
}
