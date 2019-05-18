namespace NumericMethods.Models
{
    public class Response<T>
    {
        public bool IsSuccess { get; set; }

        public T Data { get; set; }

        public string Message { get; set; }

        public static Response<T> Succeeded(T data)
            => new Response<T>
            {
                IsSuccess = true,
                Data = data
            };

        public static Response<T> Failed(string message)
            => new Response<T>
            {
                IsSuccess = false,
                Message = message
            };
    }
}
