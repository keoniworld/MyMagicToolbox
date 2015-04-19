using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MyMagicCollection.Shared;

namespace MyMagicCollection.wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Create log folder if needed:
            var folder = Path.Combine(PathHelper.ExeFolder, "logs");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
    }
}
