namespace SERDAL_API.Application.Models.Datasets
{
    public class DataGroup
    {
        public int Id { get; set; }
        public string? Production { get; set; }
        public string? Description { get; set; }

        public List<Commodity>? Commodity { get; set; } = new List<Commodity>();
        public List<Variable>? Variable { get; set; } = new List<Variable>();

        public int chartDatasetId { get; set; }
        public ChartDataset chartDataset { get; set; }
    }

}
