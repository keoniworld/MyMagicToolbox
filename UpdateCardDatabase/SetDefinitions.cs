using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMagicCollection.Shared.Models;

namespace UpdateCardDatabase
{
    public static class SetDefinitions
    {
        public static Dictionary<string, MagicSetDefinition> BlockDefinition = new Dictionary<string, MagicSetDefinition>()
        {
            { "DTK",
              new MagicSetDefinition
              {
                  Code = "DTK",
                  CodeMagicCardsInfo = "DTK",
                  Name = "Dragons of Tarkir",
                  Block = "Khans of Tarkir",
                  ReleaseDate = "03/2015",
              }
            },

            { "FRF",
              new MagicSetDefinition
              {
                  Code = "FRF",
                  CodeMagicCardsInfo = "FRF",
                  Name = "Fate Reforged",
                  Block = "Khans of Tarkir",
                  ReleaseDate = "01/2015",
              }
            },

              { "KTK",
              new MagicSetDefinition
              {
                  Code = "KTK",
                  CodeMagicCardsInfo = "KTK",
                  Name = "Khans of Tarkir",
                  Block = "Khans of Tarkir",
                  ReleaseDate = "09/2014",
              }
            },
        };
    }
}
