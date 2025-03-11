namespace SERDAL_API.Application.Models.Datasets
{
    public class VariableValue
    {
        public int Id { get; set; }
        public double? value { get; set; }

        public int variableId { get; set; }
        public Variable? variable { get; set; }
    }
}
