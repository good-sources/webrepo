namespace AggregationServiceClient
{
    using System.Configuration;

    static class ApplicationSettingsManager
    {
        public static string Pick(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static void Save(string key, string value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            settings[key].Value = value;

            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
    }
}
