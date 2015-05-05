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
    public static class TokenDefinitions
    {
        static TokenDefinitions()
        {
            var config = new CsvConfiguration()
            {
                Encoding = Encoding.UTF8,
                HasHeaderRecord = false,
                CultureInfo = CultureInfo.InvariantCulture,
            };

            TockenDefinition = new List<MagicCardDefinition>();
            var assembly = typeof(PatchCardDefinitions).Assembly;
            var tokenDefinitions = assembly.FindAllEmbeddedResource("TokenDefinitions");

            foreach (var patchFile in tokenDefinitions)
            {
                using (var stream = assembly.GetManifestResourceStream(patchFile))
                {
                    using (var inputCsv = new CsvReader(new StreamReader(stream), config))
                    {
                        while (inputCsv.Read())
                        {
                            var setCode = inputCsv.GetField<string>(0).Trim();
                            if (setCode == "COMMENT")
                            {
                                continue;
                            }

                            var cardDefinition = new MagicCardDefinition()
                            {
                                SetCode = setCode,
                                NumberInSet = inputCsv.GetField<string>(1),
                                NameEN = inputCsv.GetField<string>(2).Trim(),
                                NameDE = inputCsv.GetField<string>(3).Trim(),
                                NameMkm = inputCsv.GetField<string>(4).Trim(),

                                MagicCardType = MagicCardType.Token,
                            };

                            cardDefinition.CardId = string.Format(
                                CultureInfo.InvariantCulture,
                                "{0}_TOKEN_{1}",
                                cardDefinition.SetCode,
                                cardDefinition.NumberInSet);

                            TockenDefinition.Add(cardDefinition);
                        }
                    }
                }
            }
        }

        public static List<MagicCardDefinition> TockenDefinition { get; }
    }
}