using System;
using System.ComponentModel.Composition;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Dapper;
using MagicContracts;

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
                var propertyList = typeof(SettingsItem).GetPropertyNames(null);
                var insertStatement = string.Format(
                    "INSERT INTO Settings ({0}) VALUES ({1});",
                    string.Join(", ", propertyList),
                    string.Join(", ", propertyList.Select(p => "@" + p)));

                var item = new SettingsItem
                {
                    Key = key,
                    Value = value,
                };

                _connection.Query(insertStatement, item);
            }
            else
            {
                found.Value = value;
                var updateStatement = "UPDATE Settings SET Value=@Value WHERE key = @KEY";
                _connection.Query(updateStatement, found);
            }
        }

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