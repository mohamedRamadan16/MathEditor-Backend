namespace Api.Application.Common.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public ApiResponse(T? data, bool success = true, string? message = null)
        {
            Data = data;
            Success = success;
            Message = message;
        }
        public ApiResponse(string message)
        {
            Success = false;
            Message = message;
        }
        public ApiResponse() { }
    }
}
