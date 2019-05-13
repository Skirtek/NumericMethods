using System.Collections.Generic;
using NumericMethods.Models;

namespace NumericMethods.Interfaces
{
    public interface IExtendedFunctions
    {
        Function PrepareFunction(string formula);

        List<ExtendedOperation> CalculateDerivative(IEnumerable<ExtendedOperation> operations);

        float FunctionResult(float x, float y, List<ExtendedOperation> operations);
    }
}