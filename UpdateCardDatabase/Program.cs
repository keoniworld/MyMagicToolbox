using System;
using System.IO;
using System.Linq;
using CsvHelper;
using Dapper;
using MagicLibrary;
using MagicDatabase;

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

            connection.Query("delete from MagicCard");
            int count = 0;
            var inputFiles = Directory.EnumerateFiles(provider.ExeFolder, "*.csv", SearchOption.AllDirectories).ToList();

            var propertyList = typeof(Card).GetPropertyNames(null);
            var insertStatement = string.Format(
                "INSERT INTO MagicCard ({0}) VALUES ({1});",
                string.Join(", ", propertyList),
                string.Join(", ", propertyList.Select(p => "@" + p)));

            foreach (var inputCsvName in inputFiles)
            {
                Console.WriteLine("Reading file " + new FileInfo(inputCsvName).Name);
                var inputCsv = new CsvReader(new StringReader(File.ReadAllText(inputCsvName)));
                inputCsv.Configuration.Delimiter = "||";

                while (inputCsv.Read())
                {
                    count += 1;

                    var card = new Card();
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

                    card.RulesText = inputCsv.GetField<string>("ability")
                        .Replace("£", Environment.NewLine);

                    card.RulesTextDE = inputCsv.GetField<string>("ability_DE")
                        .Replace("£", Environment.NewLine);

                    Console.WriteLine(count + " Reading " + card.NameEN + "(" + card.SetCode + ")...");

                    ////var found = connection.Query<Card>(
                    ////    "Select * from MagicCard where Id = @Id",
                    ////    new { card.Id});

                    connection.Query(insertStatement, card);

                    // if (count % 100==0) break;
                }

                inputCsv.Dispose();
            }

            Console.WriteLine("Cleaning database...");
            connection.Query("VACUUM");
            connection.Dispose();
        }
    }
}