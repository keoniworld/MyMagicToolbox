using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MagicLibrary;
using MoreLinq;

namespace MagicDatabase
{
    [Export(typeof(ICardDatabase))]
    public sealed class CardDatabase : IDisposable, ICardDatabase
    {
        private readonly string _databaseFolder;
        private readonly SQLiteConnection _connection;

        [ImportingConstructor]
        public CardDatabase(ICardDatabaseFolderProvider folderProvider)
        {
            this._databaseFolder = folderProvider.MagicCardDatabaseFolder;
            _connection = SimpleDbConnection();
            _connection.Open();
        }

        public string DatabaseName
        {
            get
            {
                return Path.Combine(
                    this._databaseFolder,
                    "MagicDatabase.s3db");
            }
        }

        public SQLiteConnection SimpleDbConnection()
        {
            return new SQLiteConnection("Data Source=" + DatabaseName);
        }

        public Card FindCardById(string magicCardId)
        {
            return _connection.Query<Card>("SELECT * from MagicCard where CardId = @CardId", new { CardId = magicCardId }).FirstOrDefault();
        }

        public IEnumerable<Card> FindCards(ICardSearchModel searchModel)
        {
            var query = new StringBuilder();
            query.Append("SELECT * from MagicCard where ");

            var names = new List<string>();

            // Names

            if (searchModel.SearchName)
            {
                names.Add("(NameEN LIKE @SearchTerm)");
                names.Add("(NameDE LIKE @SearchTerm)");
            }

            if (searchModel.SearchRulesText)
            {
                names.Add("(RulesText LIKE @SearchTerm)");
                names.Add("(RulesTextDE LIKE @SearchTerm)");
            }

            // Add names
            if (names.Any())
            {
                query.Append("(");

                query.Append(string.Join(" OR ", names));

                query.Append(")");
            }

            // add initial ordering
            query.Append(" order by NameEN");

            var found = _connection.Query<Card>(
                query.ToString(),
                new { SearchTerm = "%" + searchModel.SearchTerm + "%" });

            if (searchModel.DistinctNames)
            {
                found = found.DistinctBy(c => c.NameEN);
            }

            return found;
        }

        public IEnumerable<Set> GetAllSets()
        {
            return _connection.Query<Set>("Select * from Nsets;");
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