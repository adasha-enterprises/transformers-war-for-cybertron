namespace WarForCybertron.Model
{
    public class ServiceResponse<T> where T : class
    {
        public T ResponseEntity { get; set; }
        public string ResponseMessage { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = false;

        public ServiceResponse(T responseEntity, string responseMessage, bool isSuccess)
        {
            this.ResponseEntity = responseEntity;
            this.ResponseMessage = responseMessage;
            this.IsSuccess = isSuccess;
        }
    }
}
