namespace SERDAL_API.Application.Models.Datasets
{
    public class ChartDataset
    {
        public int Id { get; set; }
        public string? Title { get; set; }

        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int IsDeleted { get; set; } = 0;

        public List<DataGroup> dataGroups { get; set; }
    }












}
