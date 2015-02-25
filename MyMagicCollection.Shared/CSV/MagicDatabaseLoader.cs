using System.Collections.Generic;
using System.IO;
using CsvHelper;
using MyMagicCollection.Shared.Helper;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.Shared.CSV
{
    public class MagicDatabaseLoader
    {
        public MagicDatabaseLoader()
        {
        }

        public IEnumerable<MagicCardDefinition> LoadCardDatabase()
        {
            var result = new List<MagicCardDefinition>();
            var assembly = GetType().Assembly;
            var resourceName = assembly.FindEmbeddedResource("CSV.MagicDatabase.csv");
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                var inputCsv = new CsvReader(new StreamReader(stream));

                while (inputCsv.Read())
                {
                    var line = inputCsv.GetRecord<MagicCardDefinition>();
                    result.Add(line);
                }
            }
            return result;
        }

        public IEnumerable<MagicSetDefinition> LoadSetDatabase()
        {
            var result = new List<MagicSetDefinition>();
            var assembly = GetType().Assembly;
            var resourceName = assembly.FindEmbeddedResource("CSV.MagicDatabaseSets.csv");
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                var inputCsv = new CsvReader(new StreamReader(stream));

                while (inputCsv.Read())
                {
                    var line = inputCsv.GetRecord<MagicSetDefinition>();
                    result.Add(line);
                }
                return result;
            }
        }
    }
}