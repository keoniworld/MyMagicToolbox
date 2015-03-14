using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Dapper;
using MagicDatabase;
using MyMagicCollection.Shared.Models;

namespace UpdateCardDatabase
{
    public class Program
    {
        public static bool? ComputeLegality(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            switch (input.ToLowerInvariant())
            {
                case "v":
                    return true;

                case "b":
                    return false;
            }

            return false;
        }

        public static MagicRarity? ComputeRarity(string input)
        {
            switch (input.ToLowerInvariant())
            {
                case "l":
                    return MagicRarity.Land;

                case "t":
                case "t // t":
                    return MagicRarity.Token;

                case "u":
                case "u // u":
                    return MagicRarity.Uncommon;

                case "c":
                case "c // c":
                    return MagicRarity.Common;

                case "m":
                case "m // m":
                    return MagicRarity.Mythic;

                case "r":
                case "r // r":
                    return MagicRarity.Rare;

                case "":
                    return null;
            }

            throw new InvalidOperationException("Invalid rarity " + input);
        }

        public static string DeEscape(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            return input.Replace("£", Environment.NewLine);
        }

        public static int? SafeGetInt(CsvReader inputCsv, string columnName)
        {
            try
            {
                return inputCsv.GetField<int?>("number_int");
            }
            catch (Exception error)
            {
                Console.WriteLine("Error reading " + columnName + " column: " + error.Message);
            }

            return null;
        }

        public static string PatchSetCode(string setCode)
        {
            var patch = new Dictionary<string, string>()
            {
                { "A", "LEA" },
                { "B", "LEB" },
                { "U", "2ED" },
                { "R", "3ED" },
                { "4E", "4ED" },
                { "5E", "5ED" },
                { "6E", "6ED" },
                { "7E", "7ED" },
                { "8E", "8ED" },
                { "9E", "9ED" },
                { "LG", "LEG" },

                { "CFX", "CON" },
                { "ADVD", "DDC" },
                { "AGVL", "DDD" },
                { "AJVC", "DD2" },
                { "AVB", "DDH" },
                { "ANH", "V14" },
                { "CS", "CSP" },
                { "DRG", "DRP" },
                { "DS", "DST" },
                { "GP", "GPT" },
                { "HL", "HML" },
                { "AEVD", "DD3" },

                { "ANT", "ATH" },
                { "CRS", "CM1" },
                { "TO", "TOR" },
                { "UD", "UDS" },
                { "UL", "ULG" },
                { "US", "USG" },
                { "PT", "POR" },
                { "ON", "ONS" },
                { "MR", "MRD" },
                { "DPA", "DRP" },
                { "FD", "5DN" },
                { "HVM", "DDL" },
                { "TWE", "V13" },
                { "VVK", "DDI" },
           };

            string found;
            if (patch.TryGetValue(setCode, out found))
            {
                return found;
            }

            return setCode;
        }

        public static string PatchSetCodeMagicCardsInfo(string setCode)
        {
            var patch = new Dictionary<string, string>()
            {
                { "4ED", "4E" },
                { "5ED", "5E" },
                { "6ED", "6E" },
                { "7ED", "7E" },
                { "8ED", "8E" },
                { "9ED", "9E" },
                { "ONS", "ON" },
                { "MRD", "MR" },

                { "15A", "15ANN" },
                { "AL", "AI" },
                { "ATH", "AT" },
                { "ARS", "ARCS" },
                { "ARE", "ARENA" },
                { "CC", "UQC" },
                { "CHA", "CP" },
                { "CST", "CSTD" },
                { "CM1", "CMA" },
                { "DIS", "DI" },
                { "ADVD", "DD3" },
                { "AEVG", "DD3" },
                { "AGVL", "DD3" },
                { "AJVC", "DD3" },
                { "AVB", "DDH" },
                { "EVT", "DDF" },
                { "IVG", "DDJ" },
                { "JVV", "DDM" },
                { "KVD", "DDG" },
                { "SVT", "DDK" },
                { "SVC", "DDN" },
                { "VVK", "DDI" },
                { "EUR", "EURO" },
                { "FNM", "FNMP" },
                { "ANH", "V14" },
                { "DRG", "FVD" },
                { "EXL", "FVE" },
                { "LEG", "FVL" },
                { "RLM", "V12" },
                { "RLC", "FVR" },
                { "TWE", "V13" },
                { "GUR", "GURU" },
                { "JCG", "JR" },
                { "LGM", "DCILM" },
                { "A", "AL" },
                { "B", "BE" },
                { "LRW", "LW" },
                { "GDC", "MGDC" },
                { "REW", "MPRP" },
                { "GLP", "MLP" },
                { "CNSC", "CNS" },
                { "ME", "MED" },
                { "MI", "MR" },
                { "MOR", "MT" },
                { "PLC", "PC" },
                { "PCP", "PCHP" },
                { "POR", "PO" },
                { "P2", "PO2" },
                { "P3", "P3K" },
                { "FAL", "FD2" },
                { "GRV", "PD3" },
                { "SLI", "PDS" },
                { "PRE", "PTC" },
                { "PY", "PR" },
                { "RLS", "REP" },
                { "R", "RV" },
                { "S2", "ST2K" },
                { "SS", "SUS" },
                { "TE", "TP" },
                { "TSP", "TS" },
                { "TSB", "TSTS" },
                { "TOR", "TR" },
                { "2HG", "THGT" },
                { "UNH", "UH" },
                { "U", "UN" },
                { "WCQ", "WMCQ" },
                { "GTW", "GRC" },
                { "GPT", "GP" },
                { "UDS", "UD" },
                { "ULG", "UL" },
                { "USG", "US" },
                { "DRP", "DPA" },
            };

            string found;
            if (patch.TryGetValue(setCode, out found))
            {
                return found;
            }

            return setCode;
        }

        public static bool IsSetIncluded(string setCode)
        {
            switch (setCode)
            {
                case "15A":
                case "CNSC":
                case "CHA":
                case "8EB":
                case "9EB":
                case "2HG":
                case "FAL":
                case "PRO":
                case "WRL":
                case "WCQ":
                case "ST":
                case "S2":
                case "SS":
                case "SLI":
                case "APAC":
                case "ARE":
                case "ARS":
                case "CC":
                case "CST":
                case "EUR":
                case "MBP":
                case "LGM":
                case "ME":
                case "GUR":
                case "HHO":
                case "ATH":
                case "DPA":
                    // case "TSB":
                    return false;

                default:
                    return true;
            }
        }

        public static string PatchSetName(string setCode)
        {
            switch (setCode)
            {
                case "Magic: The Gathering—Conspiracy":
                    return "Conspiracy";

                case "Magic: The Gathering-Commander":
                    return "Commander";

                case "Magic 2014 Core Set":
                    return "Magic 2014";

                case "Magic 2015 Core Set":
                    return "Magic 2015";

                case "Classic Sixth Edition":
                    return "Sixth Edition";

                case "Prerelease Events":
                    return "Prerelease Promos";

                case "From the Vault: Annihilation (2014)":
                    return "From the Vault: Annihilation";
            }

            return setCode;
        }

        private static void Main(string[] args)
        {
            var provider = new CardDatabaseFolderProvider();

            var exportFileName = Path.Combine(provider.ExeFolder, "CSV", "MagicDatabase.csv");
            if (File.Exists(exportFileName))
            {
                File.Delete(exportFileName);
            }

            var exportSetFileName = Path.Combine(provider.ExeFolder, "CSV", "MagicDatabaseSets.csv");
            if (File.Exists(exportSetFileName))
            {
                File.Delete(exportSetFileName);
            }

            int count = 0;
            var inputFiles = Directory.EnumerateFiles(provider.ExeFolder, "*.csv", SearchOption.AllDirectories).ToList();

            var textWriter = new StreamWriter(exportFileName);

            var config = new CsvConfiguration()
            {
                Encoding = Encoding.UTF8,
                HasHeaderRecord = true,
                CultureInfo = CultureInfo.InvariantCulture,
            };

            var availableSets = new Dictionary<string, MagicSetDefinition>();

            var writer = new CsvWriter(textWriter, config);
            writer.WriteHeader(typeof(MagicCardDefinition));

            var setWriter = new CsvWriter(new StreamWriter(exportSetFileName), config);
            setWriter.WriteHeader<MagicSetDefinition>();

            var uniqueList = new Dictionary<string, string>();

            foreach (var inputCsvName in inputFiles)
            {
                Console.WriteLine("Reading file " + new FileInfo(inputCsvName).Name);
                var inputCsv = new CsvReader(new StringReader(File.ReadAllText(inputCsvName)));
                inputCsv.Configuration.Delimiter = "||";

                while (inputCsv.Read())
                {
                    count += 1;

                    var card = new MagicCardDefinition();
                    card.Id = count;
                    card.NameDE = inputCsv.GetField<string>("name_DE");
                    card.NameEN = inputCsv.GetField<string>("name");
                    card.SetCode = PatchSetCode(inputCsv.GetField<string>("set_code"));
                    card.CardId = inputCsv.GetField<string>("id");
                    card.CardType = inputCsv.GetField<string>("type");
                    card.NumberInSet = SafeGetInt(inputCsv, "number_int");
                    card.LegalityModern = ComputeLegality(inputCsv.GetField<string>("legality_Modern"));
                    card.LegalityStandard = ComputeLegality(inputCsv.GetField<string>("legality_Standard"));
                    card.LegalityLegacy = ComputeLegality(inputCsv.GetField<string>("legality_Legacy"));
                    card.LegalityVintage = ComputeLegality(inputCsv.GetField<string>("legality_Vintage"));
                    card.LegalityPauper = ComputeLegality(inputCsv.GetField<string>("legality_Pauper"));
                    card.LegalityCommander = ComputeLegality(inputCsv.GetField<string>("legality_Commander"));
                    card.LegalityFrenchCommander = ComputeLegality(inputCsv.GetField<string>("legality_French_Commander"));
                    card.Rarity = ComputeRarity(inputCsv.GetField<string>("rarity"));

                    if (!IsSetIncluded(card.SetCode))
                    {
                        continue;
                    }

                    card.RulesText = inputCsv.GetField<string>("ability");
                    // .Replace("£", Environment.NewLine);

                    card.RulesTextDE = inputCsv.GetField<string>("ability_DE");
                    // .Replace("£", Environment.NewLine);

                    var setName = PatchSetName(inputCsv.GetField<string>("set"));
                    if (!string.IsNullOrWhiteSpace(setName) && !availableSets.ContainsKey(card.SetCode))
                    {
                        var definition = new MagicSetDefinition
                        {
                            Name = setName,
                            Code = card.SetCode,
                            CodeMagicCardsInfo = PatchSetCodeMagicCardsInfo(card.SetCode),
                        };

                        availableSets.Add(card.SetCode, definition);
                    }

                    var unique = StaticMagicData.MakeNameSetCode(card.SetCode, card.NameEN, card.NumberInSet);
                    if (uniqueList.ContainsKey(unique))
                    {
                        // Ignore variants of
                        continue;
                    }

                    uniqueList.Add(unique, unique);

                    writer.WriteRecord<MagicCardDefinition>(card);

                    Console.WriteLine(count + " Reading " + card.NameEN + "(" + card.SetCode + ")...");

                    ////var found = connection.Query<Card>(
                    ////    "Select * from MagicCard where Id = @Id",
                    ////    new { card.Id});

                    // connection.Query(insertStatement, card);

                    // if (count % 100==0) break;
                }

                // Write Sets
                foreach (var set in availableSets.OrderBy(s => s.Key))
                {
                    setWriter.WriteRecord(set.Value);
                }

                inputCsv.Dispose();
                writer.Dispose();
                setWriter.Dispose();
            }
        }
    }
}