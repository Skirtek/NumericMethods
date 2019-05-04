﻿using System.Collections.Generic;

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

        public static List<string> NonLinearEquationPrecisionOptions = new List<string>
        {
            "Niska dokładność",
            "Średnia dokładność",
            "Wysoka dokładność",
        };

        public const string ArgumentRegex = "([+\\-]?)([0-9.,]*)[Xx](\\^([0-9]*[.,])?[0-9]+|\\^\\(([0-9]*[.,])?[0-9]+/([0-9]*[.,])?[0-9]+\\))?";

        public const string ConstantTermRegex = "[\\^]?[+\\-]?([0-9]*[.,])?[0-9]+$";

        public const string FormulaRegex = "^[xX(),.+-^0-9]+$";

        public const int DefaultEntryMaxLength = 7;

        #region Messages
        public const string ChangeEquationNumber = nameof(ChangeEquationNumber);

        public const string ChangePlotSize = nameof(ChangePlotSize);

        public const string ChangeDomainSize = nameof(ChangeDomainSize);
        #endregion

        public const double Epsilon = 0.00001;

        public const int InitialX = -25;
    }
}
