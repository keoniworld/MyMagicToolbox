using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDatabase
{
    [DebuggerDisplay("{Ncode} - {Nname}")]
    public class Set
    {
        public string Ncode { get; set; }
        public string Nname { get; set; }
        public string Ndate { get; set; }
    }
}
