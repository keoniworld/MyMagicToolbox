using MyMagicCollection.Shared.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace UpdateCardDatabase
{
    public static class CardDatabaseHelper
    {
        public static string PatchSetName(string setCode)
        {
            switch (setCode)
            {
                case "Magic: The Gathering—Conspiracy":
                    return "Conspiracy";

                case "Magic: The Gathering-Commander":
                    return "Commander";

                ////case "Magic 2014 Core Set":
                ////    return "Magic 2014";

                ////case "Magic 2015 Core Set":
                ////    return "Magic 2015";

                case "Unlimited Edition":
                    return "Unlimited";

                case "Revised Edition":
                    return "Revised";

                case "Classic Sixth Edition":
                    return "Sixth Edition";

                case "Battle Royale Box Set":
                    return "Battle Royale";

                case "Commander 2013 Edition":
                    return "Commander 2013";

                case "Prerelease Events":
                    return "Prerelease Promos";

                case "From the Vault: Annihilation (2014)":
                    return "From the Vault: Annihilation";

                case "Planechase 2012 Edition":
                    return "Planechase 2012";

                case "Ugin's Fate promos":
                    return "Ugin's Fate Promos";
            }

            return setCode;
        }

        public static MagicRarity? ComputeRarity(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            switch (input.ToLowerInvariant())
            {
                case "l":
                case "basic land":
                    return MagicRarity.Land;

                case "t":
                case "t // t":
                    return MagicRarity.Token;

                case "u":
                case "u // u":
                case "uncommon":
                    return MagicRarity.Uncommon;

                case "c":
                case "c // c":
                case "common":
                    return MagicRarity.Common;

                case "m":
                case "m // m":
                case "mythic rare":
                    return MagicRarity.Mythic;

                case "r":
                case "r // r":
                case "rare":
                    return MagicRarity.Rare;

                case "special":
                case "":
                    return null;
            }

            throw new InvalidOperationException("Invalid rarity " + input);
        }

        public static int? ComputeConvertedManaCost(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            return input
                .Replace("//", "")
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => (int)Math.Round(decimal.Parse(s, CultureInfo.InvariantCulture)))
                .Min();
        }

        public static string GetPotentialVersionCardData(IEnumerable<MagicCardDefinition> cards)
        {
            var result = new StringBuilder();

            ////var swamps = StaticMagicData.CardDefinitions.Where(c => c.NameEN.IndexOf("swamp", StringComparison.InvariantCultureIgnoreCase) >= 0 && c.SetCode == "RTR")
            ////    .ToList();

            var grouped = cards
                .GroupBy(c => StaticMagicData.MakeNameSetCode(c.SetCode, c.NameMkm, ""))
                .Where(g => g.Count() > 1)
                .ToList();

            foreach (var group in grouped)
            {
                int version = 1;
                foreach (var item in group.OrderBy(c => c.NameMkm).ThenBy(c => c.CardId))
                {
                    result.AppendFormat("{0}, {1} (Version {4}) {2}", item.CardId, item.NameMkm, item.SetCode, item.NumberInSet, version);
                    result.AppendLine();

                    version++;
                }
            }

            return result.ToString();
        }

        public static void RemoveSplitCardDuplicates(IList<MagicCardDefinition> cards)
        {
            var splitCards = cards
                .Where(c => c.CardLayout == "split")
                .GroupBy(c => c.NameEN + c.SetCode)
                .Select(c => c.OrderBy(g => g.NumberInSet).ElementAt(1))
                .ToList();

            foreach (var item in splitCards)
            {
                cards.Remove(item);
            }
        }

        public static void PatchPotentialVersionCardData(IEnumerable<MagicCardDefinition> cards)
        {
            var result = new StringBuilder();

            ////var swamps = StaticMagicData.CardDefinitions.Where(c => c.NameEN.IndexOf("swamp", StringComparison.InvariantCultureIgnoreCase) >= 0 && c.SetCode == "RTR")
            ////    .ToList();

            var grouped = cards
                .GroupBy(c => StaticMagicData.MakeNameSetCode(c.SetCode, c.NameMkm, ""))
                .Where(g => g.Count() > 1)
                .ToList();

            foreach (var group in grouped)
            {
                int version = 1;
                foreach (var item in group.OrderBy(c => c.NameMkm).ThenBy(c => c.CardId))
                {
                    item.NameMkm = item.NameMkm + " (Version " + version + ")";
                    ////result.AppendFormat("{0}, {1} (Version {4}) {2}", item.CardId, item.NameMkm, item.SetCode, item.NumberInSet, version);
                    ////result.AppendLine();

                    version++;
                }
            }
        }
    }
}