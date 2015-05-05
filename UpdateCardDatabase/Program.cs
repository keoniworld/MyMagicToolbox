using CsvHelper;
using CsvHelper.Configuration;
using MyMagicCollection.Shared;
using MyMagicCollection.Shared.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace UpdateCardDatabase
{
    public class Program
    {
        public static DateTime? ParseReleaseDate(string releaseDate)
        {
            if (string.IsNullOrWhiteSpace(releaseDate))
            {
                return null;
            }

            var parts = releaseDate.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            return new DateTime(
                int.Parse(parts[0]),
                parts.Length >= 2 ? int.Parse(parts[1]) : 12,
                parts.Length >= 3 ? int.Parse(parts[2]) : 1);
        }

        private static void Main(string[] args)
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
            var inputFiles = Directory.EnumerateFiles(exeFolder, "AllSets-x.json", SearchOption.AllDirectories).ToList();

            var textWriter = new StreamWriter(exportFileName);

            var config = new CsvConfiguration()
            {
                Encoding = Encoding.UTF8,
                HasHeaderRecord = true,
                CultureInfo = CultureInfo.InvariantCulture,
            };

            // config.RegisterClassMap(new MagicCardDefinitionCsvMapper());

            var availableSets = new Dictionary<string, MagicSetDefinition>();
            var availableCards = new List<MagicCardDefinition>();

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

            foreach (var inputCsvName in inputFiles)
            {
                Console.WriteLine("Reading file " + new FileInfo(inputCsvName).Name);
                var jsonContent = File.ReadAllText(inputCsvName);
                var allCardsAndSets = JObject.Parse(jsonContent);

                var numberOfCards = allCardsAndSets.Count;

                foreach (var setAndCards in allCardsAndSets)
                {
                    var casted = setAndCards.Value as JObject;
                    if (casted != null)
                    {
                        var block = casted.GetValue("block");
                        var mkmCode = casted.GetValue("magicCardsInfoCode");
                        var code = casted.GetValue("code").ToString();
                        var releaseDate = casted.GetValue("releaseDate");

                        var setData = new MagicSetDefinition
                        {
                            Code = code,
                            CodeMagicCardsInfo = mkmCode != null ? mkmCode.ToString() : code,
                            Name = CardDatabaseHelper.PatchSetName(casted.GetValue("name").ToString()),
                            Block = block != null ? block.ToString() : null,
                            ReleaseDate = releaseDate != null ? releaseDate.ToString() : null,
                        };

                        availableSets.Add(setData.Code, setData);
                        Console.WriteLine("Working set " + setData.Name + "...");

                        var cards = casted.GetValue("cards");
                        foreach (var card in cards.Cast<JObject>())
                        {
                            var cardName = card.GetValue("name").ToString().Trim();

                            var multiverseId = card.GetValue("multiverseid");
                            if (multiverseId == null)
                            {
                                continue;
                                // multiverseId = setData.Code + "_" + cardName;
                            }

                            var rulesText = card.GetValue("text");
                            var manaCost = card.GetValue("manaCost");
                            var convertedManaCost = card.GetValue("cmc");
                            var rarity = card.GetValue("rarity");
                            var numberInSet = card.GetValue("number");

                            // TODO: Card Type

                            var cardDefinition = new MagicCardDefinition
                            {
                                CardId = multiverseId != null ? multiverseId.ToString() : "",
                                NameEN = cardName,
                                CardType = card.GetValue("type").ToString(),
                                RulesText = rulesText != null ? rulesText.ToString() : null,
                                ManaCost = manaCost != null ? manaCost.ToString() : null,
                                ConvertedManaCost = convertedManaCost != null ? CardDatabaseHelper.ComputeConvertedManaCost(convertedManaCost.ToString()) : (int?)null,
                                SetCode = setData.Code,
                                NumberInSet = numberInSet != null ? numberInSet.ToString() : null,
                                Rarity = CardDatabaseHelper.ComputeRarity(rarity != null ? rarity.ToString() : null),
                            };

                            // Patch special names for MKM (land versions, etc.)
                            string mkmName = null;
                            if (PatchCardDefinitions.PatchMkmCardDefinition.TryGetValue(cardDefinition.CardId, out mkmName))
                            {
                                cardDefinition.NameMkm = mkmName.Trim();
                            }
                            else
                            {
                                cardDefinition.NameMkm = cardDefinition.NameEN;
                            }

                            var legalities = card.GetValue("legalities") as JObject;
                            if (legalities != null)
                            {
                                foreach (var legality in legalities.Cast<JProperty>().ToList())
                                {
                                    var legalityName = legality.Name.ToLowerInvariant();

                                    if (legalityName.EndsWith(" block"))
                                    {
                                        continue;
                                    }

                                    switch (legalityName)
                                    {
                                        case "modern":
                                            cardDefinition.LegalityModern = true;
                                            break;

                                        case "standard":
                                            cardDefinition.LegalityStandard = true;
                                            break;

                                        case "pauper":
                                            cardDefinition.LegalityPauper = true;
                                            break;

                                        case "legacy":
                                            cardDefinition.LegalityLegacy = true;
                                            break;

                                        case "commander":
                                            cardDefinition.LegalityCommander = true;
                                            break;

                                        case "vintage":
                                            cardDefinition.LegalityVintage = true;
                                            break;

                                        case "singleton 100":
                                        case "freeform":
                                        case "prismatic":
                                        case "time spiral block":
                                        case "tribal wars legacy":
                                            // known but unsupported
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }

                            availableCards.Add(cardDefinition);

                            //Console.WriteLine("Working card " + cardDefinition.NameEN + "...");
                        }
                    }
                }

            
            }

            // Add Tokens
            availableCards.AddRange(TokenDefinitions.TockenDefinition);

            // Find duplicates
            var duplicates = availableCards.GroupBy(c => c.CardId).Where(g => g.Count() > 1).ToList();
            foreach (var duplicate in duplicates)
            {
                Console.WriteLine("DUPLICATE CARD! " + duplicate.Count() + " " + duplicate.First().NameEN);

                foreach (var item in duplicate)
                {
                    item.CardId = item.CardId + "_" + item.NumberInSet;
                }
            }

            duplicates = availableCards.GroupBy(c => c.CardId).Where(g => g.Count() > 1).ToList();
            foreach (var duplicate in duplicates)
            {
                Console.WriteLine("DUPLICATE CARD AFTER FIX! " + duplicate.Count() + " " + duplicate.First().NameEN);

                //foreach (var item in duplicate)
                //{
                //    item.CardId = item.CardId + "_" + item.NumberInSet;
                //}
            }

            // Add Version string to cards:
            CardDatabaseHelper.PatchPotentialVersionCardData(availableCards);

            // Write cards
            writer.WriteRecords(availableCards.OrderBy(c => c.SetCode).ThenBy(c => c.NameEN).ToList());

            // Write Sets
            var cardsBySet = availableCards.GroupBy(c => c.SetCode).ToList();
            foreach (var set in availableSets.OrderBy(s => s.Key))
            {
                if (!cardsBySet.Any(c => c.Key == set.Key))
                {
                    continue;
                }

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

            // Dump potential cards to be fixed:
            var dump = CardDatabaseHelper.GetPotentialVersionCardData(availableCards);
            Console.WriteLine(dump);
            Debug.WriteLine(dump);
        }
    }
}