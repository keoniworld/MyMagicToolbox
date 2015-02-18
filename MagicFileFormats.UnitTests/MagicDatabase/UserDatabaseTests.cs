using System;
using MagicDatabase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MagicFileFormats.UnitTests.MagicDatabase
{
    [TestClass]
    public class UserDatabaseTests
    {
        public TestContext TestContext { get; set; }

        [DeploymentItem(@"System.Data.SQLite")]
        [DeploymentItem(@"x64\SQLite.Interop.dll", @"x64")]
        [DeploymentItem(@"x86\SQLite.Interop.dll", @"x86")]
        [DeploymentItem(@"App_Data\UserDatabase.s3db")]
        [TestMethod]
        public void TestAccessSettingsInUserDatabase()
        {
            var folderProvider = new Mock<ICardDatabaseFolderProvider>();
            folderProvider.Setup(p => p.UserCardDatabaseFolder).Returns(TestContext.DeploymentDirectory);

            var userDatabase = new UserDatabase(folderProvider.Object);
            // var settings = new ApplicationSettings(userDatabase);

            // Test new, unknown value
            var uniqueId = Guid.NewGuid().ToString();
            var found = userDatabase.InternalGetSettingsValue(uniqueId);
            Assert.IsNull(found);

            // Now insert a value there
            const string insertedValue = "my cool value";
            userDatabase.SetSettingsValue(uniqueId, insertedValue);

            // And retrieve it again:
            found = userDatabase.InternalGetSettingsValue(uniqueId);
            Assert.IsNotNull(found);
            Assert.AreEqual(insertedValue, found.Value);

            // Insert a second value:
            const string insertedValue2 = "my other cool value";
            userDatabase.SetSettingsValue(uniqueId, insertedValue2);
            found = userDatabase.InternalGetSettingsValue(uniqueId);
            Assert.IsNotNull(found);
            Assert.AreEqual(insertedValue2, found.Value);
        }
    }
}
