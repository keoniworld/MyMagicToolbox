using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace MyMagicCollection.Shared.Price
{
    public class MkmRequestCounter
    {
        private string _storageFileName;
        private object _sync = new object();
        private bool _savedHighCount;

        public MkmRequestCounter()
        {
            _storageFileName = Path.Combine(PathHelper.UserDataFolder, "MKMRequestCounter.dat");
            Day = DateTime.UtcNow.Date;
            Load();
        }

        public int Count { get; set; }

        public DateTime Day { get; set; }

        public bool AddRequest()
        {
            lock (_sync)
            {
                if (Day.Day != DateTime.UtcNow.Day)
                {
                    Day = DateTime.UtcNow.Date;
                    Count = 0;
                }

                Count += 1;
                return Count < 5000;
            }
        }

        public void Save()
        {
            try
            {
                if (Count >= 5000)
                {
                    var ignoreSave = Count > 5001 && _savedHighCount;
                    if (ignoreSave)
                    {
                        return;
                    }

                    _savedHighCount = true;
                }

                using (var textWriter = new StreamWriter(_storageFileName))
                {
                    var writer = new CsvWriter(textWriter);
                    writer.Configuration.CultureInfo = CultureInfo.InvariantCulture;

                    writer.WriteField<string>("Count");
                    writer.WriteField<string>("Day");
                    writer.NextRecord();

                    writer.WriteField<int>(Count);
                    writer.WriteField<DateTime>(Day);
                    writer.NextRecord();
                }
            }
            catch (Exception error)
            {
            }
        }

        private void Load()
        {
            try
            {
                if (!File.Exists(_storageFileName))
                {
                    return;
                }

                var content = File.ReadAllText(_storageFileName);
                using (var inputCsv = new CsvReader(new StringReader(content)))
                {
                    inputCsv.Configuration.CultureInfo = CultureInfo.InvariantCulture;

                    if (inputCsv.Read())
                    {
                        Count = inputCsv.GetField<int>("Count");
                        Day = inputCsv.GetField<DateTime>("Day");
                    }
                }
            }
            catch (Exception error)
            {
            }
        }
    }
}