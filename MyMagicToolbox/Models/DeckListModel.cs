using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MagicFileFormats;
using MagicFileFormats.Dec;

namespace MyMagicToolbox.Models
{
    public class DeckListModel
    {
        public IEnumerable<DeckCard> Cards {get;set;}
    }
}