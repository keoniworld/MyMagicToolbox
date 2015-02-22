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
    public class CardDatabaseSearchViewModel : CardSearchViewModel
    {
        [ImportingConstructor]
        public CardDatabaseSearchViewModel(
            ICardDatabase cardDatabase)
        : base(cardDatabase)
        {
            SearchName = true;
            DistinctNames = true;
        }
    }
}
