namespace SERDAL_API.Application.Models.Datasets
{
    public class Variable
    {
        public int Id { get; set; }
        public string name { get; set; }
        public List<VariableValue>? variableValue { get; set; } = new List<VariableValue>();


        public int dataGroupId { get; set; }
        public DataGroup dataGroup { get; set; }
    }
}
