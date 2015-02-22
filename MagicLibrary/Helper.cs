using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicLibrary
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

        public static string GetInsertStatement(this Type type, string tableName, string columnToIgnore)
        {
            var propertyList = type.GetPropertyNames(columnToIgnore).ToList();
            return string.Format(
                "INSERT INTO {2} ({0}) VALUES ({1});SELECT last_insert_rowid();",
                string.Join(", ", propertyList),
                string.Join(", ", propertyList.Select(p => "@" + p)),
                tableName);

        }

        public static string GetUpdateStatement(this Type type, string tableName, string identityColumn)
        {
            var propertyList = type.GetPropertyNames(identityColumn).ToList();
            return string.Format(
                "UPDATE {1} SET {0} WHERE {2}=@{2}",
                string.Join(", ", propertyList.Select(p => p + "= @" + p)),
                tableName,
                identityColumn);

        }

    }
}