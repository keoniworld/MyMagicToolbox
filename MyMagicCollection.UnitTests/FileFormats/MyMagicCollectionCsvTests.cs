using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyMagicCollection.Shared.FileFormats.MyMagicCollection;
using MyMagicCollection.Shared.Helper;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.UnitTests
{
    [TestClass]
    public class MyMagicCollectionCsvTests
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        /// <value>The test context.</value>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestMyMagicCollectionCsvWrite()
        {
            var cards = new[] {
                new MagicCollectionCard()
                {
                    RowId="row1",
                    CardId = "card id 1",
                    Grade = MagicGrade.LightlyPlayed,
                },
                new MagicCollectionCard()
                {
                    RowId="row2",
                    CardId = "card id 2",
                    Grade = MagicGrade.NearMint,
                    Language = MagicLanguage.German,
                    QuantityTrade = 1,
                }
            }.ToList();

            var collection = new MagicCollection(cards)
            {
                Name = "My collection",
            };

            var targetFileName = Path.Combine(TestContext.DeploymentDirectory, "CsvTest " + Guid.NewGuid().ToString() + ".csv");
            var target = new MyMagicCollectionCsv();

            target.WriteFile(targetFileName, collection);

            var referenceFile = GetType().Assembly.LoadEmbeddedResourceTextFile("TestMyMagicCollectionCsvWrite.csv");
            var writtenFile = File.ReadAllText(targetFileName);

            Assert.AreEqual(referenceFile, writtenFile);
        }

        [TestMethod]
        public void TestMyMagicCollectionCsvRead()
        {
            var targetFileName = Path.Combine(TestContext.DeploymentDirectory, "CsvTest " + Guid.NewGuid().ToString() + ".csv");
            var referenceFile = GetType().Assembly.LoadEmbeddedResourceTextFile("TestMyMagicCollectionCsvWrite.csv");

            File.WriteAllText(targetFileName, referenceFile, Encoding.UTF8);

            var target = new MyMagicCollectionCsv();
            var found = target.ReadFile(targetFileName);

            Assert.AreEqual("My collection", found.Name);
            Assert.AreEqual(2, found.Cards.Count);

            Assert.AreEqual("card id 1", found.Cards[0].CardId);
            Assert.AreEqual("card id 2", found.Cards[1].CardId);
        }
    }
}