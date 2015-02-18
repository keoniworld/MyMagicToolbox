using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MagicFileFormats.Dec
{
    public class DecReader
    {
        private const RegexOptions regexOptions = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;

        private Regex mvidRegex = new Regex(@"(?:
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

        public IEnumerable<DeckCard> ReadFile(string fileName)
        {
            var foundCards = new List<DeckCard>();
            var content = File.ReadAllLines(fileName);
            foreach (var line in content)
            {
                var card = new DeckCard();

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
                    card.Location = location.Success ? location.Value : "Deck";
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
    }
}