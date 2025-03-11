namespace SERDAL_API.Application.Models.User
{
    public class User
    {
        public int ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? IsActive { get; set; }
        public string? Role { get; set; }
        public string? Img { get; set; } = string.Empty;
        public DateTime? CreateDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public int? ModifiedBy { get; set; }
        public int? university { get; set; }
    }
}
