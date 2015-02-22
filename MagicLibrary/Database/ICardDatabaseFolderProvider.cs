using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDatabase
{
    public interface ICardDatabaseFolderProvider
    {
        string MagicCardDatabaseFolder { get; }
        string UserCardDatabaseFolder { get; }
    }
}
