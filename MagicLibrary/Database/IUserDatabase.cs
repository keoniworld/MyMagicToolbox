using System.Collections.Generic;
namespace MagicDatabase
{
    public interface IUserDatabase
    {
        string GetSettingsValue(string key);

        void SetSettingsValue(string key, string value);

        void Dispose();

        /// <summary>
        /// Gets all collections from the database.
        /// </summary>
        /// <returns>Returns a collection containing all collections currently existing in the database.</returns>
        IEnumerable<MagicCollection> GetAllCollections();

        void InsertOrUpdateCollection(MagicCollection collection);
    }
}