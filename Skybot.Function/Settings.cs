using System.Configuration;

namespace Skybot.Function
{
    public static class Settings
    {
        public static string FromNumber => ConfigurationManager.AppSettings["TextNumbersFrom"];
        public static string SkybotAuthUri => ConfigurationManager.AppSettings["SkybotAuthUri"];
        public static string SkybotUri => ConfigurationManager.AppSettings["SkybotUri"];
        public static string ClientId => ConfigurationManager.AppSettings["ClientId"];
        public static string ClientSecret => ConfigurationManager.AppSettings["ClientSecret"];
        public static string SkysendConnectionString => ConfigurationManager.AppSettings["SkybotBusConnectionString"];
    }
}
