using System.IO;
using System.Linq;
using MagicFileFormats.Dec;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyMagicCollection.Shared;
using MyMagicCollection.Shared.Models;

namespace MagicFileFormats.UnitTests
{
    [TestClass]
    public class ReadDec
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem(@"TestData\Jeskai Heroic.dec")]
        public void TestReadDeckJeskaiHeroic()
        {
            InnerTestReadDeck(@"Jeskai Heroic.dec", 25, new MagicDeckCard { CardId = "386660", Name = "Seeker of the Way", Location = MagicDeckLocation.Sideboard, Quantity = 2 });
            InnerTestReadDeck(@"Jeskai Heroic.dec", 25, new MagicDeckCard { CardId = "386660", Name = "Seeker of the Way", Location = MagicDeckLocation.Mainboard, Quantity = 2 });
            InnerTestReadDeck(@"Jeskai Heroic.dec", 25, new MagicDeckCard { CardId = "373578", Name = "Akroan Crusader", Location = MagicDeckLocation.Mainboard, Quantity = 4 });
        }

        [TestMethod]
        [DeploymentItem(@"TestData\Br4nc4_112564.dec")]
        public void TestReadDeckBr4nc4_112564()
        {
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new MagicDeckCard { CardId = null, Name = "Mountain", Location = MagicDeckLocation.Mainboard, Quantity = 20 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new MagicDeckCard { CardId = null, Name = "Vexing Devil", Location = MagicDeckLocation.Mainboard, Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new MagicDeckCard { CardId = null, Name = "Skullcrack", Location = MagicDeckLocation.Mainboard, Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new MagicDeckCard { CardId = null, Name = "Searing Blaze", Location = MagicDeckLocation.Mainboard, Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new MagicDeckCard { CardId = null, Name = "Rift Bolt", Location = MagicDeckLocation.Mainboard, Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new MagicDeckCard { CardId = null, Name = "Lightning Bolt", Location = MagicDeckLocation.Mainboard, Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new MagicDeckCard { CardId = null, Name = "Lava Spike", Location = MagicDeckLocation.Mainboard, Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new MagicDeckCard { CardId = null, Name = "Hellspark Elemental", Location = MagicDeckLocation.Mainboard, Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new MagicDeckCard { CardId = null, Name = "Goblin Guide", Location = MagicDeckLocation.Mainboard, Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new MagicDeckCard { CardId = null, Name = "Eidolon of the Great Revel", Location = MagicDeckLocation.Mainboard, Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new MagicDeckCard { CardId = null, Name = "Shard Volley", Location = MagicDeckLocation.Mainboard, Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new MagicDeckCard { CardId = null, Name = "Flames of the Blood Hand", Location = MagicDeckLocation.Mainboard, Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new MagicDeckCard { CardId = null, Name = "Anger of the Gods", Location = MagicDeckLocation.Sideboard, Quantity = 1 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new MagicDeckCard { CardId = null, Name = "Leyline of Punishment", Location = MagicDeckLocation.Sideboard, Quantity = 2 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new MagicDeckCard { CardId = null, Name = "Combust", Location = MagicDeckLocation.Sideboard, Quantity = 3 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new MagicDeckCard { CardId = null, Name = "Molten Rain", Location = MagicDeckLocation.Sideboard, Quantity = 3 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new MagicDeckCard { CardId = null, Name = "Searing Blood", Location = MagicDeckLocation.Sideboard, Quantity = 2 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new MagicDeckCard { CardId = null, Name = "Smash to Smithereens", Location = MagicDeckLocation.Sideboard, Quantity = 4 });
        }

        private void InnerTestReadDeck(string fileName, int expectedNumberOfRows, MagicDeckCard expectedCard)
        {
            var notification = new Mock<INotificationCenter>();
            var fullName = Path.Combine(TestContext.DeploymentDirectory, fileName);
            var target = new DecReader(notification.Object);
            var result = target.ReadFile(fullName);

            Assert.AreEqual(expectedNumberOfRows, result.Count(), "Wrong number of rows");

            var found = result.Where(r => r.CardId == expectedCard.CardId
                && r.Name == expectedCard.Name
                && r.Quantity == expectedCard.Quantity
                && r.Location == expectedCard.Location);
        }
    }
}