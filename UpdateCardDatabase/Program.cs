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
using MagicLibrary;
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

        private static void Main(string[] args)
        {
            var provider = new CardDatabaseFolderProvider();

            var database = new CardDatabase(provider);

            var connection = database.SimpleDbConnection();
            connection.Open();

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

            connection.Query("delete from MagicCard");
            int count = 0;
            var inputFiles = Directory.EnumerateFiles(provider.ExeFolder, "*.csv", SearchOption.AllDirectories).ToList();

            var propertyList = typeof(MagicCardDefinition).GetPropertyNames(null);
            var insertStatement = string.Format(
                "INSERT INTO MagicCard ({0}) VALUES ({1});",
                string.Join(", ", propertyList),
                string.Join(", ", propertyList.Select(p => "@" + p)));

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
                    card.SetCode = inputCsv.GetField<string>("set_code");
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

                    card.RulesText = inputCsv.GetField<string>("ability");
                    // .Replace("£", Environment.NewLine);

                    card.RulesTextDE = inputCsv.GetField<string>("ability_DE");
                    // .Replace("£", Environment.NewLine);



                  

                    var setName = inputCsv.GetField<string>("set")
                        .Replace("Magic: The Gathering—Conspiracy", "Conspiracy")
                        .Replace("Magic: The Gathering-Commander", "Commander");

                    if (!string.IsNullOrWhiteSpace(setName) && !availableSets.ContainsKey(card.SetCode))
                    {
                        var definition = new MagicSetDefinition { Name = setName, Code = card.SetCode };
                        availableSets.Add(card.SetCode, definition);
                        setWriter.WriteRecord(definition);
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

                inputCsv.Dispose();
                writer.Dispose();
                setWriter.Dispose();
            }

            Console.WriteLine("Cleaning database...");
            connection.Query("VACUUM");
            connection.Dispose();
        }
    }
}