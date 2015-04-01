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

M15, M15, M15, Magic 2015 Core Set,,07/2014,False
M14, M14, M14, Magic 2014 Core Set,,07/2013,False
M13, M13, M13, Magic 2013,,07/2012,False
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