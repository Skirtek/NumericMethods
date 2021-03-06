﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NumericMethods.Enums;
using NumericMethods.Interfaces;
using NumericMethods.Models;
using NumericMethods.Settings;

namespace NumericMethods.Services
{
    public class ExtendedFunctions : IExtendedFunctions
    {
        private bool PrepareFunctionErrors { get; set; }

        private List<ExtendedOperation> Operations = new List<ExtendedOperation>();

        public Function PrepareFunction(string formula)
        {
            try
            {
                formula = formula.Replace("-x", "-1x").Replace("-y", "-1y");
                var parts = Regex.Split(formula, @"(?=-)|(?=\+)").ToList();
                parts.RemoveAll(string.IsNullOrWhiteSpace);
                var freeExpressions = parts.Where(x => !x.Contains("^") && !x.Contains("x") && !x.Contains("y")).ToList();

                if (freeExpressions.Any())
                {
                    foreach (var expression in freeExpressions)
                    {
                        Operations.Add(new ExtendedOperation
                        {
                            Value = GetValue(expression),
                            Weight = 0
                        });
                    }
                }

                parts.RemoveAll(x => !x.Contains("^") && !x.Contains("x") && !x.Contains("y"));

                foreach (var expression in parts)
                {
                    if (expression.Contains("x") && expression.Contains("y"))
                    {
                        var ingredients = Regex.Split(expression, "\\*").ToList();
                        var result = new Multiplying { IsPositive = ingredients[0].Substring(0, 1).Equals("+") };
                        ingredients[0] = ingredients[0].Replace("+","");

                        foreach (var exp in ingredients)
                        {
                            bool isX = exp.Contains("x");
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

                                if (isX)
                                {
                                    result.XValue = GetSingleValue(part);
                                    result.XWeight = weight;
                                }
                                else
                                {
                                    result.YValue = GetSingleValue(part);
                                    result.YWeight = weight;
                                }

                            }
                            else
                            {
                                if (isX)
                                {
                                    result.XValue = GetSingleValue(exp);
                                    result.XWeight = 1;
                                }
                                else
                                {
                                    result.YValue = GetSingleValue(exp);
                                    result.YWeight = 1;
                                }
                            }
                        }

                        Operations.Add(new ExtendedOperation { Multiplication = result, HasMultiplication = true });
                    }
                    else
                    {
                        if (expression.Contains("^"))
                        {
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

                            Operations.Add(new ExtendedOperation
                            {
                                Value = GetSingleValue(part),
                                Weight = weight,
                                IsY = expression.Contains("y")
                            });
                        }
                        else
                        {
                            Operations.Add(new ExtendedOperation
                            {
                                Value = GetSingleValue(expression),
                                Weight = 1,
                                IsY = expression.Contains("y")
                            });
                        }
                    }
                }

                return PrepareFunctionErrors ? new Function { IsSuccess = false, ResponseCode = FunctionResponse.WrongFunction } : new Function { IsSuccess = true, ExtendedOperations = Operations };
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

        public List<ExtendedOperation> CalculateDerivative(IEnumerable<ExtendedOperation> operations)
        {
            var list = new List<ExtendedOperation>();

            foreach (var operation in operations)
            {
                if (operation.HasMultiplication)
                {
                    list.Add(new ExtendedOperation
                    {
                        HasMultiplication = true,
                        Multiplication = new Multiplying
                        {
                            IsPositive = operation.Multiplication.IsPositive,
                            YValue = operation.Multiplication.YValue,
                            YWeight = operation.Multiplication.YValue,
                            XValue = operation.Multiplication.XValue * operation.Multiplication.XWeight,
                            XWeight = operation.Multiplication.XWeight - 1
                        }
                    });
                }
                else if (Math.Abs(operation.Weight) > AppSettings.Epsilon && !operation.IsY)
                {
                    list.Add(new ExtendedOperation
                    {
                        Value = operation.Value * operation.Weight,
                        Weight = operation.Weight - 1,
                        IsY = operation.IsY
                    });
                }
            }

            return list;
        }

        public float FunctionResult(float x, float y, List<ExtendedOperation> operations)
        {
            float result = 0;
            foreach (var operation in operations)
            {
                if (operation.HasMultiplication)
                {
                    var value = operation.Multiplication.XValue *
                                (float)Math.Pow(x, operation.Multiplication.XWeight) *
                                operation.Multiplication.YValue * (float)Math.Pow(y, operation.Multiplication.YWeight);

                    result += operation.Multiplication.IsPositive ? value : value * -1;
                }
                else
                {
                    result += operation.IsY ? operation.Value * (float)Math.Pow(y, operation.Weight) : operation.Value * (float)Math.Pow(x, operation.Weight);
                }
            }

            return result;
        }

        private float GetSingleValue(string expression)
        {
            expression = expression.Replace("+", "").ToLower();
            return expression.Equals("x") || expression.Equals("y")
                ? 1
                : expression.Equals("-x") || expression.Equals("-y")
                    ? -1
                    : GetValue(expression.Remove(expression.Length - 1, 1));
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
