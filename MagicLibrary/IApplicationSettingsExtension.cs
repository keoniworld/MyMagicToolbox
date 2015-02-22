using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace MagicLibrary
{
    public static class IApplicationSettingsExtension
    {
        public static int GetIntValue(this IApplicationSettings settings, string key, int defaultValue)
        {
            return Convert.ToInt32(
                settings.GetValue(key, defaultValue.ToString(CultureInfo.InvariantCulture)),
                CultureInfo.InvariantCulture);
        }

        public static void SetIntValue(this IApplicationSettings settings, string key, int value)
        {
            settings.SetValue(key, value.ToString(CultureInfo.InvariantCulture));
        }

        public static bool GetBoolValue(this IApplicationSettings settings, string key, bool defaultValue)
        {
            return Convert.ToBoolean(
                settings.GetValue(key, defaultValue.ToString(CultureInfo.InvariantCulture)),
                CultureInfo.InvariantCulture);
        }

        public static void SetBooleanValue(this IApplicationSettings settings, string key, bool value)
        {
            settings.SetValue(key, value.ToString(CultureInfo.InvariantCulture));
        }

        // Concrete values:
        public static string GetImageCacheFolder(this IApplicationSettings settings)
        {
            var found = settings.GetValue("ImageCacheFolder_" + Environment.MachineName, null);
            if (!string.IsNullOrWhiteSpace(found))
            {
                return found;
            }

            // Get exe folder as default
            var folder = Path.Combine(
                new FileInfo(Assembly.GetEntryAssembly().Location).Directory.FullName,
                "USER_DATA",
                "CardCache");

            return folder;
        }

        public static string GetCurrentCollection(this IApplicationSettings settings)
        {
            return settings.GetValue("CurrentCollection", null);
        }

        public static void SetCurrentCollection(this IApplicationSettings settings, string collectionName)
        {
            settings.SetValue("CurrentCollection", collectionName);
        }

    }
}