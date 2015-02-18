using System.IO;
using System.Linq;
using MagicFileFormats.Dec;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            InnerTestReadDeck(@"Jeskai Heroic.dec", 25, new DeckCard { CardId = "386660", Name = "Seeker of the Way", Location = "SB", Quantity = 2 });
            InnerTestReadDeck(@"Jeskai Heroic.dec", 25, new DeckCard { CardId = "386660", Name = "Seeker of the Way", Location = "Deck", Quantity = 2 });
            InnerTestReadDeck(@"Jeskai Heroic.dec", 25, new DeckCard { CardId = "373578", Name = "Akroan Crusader", Location = "Deck", Quantity = 4 });
        }

        [TestMethod]
        [DeploymentItem(@"TestData\Br4nc4_112564.dec")]
        public void TestReadDeckBr4nc4_112564()
        {
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new DeckCard { CardId = null, Name = "Mountain", Location = "Deck", Quantity = 20 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new DeckCard { CardId = null, Name = "Vexing Devil", Location = "Deck", Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new DeckCard { CardId = null, Name = "Skullcrack", Location = "Deck", Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new DeckCard { CardId = null, Name = "Searing Blaze", Location = "Deck", Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new DeckCard { CardId = null, Name = "Rift Bolt", Location = "Deck", Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new DeckCard { CardId = null, Name = "Lightning Bolt", Location = "Deck", Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new DeckCard { CardId = null, Name = "Lava Spike", Location = "Deck", Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new DeckCard { CardId = null, Name = "Hellspark Elemental", Location = "Deck", Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new DeckCard { CardId = null, Name = "Goblin Guide", Location = "Deck", Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new DeckCard { CardId = null, Name = "Eidolon of the Great Revel", Location = "Deck", Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new DeckCard { CardId = null, Name = "Shard Volley", Location = "Deck", Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new DeckCard { CardId = null, Name = "Flames of the Blood Hand", Location = "Deck", Quantity = 4 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new DeckCard { CardId = null, Name = "Anger of the Gods", Location = "SB", Quantity = 1 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new DeckCard { CardId = null, Name = "Leyline of Punishment", Location = "SB", Quantity = 2 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new DeckCard { CardId = null, Name = "Combust", Location = "SB", Quantity = 3 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new DeckCard { CardId = null, Name = "Molten Rain", Location = "SB", Quantity = 3 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new DeckCard { CardId = null, Name = "Searing Blood", Location = "SB", Quantity = 2 });
            InnerTestReadDeck(@"Br4nc4_112564.dec", 18, new DeckCard { CardId = null, Name = "Smash to Smithereens", Location = "SB", Quantity = 4 });
        }

        private void InnerTestReadDeck(string fileName, int expectedNumberOfRows, DeckCard expectedCard)
        {
            var fullName = Path.Combine(TestContext.DeploymentDirectory, fileName);
            var target = new DecReader();
            var result = target.ReadFile(fullName);

            Assert.AreEqual(expectedNumberOfRows, result.Count(), "Wrong number of rows");

            var found = result.Where(r => r.CardId == expectedCard.CardId
                && r.Name == expectedCard.Name
                && r.Quantity == expectedCard.Quantity
                && r.Location == expectedCard.Location);
        }
    }
}