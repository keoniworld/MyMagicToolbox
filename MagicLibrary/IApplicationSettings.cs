using System;
namespace MagicLibrary
{
    public interface IApplicationSettings
    {
        string GetValue(string key, string defaultValue);
        void SetValue(string key, string value);
    }
}
