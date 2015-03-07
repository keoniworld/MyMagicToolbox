using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using MyMagicCollection.Shared.Helper;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.ViewModels;

namespace MyMagicCollection.Shared.FileFormats.DeckBoxCsv
{
    public class DeckBoxCsvWriter
    {
        public void Write(
            string fileName, 
            IEnumerable<FoundMagicCardViewModel> cards, 
            bool isFoil, 
            MagicLanguage language,
            MagicGrade grade)
        {
            var outputCsv = new CsvWriter(File.CreateText(fileName));

            outputCsv.WriteField("Count");
            outputCsv.WriteField("Name");
            outputCsv.WriteField("Edition");
            outputCsv.WriteField("Foil");
            outputCsv.WriteField("Language");
            outputCsv.WriteField("Condition");
            outputCsv.NextRecord();

            string lastSetCode="";
            MagicSetDefinition definition = null;
            foreach (var card in cards)
            {
                if (lastSetCode != card.Definition.SetCode)
                {
                    lastSetCode = card.Definition.SetCode;
                    definition = StaticMagicData.SetDefinitionsBySetCode[lastSetCode];
                }

                outputCsv.WriteField(card.Quantity);
                outputCsv.WriteField(card.NameEN, true);
                outputCsv.WriteField(definition.Name.Replace("Magic: The Gathering�Conspiracy", "Conspiracy"), true);
                outputCsv.WriteField(isFoil ? "foil" : null);
                outputCsv.WriteField(language.ToString());
                outputCsv.WriteField(grade.ToCsv());
                outputCsv.NextRecord();
            }

            outputCsv.Dispose();
        }
    }
}