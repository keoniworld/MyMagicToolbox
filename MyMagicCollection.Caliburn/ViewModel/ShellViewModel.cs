using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using MagicLibrary;
using PropertyChanged;

namespace MyMagicCollection.Caliburn
{
    [ImplementPropertyChanged]
    [Export(typeof(IShell))]
    public class ShellViewModel : PropertyChangedBase
    {
        [ImportingConstructor]
        public ShellViewModel(
            INotificationCenter notificationCenter)
        {
            notificationCenter.NotificationFired += (sender, e) =>
                {
                    StatusBarMessage = e.Message;
                };
        }

        [Import]
        public CardDatabaseViewModel CardDatabaseViewModel { get; set; }

        [Import]
        public CollectionViewModel CollectionViewModel { get; set; }

        [Import]
        public DeckToolsViewModel DeckToolsViewModel { get; set; }

        public string StatusBarMessage { get; set; }
    }
}