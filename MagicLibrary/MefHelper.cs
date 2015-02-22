using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicLibrary
{
    public static class MefHelper
    {
        public static CompositionContainer Container {get;set;}

        public static void SatisfyImports(object target)
        {
            Container.SatisfyImportsOnce(target);
        }
    }
}
