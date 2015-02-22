using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Dapper;
using MagicLibrary;

namespace MagicDatabase
{
    [Export(typeof(IUserDatabase))]
    public sealed class UserDatabase : IDisposable, IUserDatabase
    {
        private readonly string _databaseFolder;
        private readonly SQLiteConnection _connection;

        [ImportingConstructor]
        public UserDatabase(ICardDatabaseFolderProvider folderProvider)
        {
            this._databaseFolder = folderProvider.UserCardDatabaseFolder;
            _connection = SimpleDbConnection();
            _connection.Open();
        }

        public const string SqliteName = "UserDatabase.s3db";

        public string DatabaseName
        {
            get
            {
                return Path.Combine(
                    this._databaseFolder,
                    SqliteName);
            }
        }

        public SQLiteConnection SimpleDbConnection()
        {
            return new SQLiteConnection("Data Source=" + DatabaseName);
        }

        ////public Card FindCardById(string magicCardId)
        ////{
        ////    return _connection.Query<Card>("SELECT * from MagicCard where CardId = @CardId", new { CardId = magicCardId }).FirstOrDefault();
        ////}

        ////public IEnumerable<Set> GetAllSets()
        ////{
        ////    return _connection.Query<Set>("Select * from Nsets;");
        ////}

        public SettingsItem InternalGetSettingsValue(string key)
        {
            return _connection
                .Query<SettingsItem>("SELECT * from Settings where Key = @Key", new { Key = key })
                .FirstOrDefault();
        }

        public string GetSettingsValue(string key)
        {
            var result = InternalGetSettingsValue(key);
            return result != null ? result.Value : null;
        }

        public void SetSettingsValue(string key, string value)
        {
            var found = InternalGetSettingsValue(key);
            if (found == null)
            {
                var insertStatement = typeof(SettingsItem).GetInsertStatement("Settings", null);

                var item = new SettingsItem
                {
                    Key = key,
                    Value = value,
                };

                _connection.Query(insertStatement, item);
            }
            else
            {
                if (found.Value != value)
                {
                    found.Value = value;
                    const string updateStatement = "UPDATE Settings SET Value=@Value WHERE key = @KEY";
                    _connection.Query(updateStatement, found);
                }
            }
        }

        /// <summary>
        /// Gets all collections from the database.
        /// </summary>
        /// <returns>Returns a collection containing all collections currently existing in the database.</returns>
        public IEnumerable<MagicCollection> GetAllCollections()
        {
            return _connection.Query<MagicCollection>("SELECT * FROM Collection");
        }

        public void InsertOrUpdateCollection(MagicCollection collection)
        {
            if (collection.Id >= 0)
            {
                // Update
                var statement = collection.GetType().GetUpdateStatement("Collection", "Id");
                _connection.Query(statement, collection);
            }
            else
            {
                // Insert
                var statement = collection.GetType().GetInsertStatement("Collection", "Id");
                collection.Id = _connection.Query<int>(statement, collection).First();
                // collection.Id = _connection.Query(statement, collection).FirstOrDefault();
            }
        }

        // TODO: Speichern der collection

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _connection.Close();
        }
    }
}