using System;
namespace MagicContracts
{
    public interface IApplicationSettings
    {
        string GetValue(string key, string defaultValue);
        void SetValue(string key, string value);
    }
}
