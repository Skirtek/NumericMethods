using NumericMethods.Interfaces;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace NumericMethods.Settings
{
    public class UserSettings : IUserSettings
    {
        private readonly ISettings _settings;

        public UserSettings(ISettings settings)
        {
            _settings = settings;
        }

        private static IUserSettings _instance;
        public static IUserSettings Instance => _instance ?? (_instance = new UserSettings(CrossSettings.Current));

    }
}
