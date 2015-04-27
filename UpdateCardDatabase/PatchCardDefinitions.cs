using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using MyMagicCollection.Shared.Helper;
using MyMagicCollection.Shared.Models;

namespace UpdateCardDatabase
{
    public static class PatchCardDefinitions
    {
        private static string definitionCsv = @"

COMMENT, ----- DTK -----

COMMENT, ----- Fallen Empires -----
1985, Icatian Javelineers (Version 1)
1986, Icatian Javelineers (Version 2)
1987, Icatian Javelineers (Version 3)
";

        static PatchCardDefinitions()
        {
            var config = new CsvConfiguration()
            {
                Encoding = Encoding.UTF8,
                HasHeaderRecord = false,
                CultureInfo = CultureInfo.InvariantCulture,
            };

            PatchMkmCardDefinition = new Dictionary<string, string>();
            var assembly = typeof(PatchCardDefinitions).Assembly;
            var cardPatches = assembly.FindAllEmbeddedResource("cardpatches");

            foreach (var patchFile in cardPatches)
            {
                using (var stream = assembly.GetManifestResourceStream(patchFile))
                {
                    using (var inputCsv = new CsvReader(new StreamReader(stream), config))
                    {
                        while (inputCsv.Read())
                        {
                            var cardId = inputCsv.GetField<string>(0).Trim();
                            if (!cardId.Equals("comment", StringComparison.InvariantCultureIgnoreCase))
                            {
                                PatchMkmCardDefinition.Add(cardId, inputCsv.GetField<string>(1).Trim());
                            }
                        }
                    }
                }
            }
        }

        public static Dictionary<string, string> PatchMkmCardDefinition { get; private set; }
    }
}