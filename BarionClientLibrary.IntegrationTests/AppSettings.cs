using System;
using System.Configuration;

namespace BarionClientTester
{
    public static class AppSettings
    {
        public static string BarionBaseAddress => GetAppSettings("BarionBaseAddress");
        public static string BarionPOSKey => GetAppSettings("BarionPOSKey");
        public static string BarionPayee => GetAppSettings("BarionPayee");
        public static string BarionPayer => GetAppSettings("BarionPayer");
        public static string BarionPayerPassword => GetAppSettings("BarionPayerPassword");

        private static string GetAppSettings(string key)
        {
            var environmentalVariableValue = Environment.GetEnvironmentVariable(key);

            if(string.IsNullOrEmpty(environmentalVariableValue))
            {
                return ConfigurationManager.AppSettings[key];
            }

            return environmentalVariableValue;
        }
    }
}
