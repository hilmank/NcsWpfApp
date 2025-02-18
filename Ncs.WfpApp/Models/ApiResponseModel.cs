namespace Ncs.WpfApp.Models
{
    public class ApiResponseModel<T>
    {
        public bool Success { get; set; }
        public string? ErrorCode { get; set; }
        public string? Message { get; set; }
        public string? MessageDetail { get; set; }
        public T? Data { get; set; }
    }
}
