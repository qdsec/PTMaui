using Microsoft.Maui.Storage;

namespace PeterTours.Utils
{
    public static class Settings
    {
        private const string LastCIKey = "last_ci_key";
        private const string LastPWKey = "last_pw_key";
        private const string LastNameKey = "last_name_key";
        private const string LastTelfKey = "last_telf_key";
        private const string LastLogKey = "last_log_key";
        private const string Default = "";

        public static string LastUsedCI
        {
            get => Preferences.Get(LastCIKey, Default);
            set => Preferences.Set(LastCIKey, value);
        }

        public static string LastUsedPW
        {
            get => Preferences.Get(LastPWKey, Default);
            set => Preferences.Set(LastPWKey, value);
        }

        public static string LastUsedName
        {
            get => Preferences.Get(LastNameKey, Default);
            set => Preferences.Set(LastNameKey, value);
        }

        public static string LastUsedTelf
        {
            get => Preferences.Get(LastTelfKey, Default);
            set => Preferences.Set(LastTelfKey, value);
        }

        public static string LastUsedLog
        {
            get => Preferences.Get(LastLogKey, Default);
            set => Preferences.Set(LastLogKey, value);
        }
    }
}
