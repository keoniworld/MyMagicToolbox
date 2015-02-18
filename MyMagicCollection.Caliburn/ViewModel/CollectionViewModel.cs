using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMagicCollection.Caliburn
{
    [Export]
    public class CollectionViewModel
    {
        public string CollectionName { get { return "TEST"; } }
    }
}
