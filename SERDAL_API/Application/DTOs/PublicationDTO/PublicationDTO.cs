namespace SERDAL_API.Application.DTOs.PublicationDTO
{
    public class PublicationDTO
    {

        public class PublicationFilter //Publication Main Page
        {
            public int page { get; set; } = 1;
            public int pagesize { get; set; } = 10;
            public string? universities { get; set; }
            public string? keywords { get; set; }
            public string? order { get; set; }
            public string? search { get; set; }
        }
    }
}
