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
                            Value = GetSingleValue(part),
                            Weight = weight
                        });
                    }
                    else
                    {
                        Operations.Add(new Operation
                        {
                            Value = GetSingleValue(exp),
                            Weight = 1
                        });
                    }
                }

                return PrepareFunctionErrors ? new Function { IsSuccess = false, ResponseCode = FunctionResponse.WrongFunction } : new Function { IsSuccess = true, Operations = Operations };
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

        public List<Operation> CalculateDerivative(IEnumerable<Operation> operations)
        {
            var list = new List<Operation>();
            foreach (var operation in operations)
            {
                if (!(Math.Abs(operation.Weight) > AppSettings.Epsilon))
                {
                    continue;
                }

                list.Add(new Operation
                {
                    Value = operation.Value * operation.Weight,
                    Weight = operation.Weight - 1
                });
            }

            return list;
        }

        public float FunctionResult(float x, List<Operation> operations)
        {
            float result = 0;
            foreach (var operation in operations)
            {
                result += operation.Value * (float)Math.Pow(x, operation.Weight);
            }

            return result;
        }

        private float GetSingleValue(string expression) =>
            expression.Equals("x") || expression.Equals("X")
                ? 1
                : expression.Equals("-x") || expression.Equals("-X")
                    ? -1
                    : GetValue(expression.Remove(expression.Length - 1, 1));

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
