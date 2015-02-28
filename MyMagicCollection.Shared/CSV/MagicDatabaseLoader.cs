using System.Collections.Generic;
using System.IO;
using System.Linq;
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
				using (var inputCsv = new CsvReader(new StreamReader(stream)))
				{
					return inputCsv.GetRecords<MagicCardDefinition>().ToList();
				}
			}
        }

        public IEnumerable<MagicSetDefinition> LoadSetDatabase()
        {
            var assembly = GetType().Assembly;
            var resourceName = assembly.FindEmbeddedResource("CSV.MagicDatabaseSets.csv");
			using (var stream = assembly.GetManifestResourceStream(resourceName))
			{
				using (var inputCsv = new CsvReader(new StreamReader(stream)))
				{
					return inputCsv.GetRecords<MagicSetDefinition>().ToList(); ;
				}
			}
        }
    }
}