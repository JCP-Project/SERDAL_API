namespace SERDAL_API.Application.Services
{
    public class ServiceReponse
    {
        public class ServiceResponse<T>
        {
            public int Code { get; set; }
            public bool Success { get; set; }
            public T Data { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
