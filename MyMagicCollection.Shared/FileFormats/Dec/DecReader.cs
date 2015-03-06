using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MyMagicCollection.Shared;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.ViewModels;

namespace MagicFileFormats.Dec
{
    public class DecReader
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

        public IEnumerable<MagicDeckCard> ReadFile(string fileName)
        {
            var file = new FileInfo(fileName);
            var stopwatch = Stopwatch.StartNew();

            var foundCards = new List<MagicDeckCard>();
            var content = File.ReadAllLines(fileName);
            foreach (var line in content)
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

            stopwatch.Stop();
            _notificationCenter.FireNotification(null, string.Format("Loading dec {0} took {1}", file, stopwatch.Elapsed));

            return result;
        }
    }
}