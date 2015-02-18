using MagicDatabase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MagicFileFormats.UnitTests.MagicDatabase
{
    [TestClass]
    public class ApplicationSettingsTests
    {
        [TestMethod]
        public void TestSetValue()
        {
            var database = new Mock<IUserDatabase>();

            var target = new ApplicationSettings(database.Object);
            var key = "my key";
            var value = "my value";

            target.SetValue(key, value);
            database.Verify(d => d.SetSettingsValue(key, value), Times.Once());
        }

        [TestMethod]
        public void TestGetValueWithDefault()
        {
            var key = "my key";
            var database = new Mock<IUserDatabase>();
            database.Setup(d => d.GetSettingsValue(key)).Returns((string)null);

            var target = new ApplicationSettings(database.Object);
            var value = "my value";

            var result = target.GetValue(key, value);
            Assert.AreEqual(value, result);
            database.Verify(d => d.GetSettingsValue(key), Times.Once());
        }

        [TestMethod]
        public void TestGetValueWithFoundValue()
        {
            var key = "my key";
            var value = "my value";
            var defaultValue = "my default";
            var database = new Mock<IUserDatabase>();
            database.Setup(d => d.GetSettingsValue(key)).Returns(value);

            var target = new ApplicationSettings(database.Object);

            var result = target.GetValue(key, defaultValue);
            Assert.AreEqual(value, result);
            database.Verify(d => d.GetSettingsValue(key), Times.Once());
        }
    }
}