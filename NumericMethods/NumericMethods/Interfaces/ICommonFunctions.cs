using System.Collections.Generic;
using NumericMethods.Models;

namespace NumericMethods.Interfaces
{
    public interface ICommonFunctions
    {
        Function PrepareFunction(string formula);

        List<Operation> CalculateDerivative(IEnumerable<Operation> operations);

        float FunctionResult(float x, List<Operation> operations);
    }
}