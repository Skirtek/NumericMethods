namespace NumericMethods.Models
{
    public class ExtendedOperation : Operation
    {
        public bool IsY { get; set; }

        public Multiplying Multiplication { get; set; }

        public bool HasMultiplication { get; set; }
    }
}