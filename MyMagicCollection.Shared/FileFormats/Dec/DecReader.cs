using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MyMagicCollection.Shared;
using MyMagicCollection.Shared.FileFormats;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.ViewModels;

namespace yMagicCollection.Shared.FileFormats.Dec
{
    public class DecReader : IReadCards
    {
        private const RegexOptions regexOptions = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;

        private readonly Regex mvidRegex = new Regex(@"(?:
   (?: ^ )
   (?:
      (?:
         (?<location> SB)
         (?: : \s+ )
      )?
      (?<quantity> \d+ )
      (?: \s+ )
      (?<name> [^\r^\n]+ )
   )|
   (?:
      (?: ///mvid: \s*)
      (?<mvid> \d+ )
      (?:
         (?: \s* qty: \s* )
         (?<quantity> \d+ )
         (?: \s* name: \s*)
         (?<name> [^\r^\n]+ )
         (?: \s+ loc: )
         (?<location> (Deck)|(SB) )
      )
   )
)", regexOptions);

        private readonly INotificationCenter _notificationCenter;

        public DecReader(INotificationCenter notificationCenter)
        {
            _notificationCenter = notificationCenter;
        }

        public IEnumerable<MagicBinderCardViewModel> ReadFile(string fileName)
        {
            return ReadFileContent(File.ReadAllText(fileName));
        }

        public IEnumerable<MagicDeckCard> ReadFileContentDec(string content)
        {
            var foundCards = new List<MagicDeckCard>();
            foreach (var line in content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var card = new MagicDeckCard();

                var match = mvidRegex.Match(line);
                if (match.Success)
                {
                    var id = match.Groups["mvid"];
                    if (id.Success)
                    {
                        card.CardId = id.Value;
                    }

                    card.Name = match.Groups["name"].Value;
                    card.Quantity = int.Parse(match.Groups["quantity"].Value);

                    var location = match.Groups["location"];
                    var locationText = location.Success ? location.Value : "Deck";
                    card.Location = locationText.ToLowerInvariant() == "deck" ? MagicDeckLocation.Mainboard : MagicDeckLocation.Sideboard;
                    foundCards.Add(card);
                }
            }

            var result = foundCards.Where(f => !string.IsNullOrEmpty(f.CardId)).ToList();
            var cardsWithoutId = foundCards.Where(f => string.IsNullOrEmpty(f.CardId)).ToList();

            foreach (var card in cardsWithoutId)
            {
                var exists = result.Any(f => f.Name == card.Name && f.Location == card.Location);
                if (!exists)
                {
                    result.Add(card);
                }
            }

            return result;
        }

        public IEnumerable<MagicBinderCardViewModel> ReadFileContent(string content)
        {
            var stopwatch = Stopwatch.StartNew();            
            var result = ReadFileContentDec(content);

            var finalList = new List<MagicBinderCardViewModel>();
            foreach (var card in result)
            {
                var binderCard = new MagicBinderCard
                {
                    CardId = card.CardId,
                    Quantity = card.Quantity,
                };

                MagicCardDefinition definition;
                if (string.IsNullOrWhiteSpace(binderCard.CardId))
                {
                    definition = StaticMagicData.CardDefinitions
                        .First(c => c.NameEN == card.Name);

                    binderCard.CardId = definition.CardId;
                }
                else
                {
                    definition = StaticMagicData.CardDefinitionsByCardId[binderCard.CardId];
                }

                finalList.Add(new MagicBinderCardViewModel(definition, binderCard));
            }

            stopwatch.Stop();
            // _notificationCenter.FireNotification(null, string.Format("Loading dec {0} took {1}", file, stopwatch.Elapsed));

            return finalList;
        }
    }
}