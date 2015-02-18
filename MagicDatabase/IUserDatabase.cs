namespace MagicDatabase
{
    public interface IUserDatabase
    {
        string GetSettingsValue(string key);

        void SetSettingsValue(string key, string value);

        void Dispose();
    }
}