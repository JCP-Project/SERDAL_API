namespace SERDAL_API.Application.DTOs.Survey
{
    public class SurveyCreate
    {
        public string title { get; set; }
        public string description { get; set; }
        public List<FieldDto> fields { get; set; }
    }

    public class FieldDto
    {
        public int ID { get; set; }
        public string Question { get; set; }
        public bool Required { get; set; }
        public string SelectedType { get; set; }
        public List<string>? AnswerOption { get; set; } // List of options for checkbox/radio
        //public string? AddOption { get; set; } // Additional option, if applicable
        //public string? Answer { get; set; } // User's answer, if applicable
        //public List<string>? AnswerType { get; set; } // List of possible answer types (e.g., short text, long text, etc.)
    }
}
