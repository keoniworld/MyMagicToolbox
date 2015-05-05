using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using MyMagicCollection.Shared.Models;

namespace UpdateCardDatabase
{
    public static class SetDefinitions
    {
        private static string definitionCsv = @"
DTK, DTK, DTK, Dragons of Tarkir, Khans of Tarkir, 03/2015, False,
FRF, FRF, FRF, Fate Reforged, Khans of Tarkir, 01/2015, False,
KTK, KTK, KTK, Khans of Tarkir, Khans of Tarkir, 09/2014, False,

JOU, JOU, JOU, Journey into Nyx, Theros, 05/2014, False,
BNG, BNG, BNG, Born of the Gods, Theros, 02/2014, False,
THS, THS, THS, Theros, Theros, 09/2013, False,

LE, LGN, LE, Legions, , 02/2003, False
HL, HML, HL, Homelands, , 10/1995, False
LG, LEG, LG, Legends, , 07/1994, False


M15, M15, M15, Magic 2015 Core Set,,07/2014,False
M14, M14, M14, Magic 2014 Core Set,,07/2013,False
M13, M13, M13, Magic 2013,,07/2012,False
M12, M12, M12, Magic 2012,,07/2011,False
M11, M11, M11, Magic 2011,,07/2010,False
M10, M10, M10, Magic 2010,,07/2009,False

10E, 10E, 10E, Tenth Edition,,07/2007,False
9E, 9E, 9E, Ninth Edition,,07/2005,False
8E, 8E, 8E, Eighth Edition,,07/2003,False
7E, 7E, 7E, Seventh Edition,,07/2001,False
6E, 6E, 6E, Sixth Edition,,07/1999,False
5E, 5E, 5E, Fifth Edition,,07/1997,False
4E, 4E, 4E, Fourth Edition,,07/1995,False
R,3ED, RV, Revised,,04/1994,False

C14, C14, C14,Commander 2014,,11/2014,False
C13, C13, C13,Commander 2013,,11/2013,False
CNS, CNS, CNS,Conspiracy,,06/2014,False
DDM,DDM,DDM,Duel Decks: Jace vs. Vraska,,03/2014,False

LEG, V11, FVL, From the Vault: Legends,,08/2011,True

CS,CSP,CS, Coldsnap, Coldsnap, 06/2006, False
AL,AL,AI, Alliances, Ice Age, 06/1996, False
IA,IA,IA, Ice Age, Ice Age, 06/1995, False
";

        static SetDefinitions()
        {
            var config = new CsvConfiguration()
            {
                Encoding = Encoding.UTF8,
                HasHeaderRecord = false,
                CultureInfo = CultureInfo.InvariantCulture,
            };

            BlockDefinition = new Dictionary<string, MagicSetDefinition>();

            using (var inputCsv = new CsvReader(new StringReader(definitionCsv), config))
            {
                while (inputCsv.Read())
                {
                    var setDefinition = new MagicSetDefinition
                    {
                        Code = inputCsv.GetField<string>(1).Trim(),
                        CodeMagicCardsInfo = inputCsv.GetField<string>(2).Trim(),
                        Name = inputCsv.GetField<string>(3).Trim(),
                        Block = inputCsv.GetField<string>(4).Trim(),
                        ReleaseDate = inputCsv.GetField<string>(5).Trim(),
                        IsPromoEdition = inputCsv.GetField<bool>(6),
                    };

                    BlockDefinition.Add(inputCsv.GetField<string>(0).Trim(), setDefinition);
                }
            }
        }

        public static Dictionary<string, MagicSetDefinition> BlockDefinition { get; private set; }
    }
}