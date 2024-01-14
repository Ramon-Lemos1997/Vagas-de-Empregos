namespace Domain.Models.Support
{
    public class ResultModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public ResultModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }

}
