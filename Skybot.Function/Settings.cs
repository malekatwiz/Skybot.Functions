using System.Configuration;

namespace Skybot.Function
{
    public static class Settings
    {
        public static string FromNumber => ConfigurationManager.AppSettings["TextNumbersFrom"];
        public static string ToNumber => ConfigurationManager.AppSettings["TextNumbersTo"];
        public static string TextoApiUri => ConfigurationManager.AppSettings["TextApi"];
        public static string SkybotApiUri => ConfigurationManager.AppSettings["SkybotApi"];
        public static string SkybotClientId => ConfigurationManager.AppSettings["SkybotAppCredentialsClientId"];
        public static string SkybotClientSecret => ConfigurationManager.AppSettings["SkybotAppCredentialsClientSecret"];
        public static string SkybotClientIdentifier => ConfigurationManager.AppSettings["SkybotAppCredentialsIdentifier"];
        public static string TextoClientId => ConfigurationManager.AppSettings["TextoCredentialsClientId"];
        public static string TextoClientSecret => ConfigurationManager.AppSettings["TextoCredentialsClientSecret"];
        public static string TextoClientIdentifier => ConfigurationManager.AppSettings["TextoCredentialsIdentifier"];
    }
}
