using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyMagicCollection.Shared
{
    public static class PathHelper
    {
        static PathHelper()
        {
            ExeFolder = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.FullName;
            UserDataFolder = Path.Combine(ExeFolder, "App_Data");
            ImageCacheFolder = Path.Combine(UserDataFolder, "ImageCache");
        }

        public static string ExeFolder{get;private set;}

        public static string UserDataFolder { get; private set; }

        public static string ImageCacheFolder { get; private set; }
    }
}
