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
    public class DeckBoxInventoryCsvReader : IReadCards
    {
        private INotificationCenter _notificationCenter;

        public DeckBoxInventoryCsvReader(INotificationCenter notificationCenter)
        {
            _notificationCenter = notificationCenter;
        }

        public IEnumerable<MagicBinderCardViewModel> ReadFile(string fileName)
        {
            return ReadFileContent(File.ReadAllText(fileName));
        }

        public IEnumerable<MagicBinderCardViewModel> ReadFileContent(string content)
        {
            var result = new List<MagicBinderCardViewModel>();

            var groupedCards = StaticMagicData.CardDefinitions.GroupBy(d => d.SetCode).ToList();

            MagicSetDefinition set = null;
            MagicCardDefinition definition = null;
            string lastSetCodeName;
            using (var inputCsv = new CsvReader(new StringReader(content)))
            {
                while (inputCsv.Read())
                {
                    var card = new MagicBinderCard()
                    {
                        Quantity = inputCsv.GetField<int>("Count"),
                        QuantityTrade = inputCsv.GetField<int>("Tradelist Count"),
                        Grade = inputCsv.GetField<string>("Condition").ToMagicGrade(),
                        IsFoil = inputCsv.GetField<string>("Foil") == "foil",
                        Language = inputCsv.GetField<string>("Language").ToMagicLanguage()
                    };

                    var cardNumber = inputCsv.GetField<int?>("Card Number");
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

                    var setCodeName = StaticMagicData.MakeNameSetCode(set.Code, cardName, cardNumber);
                    lastSetCodeName = setCodeName;

                    if (!StaticMagicData.CardDefinitionsByNameSetCode.TryGetValue(setCodeName, out definition))
                    {
                        _notificationCenter.FireNotification("CSV", string.Format("Cannot find card {0} ({1})", cardName, setName));
                        continue;
                    }

                    result.Add(new MagicBinderCardViewModel(definition, card));
                }
            }

            return result;
        }
    }
}