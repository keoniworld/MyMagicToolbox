using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using MyMagicCollection.Shared.Models;
using NLog;

namespace MyMagicCollection.Shared.FileFormats.MyMagicCollection
{
    public class MyMagicCollectionCsv
    {
        private const string _delimiter = "----- CARDSDELIMITER -----";
        private readonly CsvConfiguration _config;

        public MyMagicCollectionCsv()
        {
            _config = new CsvConfiguration()
            {
                Encoding = Encoding.UTF8,
                HasHeaderRecord = true,
                CultureInfo = CultureInfo.InvariantCulture,
            };

            _config.RegisterClassMap(new MagicCollectionCsvMapper());
        }

        public void WriteFile(string fileName, MagicBinder collection)
        {
            if (File.Exists(fileName))
            {
                var dest = fileName + ".backup";
                File.Copy(fileName, dest, true);
            }

            using (var textWriter = new StreamWriter(fileName))
            {
                // Write header first:
                var writer = new CsvWriter(textWriter, _config);
                {
                    writer.Configuration.CultureInfo = CultureInfo.InvariantCulture;

                    writer.WriteField<string>("Name");
                    writer.WriteField<string>("Version");
                    writer.NextRecord();

                    writer.WriteField<string>(collection.Name);
                    writer.WriteField(collection.Version);
                    writer.NextRecord();

                    if (collection.Cards != null && collection.Cards.Any())
                    {
                        textWriter.WriteLine(_delimiter);

                        // Now write the cards
                        writer.WriteHeader<MagicBinderCard>();
                        writer.WriteRecords(collection.Cards);
                    }
                }
            }
        }

        public MagicBinder ReadFile(string fileName)
        {
            MagicBinder result = null;
            try
            {
                var content = File.ReadAllText(fileName);
                var delimiterPos = content.IndexOf(_delimiter, StringComparison.InvariantCultureIgnoreCase);
                string header = "";
                string cards = "";

                if (delimiterPos >= 0)
                {
                    // delimiter found -> split text
                    header = content.Substring(0, delimiterPos).Trim('\r', '\n');
                    cards = content.Substring(delimiterPos + _delimiter.Length, content.Length - _delimiter.Length - delimiterPos).Trim('\r', '\n');
                }
                else
                {
                    // no delimiter -> take all
                    header = content;
                }

                IList<MagicBinderCard> collectionCards = new List<MagicBinderCard>();
                if (!string.IsNullOrWhiteSpace(cards))
                {
                    using (var inputCsv = new CsvReader(new StringReader(cards), _config))
                    {
                        inputCsv.Configuration.CultureInfo = CultureInfo.InvariantCulture;
                        collectionCards = inputCsv.GetRecords<MagicBinderCard>().ToList();
                    }
                }

                result = new MagicBinder(Consolidate(collectionCards.Where(c => c.Quantity > 0 || c.QuantityTrade > 0 || c.QuantityWanted > 0)));

                using (var inputCsv = new CsvReader(new StringReader(header)))
                {
                    inputCsv.Configuration.CultureInfo = CultureInfo.InvariantCulture;

                    inputCsv.Read();
                    result.Name = inputCsv.GetField<string>("Name");
                    result.Version = inputCsv.GetField<int>("Version");
                }
            }
            catch (Exception error)
            {
                NotificationCenter.Instance.FireNotification(
                    LogLevel.Error,
                    nameof(MyMagicCollectionCsv) + "ReadFile() failed: " + error.Message);

                result = new MagicBinder();
            }

            return result;
        }

        public IList<MagicBinderCard> Consolidate(IEnumerable<MagicBinderCard> cards)
        {
            var result = new List<MagicBinderCard>();

            var grouped = cards.GroupBy(c => MakeConsolidateString(c));
            foreach (var group in grouped)
            {
                var first = group.First();
                first.Quantity = group.Sum(c=>c.Quantity);
                first.QuantityTrade = group.Sum(c => c.QuantityTrade);
                first.QuantityWanted= group.Sum(c => c.QuantityWanted);

                result.Add(first);
            }

            return result;

        }

        public string MakeConsolidateString(MagicBinderCard card)
        {
            return card.IsFoil.ToString() + card.CardId + card.Language;
        }
    }
}