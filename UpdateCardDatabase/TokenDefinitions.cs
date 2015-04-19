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
	public static class TokenDefinitions
	{
		private const string definitionCsv = @"

COMMENT,,, ----- Conspiracy -----
CNS,1,Zombie Token,Zombiespielstein
CNS,2,Spirit Token,Geistspielstein
CNS,3,Demon Token,Dämonenspielstein
CNS,4,Ogre Token,Ogerspielstein
CNS,5,Elephant Token,Elephantenspielstein
CNS,6,Squirrel Token,Eichhörchenspielstein
CNS,7,Wolf Token,Wolfspielstein
CNS,8,Construct Token,Konstruktspielstein
CNS,9,Emblem Dack Fayden,Emblem Dack Fayden

COMMENT,,, ----- M15 -----
M15,1,Wolf Token,
M15,2,Sliver Token,
M15,3,Soldier Token,
M15,4,Zombie Token,
M15,5,Goblin Token,
M15,6,Beast Token (Black),
M15,7,Insect Token,
M15,8,Spirit Token,
M15,9,Squid Token,
M15,10,Beast Token (Green),
M15,11,Dragon Token,
M15,12,Treefolk Warrior Token,
M15,13,Land Mine Token,
M15,14,Emblem Ajani,
M15,15,Emblem Garruk,

COMMENT,,, ----- Born of the Gods-----
BNG,1,Bird Token,
BNG,2,Cat Soldier Token,
BNG,3,Soldier Token,
BNG,4,Bird 2 Token,
BNG,5,Kraken Token,
BNG,6,Zombie Token,
BNG,7,Elemental Token,
BNG,8,Centaur Token,
BNG,9,Wolf Token,
BNG,10,Gold Token,
BNG,11,Emblem Kiora the Crashing Wave,

COMMENT,,, ----- Theros -----
THS,1,Cleric Token,
THS,2,Soldier 1 Token,
THS,3,Soldier 2 Token,
THS,4,Bird Token,
THS,5,Elemental Token,
THS,6,Harpy Token,
THS,7,Soldier Token,
THS,8,Boar Token,
THS,9,Satyr Token,
THS,10,Golem Token,
THS,11,Elspeth Sun's Champion Emblem,

COMMENT,,, ----- M15 -----
WWK,1,Soldier Ally Token,
WWK,2,Dragon Token,
WWK,3,Ogre Token,
WWK,4,Elephant Token,
WWK,5,Plant Token,
WWK,6,Construct Token,
";

		static TokenDefinitions()
		{
			var config = new CsvConfiguration()
			{
				Encoding = Encoding.UTF8,
				HasHeaderRecord = false,
				CultureInfo = CultureInfo.InvariantCulture,
			};

			TockenDefinition = new List<MagicCardDefinition>();

			using (var inputCsv = new CsvReader(new StringReader(definitionCsv), config))
			{
				while (inputCsv.Read())
				{
					var setCode = inputCsv.GetField<string>(0).Trim();
					if (setCode == "COMMENT")
					{
						continue;
					}

					var cardDefinition = new MagicCardDefinition()
					{
						SetCode = setCode,
						NumberInSet = inputCsv.GetField<int>(1),
						NameEN = inputCsv.GetField<string>(2).Trim(),
						NameDE = inputCsv.GetField<string>(3).Trim(),

						MagicCardType = MagicCardType.Token,
					};

					cardDefinition.CardId = string.Format(
						CultureInfo.InvariantCulture,
						"{0}_TOKEN_{1}",
						cardDefinition.SetCode,
						cardDefinition.NumberInSet);

					TockenDefinition.Add(cardDefinition);
				}
			}
		}

		public static List<MagicCardDefinition> TockenDefinition { get; }
	}
}