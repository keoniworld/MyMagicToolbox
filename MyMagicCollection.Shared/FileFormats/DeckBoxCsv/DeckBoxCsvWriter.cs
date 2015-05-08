using CsvHelper;
using MyMagicCollection.Shared.Helper;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.VieModels;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyMagicCollection.Shared.FileFormats.DeckBoxCsv
{
    public class DeckBoxCsvWriter
    {
        private static string PatchCardName(string cardName)
        {
            return cardName
                .Replace("Æ", "Ae");
        }

        private static Dictionary<string, string> _setLookup = new Dictionary<string, string>
            {
                {  "Battle Royale", "Battle Royale Box Set" },
                { "Unlimited", "Unlimited Edition"},
                { "Revised", "Revised Edition"}
            };

        private static string PatchSetName(string cardName)
        {
            string found = null;
            if (_setLookup.TryGetValue(cardName, out found))
            {
                return found;
            }

            return cardName;
        }

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
                outputCsv.WriteField(PatchCardName(card.NameEN), true);
                outputCsv.WriteField(PatchSetName(definition.Name.Replace("Magic: The Gathering�Conspiracy", "Conspiracy")), true);
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