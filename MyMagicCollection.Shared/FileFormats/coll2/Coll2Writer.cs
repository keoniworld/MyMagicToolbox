using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.VieModels;

namespace MyMagicCollection.Shared.FileFormats.coll2
{
    public class Coll2Writer
    {
        private readonly INotificationCenter _notificationCenter;

        public Coll2Writer(INotificationCenter notificationCenter)
        {
            _notificationCenter = notificationCenter;
        }

        public void Write(
            string fileName,
            IEnumerable<IMagicBinderCard> cards)
        {
            var info = new FileInfo(fileName);
            var parentFolder = info.Directory;

            if (!parentFolder.Exists)
            {
                parentFolder.Create();
            }

            var builder = new StringBuilder();
            builder.AppendLine(@"doc:
- version: 1
- items:");

            var grouped = cards.GroupBy(c => c.CardId);

            foreach (var card in grouped)
            {
                int quantity = 0;
                int quantityFoil = 0;
                foreach (var c in card)
                {
                    quantity += !c.IsFoil ? c.Quantity : 0;
                    quantityFoil += c.IsFoil ? c.Quantity : 0;
                }

                if (quantity > 0 || quantityFoil > 0)
                {
                    builder.AppendLine("  - - id: " + card.Key);

                    if (quantity > 0)
                    {
                        builder.AppendLine(string.Format(CultureInfo.InvariantCulture, "    - r: {0}", quantity));
                    }

                    if (quantityFoil > 0)
                    {
                        builder.AppendLine(string.Format(CultureInfo.InvariantCulture, "    - f: {0}", quantityFoil));
                    }
                }
            }

            File.WriteAllText(fileName, builder.ToString(), Encoding.ASCII);
        }
    }
}