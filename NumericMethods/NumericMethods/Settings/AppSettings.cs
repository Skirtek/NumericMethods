using System.Collections.Generic;
using Xamarin.Forms;

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

        public const string ArgumentRegex = "([+\\-]?)([0-9.,]*)[Xx](\\^\\d)?";

        public const string ConstantTermRegex = "[+\\-]?([0-9]*[.,])?[0-9]+$";

        public const string FormulaRegex = "^[xX^,.+\\-0-9]+$";

        public const int DefaultEntryMaxLength = 7;
    }
}
