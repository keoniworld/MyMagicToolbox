using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace MyMagicCollection.Caliburn
{
    [Export]
    [ImplementPropertyChanged]
    public class CardDatabaseSearchViewModel : CardSearchViewModel
    {
        public CardDatabaseSearchViewModel()
        {
            SearchName = true;
            DistinctNames = true;
        }
    }
}
