using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicDatabase;
using PropertyChanged;

namespace MyMagicCollection
{
    [ImplementPropertyChanged]
    public class MainViewModel
    {
        private readonly CardDatabase _cardDatabase;
        private readonly IUserDatabase _userDatabase;

        public MainViewModel(
            CardDatabase cardDatabase,
            IUserDatabase userDatabase)
        {
            _cardDatabase = cardDatabase;
            _userDatabase = userDatabase;

            DeckTools = new DeckToolsViewModel(_cardDatabase);
        }

        public DeckToolsViewModel DeckTools { get; private set; }
    }
}
