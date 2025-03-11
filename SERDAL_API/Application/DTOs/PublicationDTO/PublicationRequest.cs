namespace SERDAL_API.Application.DTOs.PublicationDTO
{
    public class PublicationRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Summary { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int Status { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ImgPath { get; set; } = string.Empty;
        public string PDFLink { get; set; } = string.Empty;
        public string PDFFile { get; set; } = string.Empty;
        public string Keywords { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
    }
}
