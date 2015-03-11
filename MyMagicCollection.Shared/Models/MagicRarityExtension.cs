using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMagicCollection.Shared.Models
{
    public static class MagicRarityExtension
    {
        public static string ToCode(this MagicRarity rarity)
        {
            switch (rarity)
            {
                case MagicRarity.Mythic:
                    return "M";

                case MagicRarity.Rare:
                    return "R";

                case MagicRarity.Uncommon:
                    return "U";

                default:
                case MagicRarity.Common:
                    return "C";
            }
        }
    }
}