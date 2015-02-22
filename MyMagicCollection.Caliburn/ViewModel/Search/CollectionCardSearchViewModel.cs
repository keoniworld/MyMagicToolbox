using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicDatabase;
using PropertyChanged;

namespace MyMagicCollection.Caliburn
{
    [Export]
    [ImplementPropertyChanged]
    public class CollectionCardSearchViewModel : CardSearchViewModel
    {
        [ImportingConstructor]
        public CollectionCardSearchViewModel(ICardDatabase cardDatabase)
        : base(cardDatabase)
        {
            SearchName = true;
        }

    }
}
