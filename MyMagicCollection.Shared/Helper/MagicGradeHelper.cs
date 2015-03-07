using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.Shared.Helper
{
    public static class MagicGradeHelper
    {
        public static MagicGrade? ToMagicGrade(this string instance)
        {
            if (string.IsNullOrEmpty(instance))
            {
                return null;
            }

            instance = instance?.ToLowerInvariant();
            switch (instance)
            {
                case "mint":
                    return MagicGrade.Mint;

                case "near mint":
                    return MagicGrade.NearMint;

                case "excellent":
                    return MagicGrade.Excellent;

                case "good":
                    return MagicGrade.Good;

                case "lightlyplayed":
                    return MagicGrade.LightlyPlayed;

                case "played":
                    return MagicGrade.Played;

                case "poor":
                    return MagicGrade.Poor;

                default:
                    return null;
            }
        }

        public static string ToCsv(this MagicGrade instance)
        {
            switch (instance)
            {
                case MagicGrade.Mint:
                    return "Mint";

                default:
                case MagicGrade.NearMint:
                    return "Near Mint";

                case MagicGrade.Excellent:
                    return "Excellent";

                case MagicGrade.Good:
                    return "Good";

                case MagicGrade.LightlyPlayed:
                    return "Lightly Played";

                case MagicGrade.Played:
                    return "Played";

                case MagicGrade.Poor:
                    return "Poor";
            }
        }
    }
}