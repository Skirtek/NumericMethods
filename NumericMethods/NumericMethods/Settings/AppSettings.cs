using System.Collections.Generic;

namespace NumericMethods.Settings
{
    public static class AppSettings
    {
        public static List<string> IntegralPrecisionOptions = new List<string>
        {
            "Niska dokładność (n = 100)",
            "Średnia dokładność (n = 500)",
            "Wysoka dokładność (n = 1000)",
            "Bardzo wysoka dokładność (n = 10 000)",
            "Własna..."
        };

        public const string ArgumentRegex = "([0-9.,]*)[Xx](\\^\\d)?";

        public const string ConstantTermRegex = "([+\\-]+)\\d*$";
    }
}
