﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using MyMagicCollection.Shared;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.CSV;

namespace UpdateCardDatabase
{
    public class ProgramOld
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
                            
                { "PCH", "HOP" },

                { "CFX", "CON" },
                { "ADVD", "DDC" },
                // { "AGVL", "DDD" },
                { "AVB", "DDH" },
                { "ANH", "V14" },
                
                { "DRG", "DRP" },
                { "DS", "DST" },
                { "GP", "GPT" },
                
                // { "AEVD", "DD3" },
                { "SVC", "DDN" },

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
                { "SH", "STH" },
                { "PY", "PCY" },
                { "SC", "SCG" },
                { "JU", "JUD" },
                { "SVT", "DDK" },
                { "RLM", "V12" },
                { "PVC", "DDE" },
                { "KVD", "DDG" },
                { "JVV", "DDM" },
                { "JVC", "DD2" },
                { "GVL", "DDD" },
                { "IVG", "DDJ" },
                { "RLC", "V10" },
                { "EVT", "DDF" },
                { "DVD", "DDC" },
                { "GRV", "PD3" },
                { "DM", "DKM" },
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
                { "ONS", "ON" },
                { "MRD", "MI" },
                { "JUD", "JU" },
                { "DDE", "PVC" },
                { "DDD", "GVL" },
                { "HOP", "PCH" },
                { "DDC", "DVD" },
                { "DKM", "DM" },
                { "PCY", "PR" },

                { "15A", "15ANN" },
                
                { "ATH", "AT" },
                { "ARS", "ARCS" },
                { "ARE", "ARENA" },
                { "CC", "UQC" },
                { "CHA", "CP" },
                { "CST", "CSTD" },
                { "CM1", "CMA" },
                { "DIS", "DI" },
                //{ "ADVD", "DD3" },
                //{ "AEVG", "DD3" },
                //{ "AGVL", "DD3" },
                //{ "AJVC", "DD3" },
                { "AVB", "DDH" },

                { "VVK", "DDI" },
                { "EUR", "EURO" },
                { "FNM", "FNMP" },
                { "ANH", "V14" },
                { "DRG", "FVD" },
                { "EXL", "FVE" },
                // { "V11", "FVL" },
                { "V10", "FVR" },
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
                { "DST", "DS" },
                { "CON", "CFX" },
                
                { "STH", "SH" },
                { "SCG", "SC" },
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
                case "SUM":
                case "GPX":

                // Anthology Duel Decks
                case "ADVD":
                case "AEVD":
                case "AEVG":
                case "AGVL":
                case "AJVC":
                case "DDC":

                    // case "TSB":
                    return false;

                default:
                    return true;
            }
        }

       

        public static MagicCardType GetCardType(string cardType)
        {
            // TODO: Implement this
            return MagicCardType.Unknown;
        }

        private static void MainOld(string[] args)
        {
            var exeFolder = PathHelper.ExeFolder;
            MagicSetDefinition lastSet = null;

            var relativeToSource = @"..\..\..\MyMagicCollection.Shared";

            var exportFileName = Path.Combine(exeFolder, relativeToSource, "CSV", "MagicDatabase.csv");
            if (File.Exists(exportFileName))
            {
                File.Delete(exportFileName);
            }

            var exportSetFileName = Path.Combine(exeFolder, relativeToSource, "CSV", "MagicDatabaseSets.csv");
            if (File.Exists(exportSetFileName))
            {
                File.Delete(exportSetFileName);
            }

            int count = 0;
            var inputFiles = Directory.EnumerateFiles(exeFolder, "set_*.csv", SearchOption.AllDirectories).ToList();

            var textWriter = new StreamWriter(exportFileName);

            var config = new CsvConfiguration()
            {
                Encoding = Encoding.UTF8,
                HasHeaderRecord = true,
                CultureInfo = CultureInfo.InvariantCulture,
            };

			// config.RegisterClassMap(new MagicCardDefinitionCsvMapper());

			var availableSets = new Dictionary<string, MagicSetDefinition>();

            var writer = new CsvWriter(textWriter, config);
            writer.WriteHeader<MagicCardDefinition>();

            var setWriter = new CsvWriter(new StreamWriter(exportSetFileName), config);
            setWriter.WriteField<string>("Code");
            setWriter.WriteField<string>("Name");
            setWriter.WriteField<string>("CodeMagicCardsInfo");
            setWriter.WriteField<string>("ReleaseDate");
            setWriter.WriteField<string>("Block");
            setWriter.WriteField<string>("IsPromoEdition");
            setWriter.NextRecord();

            var uniqueList = new Dictionary<string, string>();

            // Write Tokens:
            writer.WriteRecords(TokenDefinitions.TockenDefinition);

            foreach (var inputCsvName in inputFiles)
            {
                Console.WriteLine("Reading file " + new FileInfo(inputCsvName).Name);
                var inputCsv = new CsvReader(new StringReader(File.ReadAllText(inputCsvName)));
                inputCsv.Configuration.Delimiter = "||";

                while (inputCsv.Read())
                {
                    count += 1;

                    var setCode = PatchSetCode(inputCsv.GetField<string>("set_code"));

                    var card = new MagicCardDefinition();
                    card.NameDE = inputCsv.GetField<string>("name_DE");
                    card.NameEN = inputCsv.GetField<string>("name");

                    card.CardId = inputCsv.GetField<string>("id");
                    card.CardType = inputCsv.GetField<string>("type");
                    card.MagicCardType = GetCardType(card.CardType);
                    card.NumberInSet = SafeGetInt(inputCsv, "number_int").ToString();
                    card.LegalityModern = ComputeLegality(inputCsv.GetField<string>("legality_Modern"));
                    card.LegalityStandard = ComputeLegality(inputCsv.GetField<string>("legality_Standard"));
                    card.LegalityLegacy = ComputeLegality(inputCsv.GetField<string>("legality_Legacy"));
                    card.LegalityVintage = ComputeLegality(inputCsv.GetField<string>("legality_Vintage"));
                    card.LegalityPauper = ComputeLegality(inputCsv.GetField<string>("legality_Pauper"));
                    card.LegalityCommander = ComputeLegality(inputCsv.GetField<string>("legality_Commander"));
                    card.LegalityFrenchCommander = ComputeLegality(inputCsv.GetField<string>("legality_French_Commander"));
                    card.Rarity = CardDatabaseHelper.ComputeRarity(inputCsv.GetField<string>("rarity"));
                    card.ManaCost = inputCsv.GetField<string>("manacost");
                    card.ConvertedManaCost = CardDatabaseHelper.ComputeConvertedManaCost(inputCsv.GetField<string>("converted_manacost"));

                    if (!IsSetIncluded(setCode))
                    {
                        continue;
                    }

					// TODO: Add Patch for MKM Name (multiple versions, etc.)
					string mkmName = null;
					if (PatchCardDefinitions.PatchMkmCardDefinition.TryGetValue(card.CardId, out mkmName))
					{
						card.NameMkm = mkmName;
					}
					//else
					//{
					//	card.NameMkm = card.NameEN;
					//}

					card.RulesText = inputCsv.GetField<string>("ability");
                    // .Replace("£", Environment.NewLine);

                    card.RulesTextDE = inputCsv.GetField<string>("ability_DE");
                    // .Replace("£", Environment.NewLine);

                    var setName = CardDatabaseHelper.PatchSetName(inputCsv.GetField<string>("set"));
                    if (!string.IsNullOrWhiteSpace(setName) && !availableSets.ContainsKey(setCode))
                    {
                        MagicSetDefinition blockData;
                        if (SetDefinitions.BlockDefinition.TryGetValue(setCode, out blockData))
                        {
                            availableSets.Add(setCode, blockData);
                        }
                        else
                        {
                            var definition = new MagicSetDefinition
                            {
                                Name = setName,
                                Code = setCode,
                                CodeMagicCardsInfo = PatchSetCodeMagicCardsInfo(setCode),
                            };

                            availableSets.Add(setCode, definition);
                        }
                    }

                    if (availableSets.TryGetValue(setCode, out lastSet))
                    {
                        card.SetCode = lastSet.Code;
                    }
                    else
                    {
                        card.SetCode = setCode;
                    }

                    var unique = StaticMagicData.MakeNameSetCode(setCode, card.NameEN, card.NumberInSet);
                    if (uniqueList.ContainsKey(unique))
                    {
                        // Ignore variants of
                        continue;
                    }

                    uniqueList.Add(unique, unique);

                    writer.WriteRecord(card);

                    Console.WriteLine(count + " Reading " + card.NameEN + "(" + card.SetCode + ")...");
                }

                inputCsv.Dispose();
            }

            // Write Sets
            foreach (var set in availableSets.OrderBy(s => s.Key))
            {
                // setWriter.WriteRecord(set.Value);
                setWriter.WriteField<string>(set.Value.Code);
                setWriter.WriteField<string>(set.Value.Name);
                setWriter.WriteField<string>(set.Value.CodeMagicCardsInfo);
                setWriter.WriteField<string>(set.Value.ReleaseDate);
                setWriter.WriteField<string>(set.Value.Block);
                setWriter.WriteField<bool>(set.Value.IsPromoEdition);
                setWriter.NextRecord();
            }

            writer.Dispose();
            setWriter.Dispose();
        }
    }
}