using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MyMagicCollection.Shared.CSV;

namespace MyMagicCollection.Shared.Models
{
    /// <summary>
    /// This class contains all the static MTG card data.
    /// </summary>
    public class StaticMagicData
    {
        static StaticMagicData()
        {
            var stopWatch = Stopwatch.StartNew();

            var loader = new MagicDatabaseLoader();
            CardDefinitions = loader.LoadCardDatabase();
            SetDefinitions = loader.LoadSetDatabase();

            CardDefinitionsByCardId = CardDefinitions.ToDictionary(c => c.CardId);
            SetDefinitionsBySetCode=SetDefinitions.ToDictionary(c => c.Code);

            stopWatch.Stop();
            Debug.WriteLine("Loading static MTG data took " + stopWatch.Elapsed);
        }

        public static IEnumerable<MagicCardDefinition> CardDefinitions { get; private set; }

        public static IEnumerable<MagicSetDefinition> SetDefinitions { get; private set; }

        public static IReadOnlyDictionary<string, MagicCardDefinition> CardDefinitionsByCardId { get; private set; }

        public static IReadOnlyDictionary<string, MagicSetDefinition> SetDefinitionsBySetCode { get; private set; }
    }
}