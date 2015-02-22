using System.Collections.Generic;
using System.IO;
using CsvHelper;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.Shared.CSV
{
    public class MagicDatabaseLoader
    {
        public MagicDatabaseLoader()
        {
        }

        public IEnumerable<MagicCardDefinition> LoadCardDatabase(FileInfo databaseFile)
        {
            var inputCsv = new CsvReader(new StringReader(File.ReadAllText(databaseFile.FullName)));

            var result = new List<MagicCardDefinition>();
            while (inputCsv.Read())
            {
                var line = inputCsv.GetRecord<MagicCardDefinition>();
                result.Add(line);
            }
            return result;
        }

        public IEnumerable<MagicSetDefinition> LoadSetDatabase(FileInfo databaseFile)
        {
            var inputCsv = new CsvReader(new StringReader(File.ReadAllText(databaseFile.FullName)));

            var result = new List<MagicSetDefinition>();
            while (inputCsv.Read())
            {
                var line = inputCsv.GetRecord<MagicSetDefinition>();
                result.Add(line);
            }
            return result;
        }
    }
}