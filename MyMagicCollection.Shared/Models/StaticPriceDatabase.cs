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
using NLog;
using System.Diagnostics;

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

        public static MagicCardPrice FindPrice(
            IMagicCardDefinition definition, 
            bool autoUpdate, 
            bool saveDatabase,
            string additionalLogText,
			bool forcePriceUpdate)
        {
            MagicCardPrice price = null;
            if (definition == null)
            {
                return price;
            }

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
                Task.Factory.StartNew(() => UpdatePrice(definition, price, saveDatabase, additionalLogText, forcePriceUpdate));
            }

            if (!autoUpdate && saveDatabase)
            {
                Write();
            }

            return price;
        }

        public static void ClearImageData(INotificationCenter notificationCenter)
        {
            var watch = Stopwatch.StartNew();
            notificationCenter.FireNotification(LogLevel.Debug, "Resetting image paths");
            foreach (var item in PriceCache.Values.ToList())
            {
                item.ImagePath = "";
            }

            notificationCenter.FireNotification(LogLevel.Debug, "Flush image cache");
            Write();

            foreach (var file in Directory.EnumerateFiles(PathHelper.CardImageCacheFolder, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    notificationCenter.FireNotification(LogLevel.Debug, "Deleting file " + file);
                    File.Delete(file);
                }
                catch (Exception error)
                {
                    notificationCenter.FireNotification(LogLevel.Error, "Error deleting file '" + file + "':" + error.Message);
                }
            }

            watch.Stop();
            notificationCenter.FireNotification(LogLevel.Debug, "Clear image data took " + watch.Elapsed);
        }

        public static void UpdatePrice(
			IMagicCardDefinition definition, 
			MagicCardPrice price, 
			bool autoWrite, 
			string additionalLogText,
			bool forcePriceUpdate)
        {
            try
            {
                lock (_sync)
                {
                    if (!forcePriceUpdate && price.IsPriceUpToDate())
                    {
                        return;
                    }

                    var request = new CardPriceRequest(_notificationCenter);
                    request.PerformRequest(definition, price, true, additionalLogText);

                    if (autoWrite)
                    {
                        Write();
                    }
                }
            }
            catch (Exception error)
            {
                _notificationCenter.FireNotification(
                    LogLevel.Error,
                    string.Format("Error getting price for {0}({1}): {2} ", definition.NameEN, definition.SetCode, error.Message) + additionalLogText);
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