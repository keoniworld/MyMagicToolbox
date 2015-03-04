using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.Shared.Helper
{
    public static class MagicLanguagehelper
    {
        public static MagicLanguage? ToMagicLanguage(this string instance)
        {
            instance = instance?.ToLowerInvariant();
            switch (instance)
            {
                case "german":
                    return MagicLanguage.German;

                default:
                case "english":
                    return MagicLanguage.English;

                case "italian":
                    return MagicLanguage.Italian;

                case "spanish":
                    return MagicLanguage.Spanish;
            }
        }
    }
}