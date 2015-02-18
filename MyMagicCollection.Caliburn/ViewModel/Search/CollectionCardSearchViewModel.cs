using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace MyMagicCollection.Caliburn
{
    [ImplementPropertyChanged]
    public class CollectionCardSearchViewModel : CardSearchViewModel
    {
        public CollectionCardSearchViewModel()
        {
            SearchName = true;
        }

    }
}
