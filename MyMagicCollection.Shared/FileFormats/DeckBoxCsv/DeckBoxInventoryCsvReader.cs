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
    public class DeckBoxInventoryCsvReader
    {
        private INotificationCenter _notificationCenter;

        public DeckBoxInventoryCsvReader(INotificationCenter notificationCenter)
        {
            _notificationCenter = notificationCenter;
        }

        public IEnumerable<MagicBinderCardViewModel> ReadCsvFile(string fileName)
        {
            return ReadCsvFileContent(File.ReadAllText(fileName));
        }

        public IEnumerable<MagicBinderCardViewModel> ReadCsvFileContent(string content)
        {
            var result = new List<MagicBinderCardViewModel>();

            MagicSetDefinition set = null;
            using (var inputCsv = new CsvReader(new StringReader(content)))
            {
                while (inputCsv.Read())
                {
                    var card = new MagicBinderCard()
                    {
                        Quantity = inputCsv.GetField<int>("Count"),
                        QuantityTrade = inputCsv.GetField<int>("Tradelist Count"),
                    };

                    var cardName = inputCsv.GetField<string>("Name");
                    var setName = inputCsv.GetField<string>("Edition");
                    if (set == null || !set.Name.Equals(setName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (!StaticMagicData.SetDefinitionsBySetName.TryGetValue(setName, out set))
                        {
                            // TODO: Handle error

                            _notificationCenter.FireNotification("CSV", string.Format("Cannot find set for card {0} ({1})", cardName, setName));
                            continue;
                        }
                    }

                    
                    var definition = StaticMagicData.CardDefinitions.Where(c => c.SetCode == set.Code && c.NameEN.Equals(cardName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (definition == null)
                    {
                        _notificationCenter.FireNotification("CSV", string.Format("Cannot find card {0} ({1})", cardName, setName));
                        continue;
                    }

                    var condition = inputCsv.GetField<string>("Condition");
                    var foil = inputCsv.GetField<string>("Foil");

                    card.Language = inputCsv.GetField<string>("Language").ToMagicLanguage();
                    card.IsFoil = foil == "foil";
                    card.Grade = condition.ToMagicGrade();

                    result.Add(new MagicBinderCardViewModel(definition, card));
                }
            }

            return result;
        }
    }
}