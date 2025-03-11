public class FileUpload
{
    public string title { get; set; }
    public string author { get; set; }
    public string summary { get; set; }
    public string? pdflink { get; set; }
    public int CreatedBy { get; set; }  
    public string? keywords { get; set; }
    public int? university { get; set; }
    public DateTime publicationDate { get; set; }

    public IFormFile? Img { get; set; }
    public IFormFile? file { get; set; }

}
