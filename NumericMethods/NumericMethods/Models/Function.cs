using System.Collections.Generic;
using NumericMethods.Enums;

namespace NumericMethods.Models
{
    public class Function
    {
        public bool IsSuccess { get; set; }

        public List<Operation> Operations { get; set; }

        public FunctionResponse ResponseCode { get; set; }

        public List<ExtendedOperation> ExtendedOperations { get; set; }
    }
}