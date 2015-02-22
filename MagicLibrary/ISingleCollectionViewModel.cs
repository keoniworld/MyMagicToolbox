using System;
using System.Collections.Generic;

namespace MagicLibrary
{
    public interface ISingleCollectionViewModel
    {
        string CollectionName { get; set; }

        void SaveHeader();
    }
}
