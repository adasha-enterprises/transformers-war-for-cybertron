namespace WarForCybertron.Model
{
    public class ServiceResponse<T> where T : class
    {
        public T ResponseEntity { get; set; }
        public string ResponseMessage { get; set; } = string.Empty;

        public ServiceResponse(T responseEntity, string responseMessage)
        {
            this.ResponseEntity = responseEntity;
            this.ResponseMessage = responseMessage;
        }
    }
}
