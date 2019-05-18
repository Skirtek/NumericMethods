using NumericMethods.Enums;

namespace NumericMethods.Models
{
    public class Operation
    {
        public OperationType Type { get; set; }

        public float Value { get; set; }

        public Value ParenthesesValue { get; set; }

        public float Weight { get; set; }

        public bool IsFreeExpression { get; set; }
    }
}