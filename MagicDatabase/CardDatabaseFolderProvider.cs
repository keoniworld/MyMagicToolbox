using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;

namespace MagicDatabase
{
    [Export(typeof(ICardDatabaseFolderProvider))]
    public class CardDatabaseFolderProvider : ICardDatabaseFolderProvider
    {
        private UserDatabaseController _userDatabaseController;

        public CardDatabaseFolderProvider()
        {
            ExeFolder = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.FullName;
            _userDatabaseController = new UserDatabaseController(Path.Combine(ExeFolder, "USER_DATA"));

            UserCardDatabaseFolder = _userDatabaseController.UserDatabasePath;
            MagicCardDatabaseFolder = Path.Combine(ExeFolder, "APP_DATA");
        }

        public string MagicCardDatabaseFolder
        {
            get;
            private set;
        }

        public string UserCardDatabaseFolder
        {
            get;
            private set;
        }

        public string ExeFolder { get; private set; }
    }
}