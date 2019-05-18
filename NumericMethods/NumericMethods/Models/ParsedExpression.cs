using NumericMethods.Enums;

namespace NumericMethods.Models
{
    public class ParsedExpression
    {
        public bool IsSuccess { get; set; }

        public FunctionResponse ResponseCode { get; set; }

        public string Value { get; set; }

        public float Weight { get; set; }

        public Value ParenthesesValue { get; set; }
    }
}
