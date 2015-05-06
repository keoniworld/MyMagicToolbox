using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using MyMagicCollection.Shared.Helper;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.VieModels;
using MyMagicCollection.Shared.ViewModels;

namespace MyMagicCollection.Shared.FileFormats.DeckBoxCsv
{
    public class DeckBoxCsvWriter
    {
        public void Write(
            string fileName,
            IEnumerable<IMagicBinderCardViewModel> cards,
            MagicLanguage language,
            MagicGrade grade,
            Func<IMagicBinderCardViewModel, int> quantitySelector)
        {
            var outputCsv = new CsvWriter(File.CreateText(fileName));

            outputCsv.WriteField("Count");
            outputCsv.WriteField("Name");
            outputCsv.WriteField("Edition");
            outputCsv.WriteField("Foil");
            outputCsv.WriteField("Language");
            outputCsv.WriteField("Condition");
            outputCsv.WriteField("Comment");
            outputCsv.NextRecord();

            string lastSetCode = "";
            MagicSetDefinition definition = null;
            foreach (var card in cards)
            {
                if (lastSetCode != card.Definition.SetCode)
                {
                    lastSetCode = card.Definition.SetCode;
                    definition = StaticMagicData.SetDefinitionsBySetCode[lastSetCode];
                }
                
                outputCsv.WriteField(quantitySelector(card));
                outputCsv.WriteField(card.NameEN, true);
                outputCsv.WriteField(definition.Name.Replace("Magic: The Gathering�Conspiracy", "Conspiracy"), true);
                outputCsv.WriteField(card.IsFoil ? "foil" : null);
                outputCsv.WriteField(card.Language.HasValue ? card.Language.Value.ToString() : language.ToString());
                outputCsv.WriteField(card.Grade.HasValue ? card.Grade.Value.ToCsv() : grade.ToCsv());
                outputCsv.WriteField(card.Comment);
                outputCsv.NextRecord();
            }

            outputCsv.Dispose();
        }

        public void Write(
           string fileName,
           IEnumerable<IMagicBinderCardViewModel> cards,
           MagicLanguage language,
           MagicGrade grade)
        {
            Write(
                fileName,
                cards,
                language,
                grade,
                new Func<IMagicBinderCardViewModel, int>((card) => card.Quantity));
        }
    }
}