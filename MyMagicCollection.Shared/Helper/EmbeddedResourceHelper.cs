using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyMagicCollection.Shared.Helper
{
    public static class EmbeddedResourceHelper
    {
        public static string FindEmbeddedResource(this Assembly assembly, string resourceName)
        {
            resourceName = resourceName.ToLowerInvariant();
            return assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.ToLowerInvariant().Contains(resourceName));
        }
    }
}
