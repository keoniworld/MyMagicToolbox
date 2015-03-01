using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minimod.NotificationObject;

namespace MyMagicCollection.Shared.Models
{
    /// <summary>
    /// This is the model which can be bound to the UI to enter search details.
    /// </summary>
    public class CardLookup : NotificationObject
    {
        private string _searchTerm;

        public string SearchTerm
        {
            get
            {
                return _searchTerm;
            }

            set
            {
                _searchTerm = value;
                RaisePropertyChanged(() => SearchTerm);
            }
        }
    }
}
