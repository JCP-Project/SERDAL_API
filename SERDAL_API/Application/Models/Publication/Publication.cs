namespace SERDAL_API.Application.Models.Publication
{
    public class Publication
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Summary { get; set; }
        public DateTime? PublicationDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int Status { get; set; }
        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
        public string ImgPath { get; set; } = string.Empty;
        public string PDFLink { get; set; } = string.Empty;
        public string PDFFile { get; set; } = string.Empty;
        public string Keywords { get; set; } = string.Empty;
        public int University { get; set; }
        public int Download { get; set; }
        public int isDeleted { get; set; }
    }
}
