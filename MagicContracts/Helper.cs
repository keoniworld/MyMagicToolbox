using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicContracts
{
    public static class Helper
    {
        public static IEnumerable<string> GetPropertyNames(this object instance, string columnToIgnore)
        {
            return GetPropertyNames(instance.GetType(), columnToIgnore);
        }

        public static IEnumerable<string> GetPropertyNames(this Type type, string columnToIgnore)
        {
            return type.GetProperties().Select(p => p.Name).Where(p => p != columnToIgnore).ToList();
        }
    }
}