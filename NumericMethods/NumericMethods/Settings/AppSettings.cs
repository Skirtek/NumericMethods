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

        public const string ArgumentRegex = "([+\\-]?)([0-9.,]*)[Xx](\\^([0-9]*[.,])?[0-9]+|\\^\\(([0-9]*[.,])?[0-9]+/([0-9]*[.,])?[0-9]+\\))?";

        public const string ConstantTermRegex = "[\\^]?[+\\-]?([0-9]*[.,])?[0-9]+$";

        public const string FormulaRegex = "^[xX(),.+-^0-9]+$";

        public const int DefaultEntryMaxLength = 7;

        public const string ChangeEquationNumber = nameof(ChangeEquationNumber);

        public const string ChangePlotSize = nameof(ChangePlotSize);

        public const double Epsilon = 0.00000000001;
    }
}
