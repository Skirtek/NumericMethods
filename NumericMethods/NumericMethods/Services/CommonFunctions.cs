using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NumericMethods.Enums;
using NumericMethods.Interfaces;
using NumericMethods.Models;
using NumericMethods.Settings;

namespace NumericMethods.Services
{
    public class CommonFunctions : ICommonFunctions
    {
        private bool PrepareFunctionErrors { get; set; }

        private List<Operation> Operations = new List<Operation>();

        public Function PrepareFunction(string formula)
        {
            try
            {
                string lastExpression = Regex.Match(formula, AppSettings.ConstantTermRegex, RegexOptions.RightToLeft)
                    .ToString();

                if (!string.IsNullOrWhiteSpace(lastExpression) && !lastExpression.Contains("^"))
                {
                    Operations.Add(new Operation
                    {
                        IsNegative = IsNegative(lastExpression),
                        Value = GetValue(lastExpression),
                        Weight = 0
                    });
                }

                var expressions = Regex.Matches(formula, AppSettings.ArgumentRegex);

                foreach (var expression in expressions)
                {
                    string exp = expression.ToString();

                    if (exp.Contains("^"))
                    {
                        var split = exp.Split('^');
                        string part = split[0];
                        float weight;

                        if (Regex.Match(split[1], "\\(([0-9]*[.,])?[0-9]+/([0-9]*[.,])?[0-9]+\\)").Success)
                        {
                            split[1] = split[1].Remove(split[1].Length - 1, 1).Remove(0, 1);
                            var components = split[1].Split('/');
                            float.TryParse(components[0], out float up);
                            float.TryParse(components[1], out float down);

                            if (Math.Abs(down) < AppSettings.Epsilon)
                            {
                                return new Function { IsSuccess = false, ResponseCode = FunctionResponse.DivideByZero };
                            }

                            weight = up / down;
                        }
                        else if (Regex.Match(split[1], "\\(.*?\\)").Success)
                        {
                            return new Function { IsSuccess = false, ResponseCode = FunctionResponse.UnclosedParentheses };
                        }
                        else
                        {
                            if (!float.TryParse(split[1].Replace(".", ","), out weight))
                            {
                                return new Function { IsSuccess = false, ResponseCode = FunctionResponse.WrongFunction };
                            }
                        }

                        Operations.Add(new Operation
                        {
                            IsNegative = IsNegative(part),
                            Value =
                                part.Equals("x") || part.Equals("X") ? 1 : GetValue(part.Remove(part.Length - 1, 1)),
                            Weight = weight
                        });
                    }
                    else
                    {
                        Operations.Add(new Operation
                        {
                            IsNegative = IsNegative(exp),
                            Value = exp.Equals("x") || exp.Equals("X") ? 1 : GetValue(exp.Remove(exp.Length - 1, 1)),
                            Weight = 1
                        });
                    }
                }

                if (PrepareFunctionErrors)
                {
                    return new Function { IsSuccess = false, ResponseCode = FunctionResponse.WrongFunction };
                }

                return new Function { IsSuccess = true, Operations = Operations };
            }
            catch (DivideByZeroException)
            {
                return new Function { IsSuccess = false, ResponseCode = FunctionResponse.DivideByZero };
            }
            catch (Exception ex)
            {
                var exception = ex.Message;
                return new Function { IsSuccess = false, ResponseCode = FunctionResponse.CriticalError };
            }
        }
        private bool IsNegative(string expression) => expression.Substring(0, 1).Equals("-");

        private float GetValue(string expression)
        {
            if (expression.Substring(0, 1).Equals("-") || expression.Substring(0, 1).Equals("+"))
            {
                expression = expression.Remove(0, 1);
            }

            if (expression.Contains("."))
            {
                expression = expression.Replace(".", ",");
            }

            PrepareFunctionErrors = !float.TryParse(expression, out float value);

            return value;
        }
    }
}
