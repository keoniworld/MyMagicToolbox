using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMagicCollection.Shared.Models
{
    [Flags]
    public enum MagicCardType
    {
        Unknown = 0,
        Token = 1,
    }
}