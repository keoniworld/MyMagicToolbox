using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Minimod.NotificationObject;

namespace MyMagicCollection.Shared.Price
{
    public class MkmRequestCounter : NotificationObject
    {
        public DateTime _date;
        private string _storageFileName;
        private object _sync = new object();
        private bool _savedHighCount;

        private int _count;

        public MkmRequestCounter()
        {
            _storageFileName = Path.Combine(PathHelper.UserDataFolder, "MKMRequestCounter.dat");
            Date = DateTime.UtcNow.Date;
            Load();
        }

        public int Count
        {
            get
            {
                return _count;
            }
            private set
            {
                _count = value;
                RaisePropertyChanged(() => Count);
            }
        }

        public DateTime Date
        {
            get
            {
                return _date;
            }
            private set
            {
                _date = value;
                RaisePropertyChanged(() => Count);
            }
        }

        public bool AddRequest()
        {
            lock (_sync)
            {
                if (Date.Date != DateTime.UtcNow.Date)
                {
                    Date = DateTime.UtcNow.Date;
                    Count = 0;
                }

                Count += 1;
                return Count < 4000;
            }
        }

        public void Save()
        {
            try
            {
                if (Count >= 4000)
                {
                    var ignoreSave = Count > 4001 && _savedHighCount;
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
                    writer.WriteField<DateTime>(Date);
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
                        Date = inputCsv.GetField<DateTime>("Day");
                    }
                }
            }
            catch (Exception error)
            {
            }
        }
    }
}