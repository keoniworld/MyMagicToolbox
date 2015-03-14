using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using MyMagicCollection.Shared.Price;

namespace MyMagicCollection.Shared.Models
{
    public static class StaticPriceDatabase
    {
        private const string _priceDatabaseName = "PriceCache.csv";

        private static FileInfo _fileName;

        private static object _sync = new object();

        private static INotificationCenter _notificationCenter = NotificationCenter.Instance;

        static StaticPriceDatabase()
        {
            _fileName = new FileInfo(Path.Combine(PathHelper.UserDataFolder, _priceDatabaseName));
            Read();
        }

        public static ConcurrentDictionary<string, MagicCardPrice> PriceCache { get; private set; }

        public static void Write()
        {
            lock (_sync)
            {
                using (var outputCsv = new CsvWriter(File.CreateText(_fileName.FullName)))
                {
                    outputCsv.Configuration.CultureInfo = CultureInfo.InvariantCulture;
                    outputCsv.WriteRecords(PriceCache.Values);
                }
            }
        }

        public static MagicCardPrice FindPrice(IMagicCardDefinition definition, bool autoUpdate, bool saveDatabase)
        {
            MagicCardPrice price = null;

            if (!PriceCache.TryGetValue(definition.CardId, out price))
            {
                price = new MagicCardPrice
                {
                    CardId = definition.CardId,
                };

                PriceCache.TryAdd(definition.CardId, price);
            }

            if (autoUpdate)
            {
                Task.Factory.StartNew(() => UpdatePrice(definition, price, saveDatabase));
            }

            if (!autoUpdate && saveDatabase)
            {
                Write();
            }

            return price;
        }

        public static void UpdatePrice(IMagicCardDefinition definition, MagicCardPrice price, bool autoWrite)
        {
            try
            {
                lock (_sync)
                {
                    if (price.UpdateUtc.HasValue && price.UpdateUtc.Value.Date >= DateTime.UtcNow.Date)
                    {
                        return;
                    }

                    var request = new CardPriceRequest(_notificationCenter);
                    request.PerformRequest(definition, price);

                    if (autoWrite)
                    {
                        Write();
                    }
                }
            }
            catch (Exception error)
            {
                _notificationCenter.FireNotification(
                    null,
                    string.Format("Error getting price for {0}({1}): {2}", definition.NameEN, definition.SetCode, error.Message));
            }
        }

        private static void Read()
        {
            if (_fileName.Exists)
            {
                try
                {
                    var content = File.ReadAllText(_fileName.FullName);
                    using (var inputCsv = new CsvReader(new StringReader(content)))
                    {
                        inputCsv.Configuration.CultureInfo = CultureInfo.InvariantCulture;

                        PriceCache = new ConcurrentDictionary<string, MagicCardPrice>(
                            inputCsv.GetRecords<MagicCardPrice>().ToDictionary(c => c.CardId));
                    }
                }
                catch (Exception error)
                {
                    _notificationCenter.FireNotification(
                        null,
                        string.Format("Error loading price database: {0}", error.Message));

                    PriceCache = new ConcurrentDictionary<string, MagicCardPrice>();
                }
            }
            else
            {
                PriceCache = new ConcurrentDictionary<string, MagicCardPrice>();
            }
        }
    }
}