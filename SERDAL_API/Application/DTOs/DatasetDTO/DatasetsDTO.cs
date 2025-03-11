namespace SERDAL_API.Application.DTOs.DatasetDTO
{
    public class DatasetsDTO
    {
    }

    public class DataSets
    {
        public int id { get; set; }
        public string? title { get; set; }

        public List<DataGroupDTO> dataGroup { get; set; }

    }


    public class DataGroupDTO
    {
        public string? production { get; set; }
        public string? description { get; set; }
        public List<string> dataYear { get; set; }
        public List<string> dataName { get; set; }
        public List<string> dataVariable { get; set; }

        public List<Series?> series { get; set; } = new List<Series?>();
    }

    public class Series
    {
        public string name { get; set; }
        public List<double?> data { get; set; } = new List<double?>();
    }

}
