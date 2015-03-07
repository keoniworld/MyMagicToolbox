using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMagicCollection.Shared.FileFormats.DeckBoxCsv;
using yMagicCollection.Shared.FileFormats.Dec;

namespace MyMagicCollection.Shared.FileFormats
{
    public class DetectFileFormat
    {
        private INotificationCenter _notificationCenter;
        public DetectFileFormat(INotificationCenter notificationCenter)
        {
            _notificationCenter = notificationCenter;
        }

        public IReadCards Detect(string fileName, string fileContent)
        {
            // simple test first:
            // Deckbox CSV:
            if (fileContent.StartsWith("Count,Tradelist Count,Name,Edition,Card Number,Condition,Language"))
            {
                return new DeckBoxInventoryCsvReader(_notificationCenter);
            }

            // Check .dec files
            var file = new FileInfo(fileName);
            if (file.Extension.ToLowerInvariant() == ".dec")
            {
                return new DecReader(_notificationCenter);
            }

            throw new InvalidOperationException("Cannot determine file format.");
        }
    }
}
