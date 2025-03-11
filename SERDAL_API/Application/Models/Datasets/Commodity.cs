namespace SERDAL_API.Application.Models.Datasets
{
    public class Commodity
    {
        public int Id { get; set; }
        public string year { get; set; }

        public int dataGroupId { get; set; }
        public DataGroup dataGroup { get; set; }
    }
}
