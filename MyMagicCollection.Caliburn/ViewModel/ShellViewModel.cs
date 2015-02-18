using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

namespace MyMagicCollection.Caliburn
{

    [Export(typeof(IShell))]
    public class ShellViewModel : PropertyChangedBase
    {
        public ShellViewModel()
        {
            
            
        }

        [Import]
        public CardDatabaseViewModel CardDatabaseViewModel { get; set; }

        [Import]
        public CollectionViewModel CollectionViewModel { get; set; }

        [Import]
        public DeckToolsViewModel DeckToolsViewModel { get; set; }
    }

}
