namespace SERDAL_API.Application.Models.Survey
{
    public class Survey
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public ICollection<Field> Fields { get; set; }
        public int isActive { get; set; }
        public int isDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

}
