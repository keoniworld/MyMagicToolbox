using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicLibrary
{
    public interface ICurrentCollectionProvider
    {
        ISingleCollectionViewModel SelectedCollection { get; }

        /// <summary>
        /// Occurs when collection loading has been started
        /// </summary>
        event EventHandler<EventArgs> CollectionLoading;

        /// <summary>
        /// Occurs when a collection has been fully loaded
        /// </summary>
        event EventHandler<EventArgs> CollectionLoaded;
    }
}
