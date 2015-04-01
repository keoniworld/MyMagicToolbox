using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyMagicCollection.Shared;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.UnitTests.Models
{
    [TestClass]
    public class StaticMagicDataTests
    {
        [TestMethod]
        public void TestSetData()
        {
            // Khans Block
            InnerTestSetData("DTK", "Dragons of Tarkir", "DTK", "03/2015", "Khans of Tarkir", false);
            InnerTestSetData("FRF", "Fate Reforged", "FRF", "01/2015", "Khans of Tarkir", false);
            InnerTestSetData("KTK", "Khans of Tarkir", "KTK", "09/2014", "Khans of Tarkir", false);

            // Theros Block
            InnerTestSetData("JOU", "Journey into Nyx", "JOU", "05/2014", "Theros", false);
            InnerTestSetData("BNG", "Born of the Gods", "FRF", "02/2014", "Theros", false);
            InnerTestSetData("THS", "Theros", "KTK", "09/2013", "Theros", false);

            // Core Sets
            InnerTestSetData("M15", "Magic 2015 Core Set", "M15", "07/2014", "", false);
            InnerTestSetData("M14", "Magic 2014 Core Set", "M14", "07/2013", "", false);
            InnerTestSetData("M13", "Magic 2013", "M13", "07/2012", "", false);




            /*
            Code,Name,CodeMagicCardsInfo
10E,Tenth Edition,10E
2ED,Unlimited Edition,2ED
3ED,Revised Edition,3ED
4ED,Fourth Edition,4E
5DN,Fifth Dawn,5DN
5ED,Fifth Edition,5E
6ED,Sixth Edition,6E
7ED,Seventh Edition,7E
8ED,Eighth Edition,8E
9ED,Ninth Edition,9E
AL,Alliances,AI
ALA,Shards of Alara,ALA
AN,Arabian Nights,AN
AP,Apocalypse,AP
AQ,Antiquities,AQ
ARB,Alara Reborn,ARB
ARC,Archenemy,ARC
AVR,Avacyn Restored,AVR
BD,Beatdown Box Set,BD
BNG,Born of the Gods,BNG
BOK,Betrayers of Kamigawa,BOK
BR,Battle Royale Box Set,BR
C13,Commander 2013 Edition,C13
C14,Commander 2014,C14
CH,Chronicles,CH
CHK,Champions of Kamigawa,CHK
CM1,Commander's Arsenal,CMA
CMD,Commander,CMD
CNS,Conspiracy,CNS
CON,Conflux,CON

DD2,"Duel Decks Anthology, Jace vs. Chandra",DD2
DD3,"Duel Decks Anthology, Elves vs. Dragons",DD3
DDC,"Duel Decks Anthology, Divine vs. Demonic",DDC
DDD,"Duel Decks Anthology, Garruk vs. Liliana",DDD
DDH,Duel Decks: Ajani vs. Nicol Bolas,DDH
DDI,Duel Decks: Venser vs. Koth,DDI
DDL,Duel Decks: Heroes vs. Monsters,DDL
DGM,Dragon's Maze,DGM
DIS,Dissension,DI
DK,The Dark,DK
DKA,Dark Ascension,DKA
DM,Deckmasters,DM
DRP,From the Vault: Dragons,DPA

DVD,Duel Decks: Divine vs. Demonic,DVD
EVE,Eventide,EVE
EVG,Duel Decks: Elves vs. Goblins,EVG
EVT,Duel Decks: Elspeth vs. Tezzeret,DDF
EX,Exodus,EX
EXL,From the Vault: Exiled,FVE
FE,Fallen Empires,FE
FNM,Friday Night Magic,FNMP
FRF,Fate Reforged,FRF
FUT,Future Sight,FUT
GDC,Magic Game Day Cards,MGDC
GLP,Magic: The Gathering Launch Parties,MLP
GPT,Guildpact,GP
GPX,Grand Prix,GPX
GRV,Premium Deck Series: Graveborn,PD3
GTC,Gatecrash,GTC
GTW,WPN/Gateway,GRC
GVL,Duel Decks: Garruk vs. Liliana,GVL
HML,Homelands,HML
IA,Ice Age,IA
IN,Invasion,IN
ISD,Innistrad,ISD
IVG,Duel Decks: Izzet vs. Golgari,DDJ
JCG,Judge Gift Program,JR
JOU,Journey into Nyx,JOU
JU,Judgment,JU
JVC,Duel Decks: Jace vs. Chandra,JVC
JVV,Duel Decks: Jace vs. Vraska,DDM
KTK,Khans of Tarkir,KTK
KVD,Duel Decks: Knights vs. Dragons,DDG
LE,Legions,LE
LEA,Limited Edition Alpha,LEA
LEB,Limited Edition Beta,LEB
LEG,From the Vault: Legends,FVL
LRW,Lorwyn,LW
M10,Magic 2010,M10
M11,Magic 2011,M11
M12,Magic 2012,M12
M13,Magic 2013,M13
M14,Magic 2014,M14

MBS,Mirrodin Besieged,MBS
MD1,Modern Event Deck 2014,MD1
ME2,Masters Edition II,ME2
ME3,Masters Edition III,ME3
ME4,Masters Edition IV,ME4
MI,Mirage,MR
MM,Mercadian Masques,MM
MMA,Modern Masters,MMA
MOR,Morningtide,MT
MRD,Mirrodin,MI
NE,Nemesis,NE
NPH,New Phyrexia,NPH
OD,Odyssey,OD
ONS,Onslaught,ON
P2,Portal Second Age,PO2
P3,Portal Three Kingdoms,P3K
PC2,Planechase 2012 Edition,PC2
PCH,Planechase,PCH
PCP,"Planechase ""Planes""",PCHP
PLC,Planar Chaos,PC
POR,Portal,PO
PP2,"Planechase 2012 Edition ""Planes"" and ""Phenomena""",PP2
PRE,Prerelease Promos,PTC
PS,Planeshift,PS
PVC,Duel Decks: Phyrexia vs. the Coalition,PVC
PY,Prophecy,PR
RAV,Ravnica: City of Guilds,RAV
REW,Magic Player Rewards,MPRP
RLC,From the Vault: Relics,FVR
RLM,From the Vault: Realms,V12
RLS,Release Events,REP
ROE,Rise of the Eldrazi,ROE
RTR,Return to Ravnica,RTR
SC,Scourge,SC
SH,Stronghold,SH
SHM,Shadowmoor,SHM
SOK,Saviors of Kamigawa,SOK
SOM,Scars of Mirrodin,SOM
SUM,Summer of Magic,SUM
SVC,Duel Decks: Speed vs. Cunning,DDN
SVT,Duel Decks: Sorin vs. Tibalt,DDK
TE,Tempest,TP
THS,Theros,THS
TOR,Torment,TR
TSB,"Time Spiral ""Timeshifted""",TSTS
TSP,Time Spiral,TS
UDS,Urza's Destiny,UD
UG,Unglued,UG
ULG,Urza's Legacy,UL
UNH,Unhinged,UH
USG,Urza's Saga,US
V13,From the Vault: Twenty,V13
V14,From the Vault: Annihilation,V14
VI,Visions,VI
VMA,Vintage Masters,VMA
WL,Weatherlight,WL
WWK,Worldwake,WWK
ZEN,Zendikar,ZEN
10E,Tenth Edition,10E
2ED,Unlimited Edition,2ED
3ED,Revised Edition,3ED
4ED,Fourth Edition,4E
5DN,Fifth Dawn,5DN
5ED,Fifth Edition,5E
6ED,Sixth Edition,6E
7ED,Seventh Edition,7E
8ED,Eighth Edition,8E
9ED,Ninth Edition,9E
AL,Alliances,AI
ALA,Shards of Alara,ALA
AN,Arabian Nights,AN
AP,Apocalypse,AP
AQ,Antiquities,AQ
ARB,Alara Reborn,ARB
ARC,Archenemy,ARC
AVR,Avacyn Restored,AVR
BD,Beatdown Box Set,BD
BNG,Born of the Gods,BNG
BOK,Betrayers of Kamigawa,BOK
BR,Battle Royale Box Set,BR
C13,Commander 2013 Edition,C13
C14,Commander 2014,C14
CH,Chronicles,CH
CHK,Champions of Kamigawa,CHK
CM1,Commander's Arsenal,CMA
CMD,Commander,CMD
CNS,Conspiracy,CNS
CON,Conflux,CON
CSP,Coldsnap,CSP
DD2,"Duel Decks Anthology, Jace vs. Chandra",DD2
DD3,"Duel Decks Anthology, Elves vs. Dragons",DD3
DDC,"Duel Decks Anthology, Divine vs. Demonic",DDC
DDD,"Duel Decks Anthology, Garruk vs. Liliana",DDD
DDH,Duel Decks: Ajani vs. Nicol Bolas,DDH
DDI,Duel Decks: Venser vs. Koth,DDI
DDL,Duel Decks: Heroes vs. Monsters,DDL
DGM,Dragon's Maze,DGM
DIS,Dissension,DI
DK,The Dark,DK
DKA,Dark Ascension,DKA
DM,Deckmasters,DM
DRP,From the Vault: Dragons,DPA
DST,Darksteel,DST

DVD,Duel Decks: Divine vs. Demonic,DVD
EVE,Eventide,EVE
EVG,Duel Decks: Elves vs. Goblins,EVG
EVT,Duel Decks: Elspeth vs. Tezzeret,DDF
EX,Exodus,EX
EXL,From the Vault: Exiled,FVE
FE,Fallen Empires,FE
FNM,Friday Night Magic,FNMP
FRF,Fate Reforged,FRF
FUT,Future Sight,FUT
GDC,Magic Game Day Cards,MGDC
GLP,Magic: The Gathering Launch Parties,MLP
GPT,Guildpact,GP
GPX,Grand Prix,GPX
GRV,Premium Deck Series: Graveborn,PD3
GTC,Gatecrash,GTC
GTW,WPN/Gateway,GRC
GVL,Duel Decks: Garruk vs. Liliana,GVL
HML,Homelands,HML
IA,Ice Age,IA
IN,Invasion,IN
ISD,Innistrad,ISD
IVG,Duel Decks: Izzet vs. Golgari,DDJ
JCG,Judge Gift Program,JR
JOU,Journey into Nyx,JOU
JU,Judgment,JU
JVC,Duel Decks: Jace vs. Chandra,JVC
JVV,Duel Decks: Jace vs. Vraska,DDM
KTK,Khans of Tarkir,KTK
KVD,Duel Decks: Knights vs. Dragons,DDG
LE,Legions,LE
LEA,Limited Edition Alpha,LEA
LEB,Limited Edition Beta,LEB
LEG,From the Vault: Legends,FVL
LRW,Lorwyn,LW
M10,Magic 2010,M10
M11,Magic 2011,M11
M12,Magic 2012,M12
M13,Magic 2013,M13
M14,Magic 2014,M14
M15,Magic 2015,M15
MBS,Mirrodin Besieged,MBS
MD1,Modern Event Deck 2014,MD1
ME2,Masters Edition II,ME2
ME3,Masters Edition III,ME3
ME4,Masters Edition IV,ME4
MI,Mirage,MR
MM,Mercadian Masques,MM
MMA,Modern Masters,MMA
MOR,Morningtide,MT
MRD,Mirrodin,MI
NE,Nemesis,NE
NPH,New Phyrexia,NPH
OD,Odyssey,OD
ONS,Onslaught,ON
P2,Portal Second Age,PO2
P3,Portal Three Kingdoms,P3K
PC2,Planechase 2012 Edition,PC2
PCH,Planechase,PCH
PCP,"Planechase ""Planes""",PCHP
PLC,Planar Chaos,PC
POR,Portal,PO
PP2,"Planechase 2012 Edition ""Planes"" and ""Phenomena""",PP2
PRE,Prerelease Promos,PTC
PS,Planeshift,PS
PVC,Duel Decks: Phyrexia vs. the Coalition,PVC
PY,Prophecy,PR
RAV,Ravnica: City of Guilds,RAV
REW,Magic Player Rewards,MPRP
RLC,From the Vault: Relics,FVR
RLM,From the Vault: Realms,V12
RLS,Release Events,REP
ROE,Rise of the Eldrazi,ROE
RTR,Return to Ravnica,RTR
SC,Scourge,SC
SH,Stronghold,SH
SHM,Shadowmoor,SHM
SOK,Saviors of Kamigawa,SOK
SOM,Scars of Mirrodin,SOM
SUM,Summer of Magic,SUM
SVC,Duel Decks: Speed vs. Cunning,DDN
SVT,Duel Decks: Sorin vs. Tibalt,DDK
TE,Tempest,TP
THS,Theros,THS
TOR,Torment,TR
TSB,"Time Spiral ""Timeshifted""",TSTS
TSP,Time Spiral,TS
UDS,Urza's Destiny,UD
UG,Unglued,UG
ULG,Urza's Legacy,UL
UNH,Unhinged,UH
USG,Urza's Saga,US
V13,From the Vault: Twenty,V13
V14,From the Vault: Annihilation,V14
VI,Visions,VI
VMA,Vintage Masters,VMA
WL,Weatherlight,WL
WWK,Worldwake,WWK
ZEN,Zendikar,ZEN

            */
        }

        public void InnerTestSetData(string setCode, string setName, string magicCardsCode, string releaseDate, string block, bool isPromo)
        {
            var set = StaticMagicData.SetDefinitionsBySetCode[setCode];

            Assert.AreEqual(setName, set.Name, "Name");
            Assert.AreEqual(magicCardsCode, set.CodeMagicCardsInfo, "CodeMagicCardsInfo");
            Assert.AreEqual(block, set.Block, "Block");
            Assert.AreEqual(releaseDate, set.ReleaseDate, "ReleaseDate");
            Assert.AreEqual(isPromo, set.IsPromoEdition, "IsPromoEdition");

            // Now look for the set image:
            var basePath = Path.Combine(PathHelper.SetCacheFolder, "medium");
            Assert.IsTrue(File.Exists(Path.Combine(basePath, setCode + "_C.jpg")), "Missing Set Symbol");
            Assert.IsTrue(File.Exists(Path.Combine(basePath, setCode + "_U.jpg")), "Missing Set Symbol");
            Assert.IsTrue(File.Exists(Path.Combine(basePath, setCode + "_M.jpg")), "Missing Set Symbol");
            Assert.IsTrue(File.Exists(Path.Combine(basePath, setCode + "_R.jpg")), "Missing Set Symbol");
        }
    }
}