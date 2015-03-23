using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MyMagicCollection.wpf.UserControls
{
    public static class BitmapImageCache
    {
        private static object _sync = new object();

        private static Dictionary<string, BitmapImage> _cache = new Dictionary<string, BitmapImage>();

        public static BitmapImage GetImage(string fileName)
        {
            var key = fileName.ToLowerInvariant();

            BitmapImage result = null;
            lock (_sync)
            {
                if (_cache.TryGetValue(key, out result))
                {
                    return result;
                }
            }

            if (File.Exists(fileName))
            {
                var uri = new Uri(fileName);
                result = new BitmapImage(uri);
                result.Freeze();
            }

            lock (_sync)
            {
                _cache.Add(key, result);
            }

            return result;
        }
    }
}