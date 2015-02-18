using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using MagicFileContracts;

namespace MagicFileFormats.CSV
{
    public class DeckBoxCsvWriter
    {
        public void Write(string fileName, IEnumerable<IDeckCard> cards, bool isFoil, MagicContracts.Language language)
        {

            var outputCsv = new CsvWriter(File.CreateText(fileName));

            outputCsv.WriteField("Count");
            outputCsv.WriteField("Name");
            outputCsv.WriteField("Edition");
            outputCsv.WriteField("Foil");
            outputCsv.WriteField("Language");
            outputCsv.NextRecord();

            foreach (var card in cards)
            {
                outputCsv.WriteField(card.Quantity);
                outputCsv.WriteField(card.Name, true);
                outputCsv.WriteField(card.Set.Replace("Magic: The Gathering�Conspiracy", "Conspiracy"), true);
                // outputCsv.WriteField(card.IsFoil ? "foil" : null);
                outputCsv.WriteField(isFoil ? "foil" : null);
                outputCsv.WriteField(language.ToString());
                outputCsv.NextRecord();
            }

            outputCsv.Dispose();
        }
    }
}
