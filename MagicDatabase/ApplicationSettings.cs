using System.ComponentModel.Composition;
using MagicContracts;

namespace MagicDatabase
{
    [Export(typeof(IApplicationSettings))]
    public class ApplicationSettings : IApplicationSettings
    {
        private readonly IUserDatabase _userDatabase;

        [ImportingConstructor]
        public ApplicationSettings(
            IUserDatabase userDatabase)
        {
            _userDatabase = userDatabase;
        }

        public string GetValue(string key, string defaultValue)
        {
            var found = _userDatabase.GetSettingsValue(key);
            if (found == null)
            {
                return defaultValue;
            }

            return found;
        }

        public void SetValue(string key, string value)
        {
            _userDatabase.SetSettingsValue(key, value);
        }
    }
}