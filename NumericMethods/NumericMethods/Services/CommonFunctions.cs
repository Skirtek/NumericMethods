using System;
using System.Collections.Generic;
using System.Linq;
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
                Operations = new List<Operation>();
                formula = formula.Replace("-x", "-1x")
                    .Replace("-e", "-1e")
                    .Replace("-sin", "-1sin")
                    .Replace("-cos", "-1cos").Replace("-tan", "-1tan").Replace("-ctg", "-1ctg").Replace("-ln", "-1ln");
                formula = formula.Replace("[-1", "[#1");
                var parts = Regex.Split(formula, @"(?=-)|(?=\+)").ToList();
                parts.RemoveAll(string.IsNullOrWhiteSpace);
                var freeExpressions = parts.Where(x => !x.Contains("^") && !x.Contains("x") && !x.Contains("e") && !x.Contains("sin") && !x.Contains("cos") && !x.Contains("tan") && !x.Contains("ctg") && !x.Contains("ln")).ToList();

                if (freeExpressions.Any())
                {
                    foreach (var expression in freeExpressions)
                    {
                        Operations.Add(new Operation
                        {
                            Type = OperationType.Common,
                            Value = GetValue(expression),
                            Weight = 0,
                            IsFreeExpression = true
                        });
                    }
                }

                parts.RemoveAll(x => !x.Contains("^") && !x.Contains("x") && !x.Contains("e") && !x.Contains("sin") && !x.Contains("cos") && !x.Contains("tan") && !x.Contains("ctg") && !x.Contains("ln"));

                for (int i = 0; i < parts.Count; i++)
                {
                    parts[i] = parts[i].Replace("[#1", "[-1");
                }

                foreach (var expression in parts)
                {
                    if (expression.Contains("e"))
                    {
                        var result = ParseExpression(expression);
                        if (!result.IsSuccess)
                        {
                            return new Function { IsSuccess = false, ResponseCode = result.ResponseCode };
                        }

                        Operations.Add(new Operation
                        {
                            Type = OperationType.Euler,
                            Value = GetEulerValue(result.Value, result.Weight),
                        });
                    }
                    else if (expression.Contains("sin"))
                    {
                        var result = ParseTrigonometricExpression(expression, OperationType.Sinus);

                        if (!result.IsSuccess)
                        {
                            return new Function { IsSuccess = false, ResponseCode = result.ResponseCode };
                        }

                        Operations.Add(new Operation
                        {
                            Type = OperationType.Sinus,
                            Value = GetValue(result.Value),
                            Weight = result.Weight,
                            ParenthesesValue = result.ParenthesesValue
                        });
                    }
                    else if (expression.Contains("cos"))
                    {
                        var result = ParseTrigonometricExpression(expression, OperationType.Cosinus);

                        if (!result.IsSuccess)
                        {
                            return new Function { IsSuccess = false, ResponseCode = result.ResponseCode };
                        }

                        Operations.Add(new Operation
                        {
                            Type = OperationType.Cosinus,
                            Value = GetValue(result.Value),
                            Weight = result.Weight,
                            ParenthesesValue = result.ParenthesesValue
                        });
                    }
                    else if (expression.Contains("tan"))
                    {
                        var result = ParseTrigonometricExpression(expression, OperationType.Tangens);

                        if (!result.IsSuccess)
                        {
                            return new Function { IsSuccess = false, ResponseCode = result.ResponseCode };
                        }

                        Operations.Add(new Operation
                        {
                            Type = OperationType.Tangens,
                            Value = GetValue(result.Value),
                            Weight = result.Weight,
                            ParenthesesValue = result.ParenthesesValue
                        });
                    }
                    else if (expression.Contains("ctg"))
                    {
                        var result = ParseTrigonometricExpression(expression, OperationType.Cotangens);

                        if (!result.IsSuccess)
                        {
                            return new Function { IsSuccess = false, ResponseCode = result.ResponseCode };
                        }

                        Operations.Add(new Operation
                        {
                            Type = OperationType.Cotangens,
                            Value = GetValue(result.Value),
                            Weight = result.Weight,
                            ParenthesesValue = result.ParenthesesValue
                        });
                    }
                    else if (expression.Contains("ln"))
                    {
                        var result = ParseTrigonometricExpression(expression, OperationType.NaturalLogarithm);

                        if (!result.IsSuccess)
                        {
                            return new Function { IsSuccess = false, ResponseCode = result.ResponseCode };
                        }

                        Operations.Add(new Operation
                        {
                            Type = OperationType.NaturalLogarithm,
                            Value = GetValue(result.Value),
                            Weight = result.Weight,
                            ParenthesesValue = result.ParenthesesValue
                        });
                    }
                    else
                    {
                        var result = ParseExpression(expression);
                        if (!result.IsSuccess)
                        {
                            return new Function { IsSuccess = false, ResponseCode = result.ResponseCode };
                        }

                        Operations.Add(new Operation
                        {
                            Type = OperationType.Common,
                            Value = GetSingleValue(result.Value),
                            Weight = result.Weight
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
                var derivativeOperation = operation;

                switch (operation.Type)
                {
                    case OperationType.Euler:
                        list.Add(operation);
                        break;
                    case OperationType.Sinus:
                        derivativeOperation.Type = OperationType.Cosinus;
                        list.Add(derivativeOperation);
                        break;
                    case OperationType.Cosinus:
                        derivativeOperation.Type = OperationType.Sinus;
                        derivativeOperation.Value *= -1;
                        list.Add(derivativeOperation);
                        break;
                    case OperationType.Tangens:
                        derivativeOperation.Type = OperationType.Cosinus;
                        derivativeOperation.Weight *= -2;
                        list.Add(derivativeOperation);
                        break;
                    case OperationType.Cotangens:
                        derivativeOperation.Type = OperationType.Sinus;
                        derivativeOperation.Value *= -1;
                        derivativeOperation.Weight *= -2;
                        break;
                    case OperationType.NaturalLogarithm:
                        derivativeOperation.Type = OperationType.Common;
                        derivativeOperation.Weight *= -1;
                        list.Add(derivativeOperation);
                        break;
                    case OperationType.Logarithm:
                        break;
                    case OperationType.Common:
                        if (!operation.IsFreeExpression)
                        {
                            list.Add(new Operation
                            {
                                Type = operation.Type,
                                Value = operation.Value * operation.Weight,
                                Weight = operation.Weight - 1
                            });
                        }
                        break;
                }
            }

            return list;
        }

        public float FunctionResult(float x, List<Operation> operations)
        {
            float result = 0;
            foreach (var operation in operations)
            {
                switch (operation.Type)
                {
                    case OperationType.Euler:
                        result += operation.Value;
                        break;
                    case OperationType.Sinus:
                        result += operation.ParenthesesValue.IsVariable
                            ? operation.Value * (float)Math.Pow(Math.Sin(Math.Pow(operation.ParenthesesValue.NumberValue * x, operation.ParenthesesValue.NumberWeight)), operation.Weight)
                            : operation.Value * (float)Math.Pow(Math.Sin(Math.Pow(operation.ParenthesesValue.NumberValue, operation.ParenthesesValue.NumberWeight)), operation.Weight);
                        break;
                    case OperationType.Cosinus:
                        result += operation.ParenthesesValue.IsVariable
                            ? operation.Value * (float)Math.Pow(Math.Cos(Math.Pow(operation.ParenthesesValue.NumberValue * x, operation.ParenthesesValue.NumberWeight)), operation.Weight)
                            : operation.Value * (float)Math.Pow(Math.Cos(Math.Pow(operation.ParenthesesValue.NumberValue, operation.ParenthesesValue.NumberWeight)), operation.Weight);
                        break;
                    case OperationType.Tangens:
                        result += operation.ParenthesesValue.IsVariable
                            ? operation.Value * (float)Math.Pow(Math.Tan(Math.Pow(operation.ParenthesesValue.NumberValue * x, operation.ParenthesesValue.NumberWeight)), operation.Weight)
                            : operation.Value * (float)Math.Pow(Math.Tan(Math.Pow(operation.ParenthesesValue.NumberValue, operation.ParenthesesValue.NumberWeight)), operation.Weight);
                        break;
                    case OperationType.Cotangens:
                        result += operation.ParenthesesValue.IsVariable
                            ? operation.Value * (float)Math.Pow(1 / Math.Tan(Math.Pow(operation.ParenthesesValue.NumberValue * x, operation.ParenthesesValue.NumberWeight)), operation.Weight)
                            : operation.Value * (float)Math.Pow(1 / Math.Tan(Math.Pow(operation.ParenthesesValue.NumberValue, operation.ParenthesesValue.NumberWeight)), operation.Weight);
                        break;
                    case OperationType.NaturalLogarithm:
                        result += operation.ParenthesesValue.IsVariable
                            ? operation.Value * (float)Math.Pow(Math.Log(Math.Pow(operation.ParenthesesValue.NumberValue * x, operation.ParenthesesValue.NumberWeight)), operation.Weight)
                            : operation.Value * (float)Math.Pow(Math.Log(Math.Pow(operation.ParenthesesValue.NumberValue, operation.ParenthesesValue.NumberWeight)), operation.Weight);
                        break;
                    case OperationType.Logarithm:
                        break;
                    case OperationType.Common:
                        result += operation.Value * (float)Math.Pow(x, operation.Weight);
                        break;
                }
            }

            return result;
        }

        private ParsedExpression ParseTrigonometricExpression(string expression, OperationType type)
        {
            var parenthesesValue = Regex.Matches(expression, "\\[([^)]+)\\]", RegexOptions.RightToLeft);

            if (parenthesesValue.Count != 1)
            {
                return new ParsedExpression { IsSuccess = false, ResponseCode = FunctionResponse.WrongFunction };
            }

            var value = GetParenthesesValue(parenthesesValue[0].Value);

            if (!value.IsSuccess)
            {
                return new ParsedExpression { IsSuccess = false, ResponseCode = FunctionResponse.WrongFunction };
            }

            expression = Regex.Replace(expression, "\\[([^)]+)\\]", "");

            var split = new List<string>();

            switch (type)
            {
                case OperationType.Sinus:
                    split = expression.Split(new[] { "sin" }, StringSplitOptions.None).ToList();
                    break;
                case OperationType.Cosinus:
                    split = expression.Split(new[] { "cos" }, StringSplitOptions.None).ToList();
                    break;
                case OperationType.Tangens:
                    split = expression.Split(new[] { "tan" }, StringSplitOptions.None).ToList();
                    break;
                case OperationType.Cotangens:
                    split = expression.Split(new[] { "ctg" }, StringSplitOptions.None).ToList();
                    break;
                case OperationType.NaturalLogarithm:
                    split = expression.Split(new[] { "ln" }, StringSplitOptions.None).ToList();
                    break;
            }

            split[0] = split[0].Replace("+","1");

            split[0] = string.IsNullOrWhiteSpace(split[0]) ? "1" : split[0];
            split[1] = split[1].Replace("^", "");

            return new ParsedExpression
            {
                IsSuccess = true,
                Value = split[0],
                Weight = string.IsNullOrWhiteSpace(split[1]) ? 1 : GetValue(split[1]),
                ParenthesesValue = value.Data
            };
        }

        private Response<Value> GetParenthesesValue(string expression)
        {
            bool isVariable = expression.Contains("x");

            expression = expression.Replace("x", "").Replace("[", "").Replace("]", "");

            if (!expression.Contains("^"))
            {
                return Response<Value>.Succeeded(new Value { IsVariable = isVariable, NumberValue = string.IsNullOrWhiteSpace(expression) ? 1 : GetValue(expression), NumberWeight = 1 });
            }

            var split = expression.Split('^');
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
                    return Response<Value>.Failed(string.Empty);
                }

                weight = up / down;
            }
            else if (Regex.Match(split[1], "\\(.*?\\)").Success)
            {
                return Response<Value>.Failed(string.Empty);

            }
            else
            {
                if (!float.TryParse(split[1].Replace(".", ","), out weight))
                {
                    return Response<Value>.Failed(string.Empty);
                }
            }

            return Response<Value>.Succeeded(new Value { IsVariable = isVariable, NumberValue = GetValue(part), NumberWeight = weight });
        }

        private ParsedExpression ParseExpression(string expression)
        {
            if (!expression.Contains("^"))
            {
                return new ParsedExpression { IsSuccess = true, Value = expression, Weight = 1 };
            }

            var split = expression.Split('^');
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
                    return new ParsedExpression { IsSuccess = false, ResponseCode = FunctionResponse.DivideByZero };
                }

                weight = up / down;
            }
            else if (Regex.Match(split[1], "\\(.*?\\)").Success)
            {
                return new ParsedExpression { IsSuccess = false, ResponseCode = FunctionResponse.UnclosedParentheses };
            }
            else
            {
                if (!float.TryParse(split[1].Replace(".", ","), out weight))
                {
                    return new ParsedExpression { IsSuccess = false, ResponseCode = FunctionResponse.WrongFunction };
                }
            }

            return new ParsedExpression { IsSuccess = true, Value = part, Weight = weight };
        }

        private float GetSingleValue(string expression)
        {
            expression = expression.Replace("+", "").ToLower();
            return expression.Equals("x")
                ? 1
                : expression.Equals("-x")
                    ? -1
                    : GetValue(expression.Remove(expression.Length - 1, 1));
        }

        private float GetEulerValue(string expression, float weight)
        {
            expression = expression.Replace("+", "").ToLower();
            return expression.Equals("e")
                ? (float)Math.Exp(weight)
                : expression.Equals("-e")
                    ? -1 * (float)Math.Exp(weight)
                    : GetValue(expression.Remove(expression.Length - 1, 1)) * (float)Math.Exp(weight);
        }

        private float GetValue(string expression)
        {
            if (expression.Substring(0, 1).Equals("+"))
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
