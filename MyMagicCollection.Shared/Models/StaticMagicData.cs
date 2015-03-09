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
            try
            {
                var stopWatch = Stopwatch.StartNew();

                var loader = new MagicDatabaseLoader();
                CardDefinitions = loader.LoadCardDatabase();
                SetDefinitions = loader.LoadSetDatabase();

                var duplicateValues = CardDefinitions.GroupBy(c => MakeNameSetCode(c.SetCode, c.NameEN, c.NumberInSet)).Where(x => x.Count() > 1)
                    .ToArray();


                var dict = new Dictionary<string, MagicCardDefinition>();
                foreach (var def in CardDefinitions)
                {
                    var unique = MakeNameSetCode(def.SetCode, def.NameEN, def.NumberInSet);
                    if (!dict.ContainsKey(unique))
                    {
                        dict.Add(unique, def);
                    }
                }

                CardDefinitionsByNameSetCode = dict;
                CardDefinitionsByCardId = CardDefinitions.ToDictionary(c => c.CardId);

                SetDefinitionsBySetCode = SetDefinitions.ToDictionary(c => c.Code);
                SetDefinitionsBySetName = SetDefinitions.ToDictionary(c => c.Name);

                stopWatch.Stop();
                Debug.WriteLine("Loading static MTG data took " + stopWatch.Elapsed);
            }
            catch
            {
                // ignore this
            }
        }

        public static string MakeNameSetCode(string setCode, string name, int? cardNumber)
        {
            return setCode + name + (cardNumber.HasValue ? cardNumber.Value.ToString() : "");
        }

        public static IEnumerable<MagicCardDefinition> CardDefinitions { get; private set; }

        public static IReadOnlyDictionary<string, MagicCardDefinition> CardDefinitionsByNameSetCode { get; private set; }

        public static IEnumerable<MagicSetDefinition> SetDefinitions { get; private set; }

        public static IReadOnlyDictionary<string, MagicCardDefinition> CardDefinitionsByCardId { get; private set; }

        public static IReadOnlyDictionary<string, MagicSetDefinition> SetDefinitionsBySetCode { get; private set; }

        public static IReadOnlyDictionary<string, MagicSetDefinition> SetDefinitionsBySetName { get; private set; }
    }
}