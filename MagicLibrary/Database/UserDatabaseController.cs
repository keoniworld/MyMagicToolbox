using System.IO;

namespace MagicDatabase
{
    public class UserDatabaseController
    {
        public UserDatabaseController(string storagePath)
        {
            UserDatabasePath = storagePath;
            var target = new FileInfo(Path.Combine(storagePath, UserDatabase.SqliteName));
            if (!target.Exists)
            {
                var exeFolder = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Directory.FullName;
                var source = Path.Combine(exeFolder, "APP_DATA", UserDatabase.SqliteName);

                if (File.Exists(source))
                {
                    if (!target.Directory.Exists)
                    {
                        target.Directory.Create();
                    }

                    File.Copy(source, target.FullName);
                }
            }
        }

        public string UserDatabasePath { get; private set; }
    }
}