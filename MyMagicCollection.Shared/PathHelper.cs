using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyMagicCollection.Shared
{
    public class PathHelper
    {
        static PathHelper()
        {
            // TODO: Frage nach Web
            ExeDir = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.FullName;
        }

        public static string ExeDir
        {
            get;
            private set;
        }
    }
}
