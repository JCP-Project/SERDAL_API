namespace SERDAL_API.Application.Models.Survey
{
    public class Field
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public Survey Survey { get; set; }

        public int Index { get; set; }
        public string Question { get; set; }
        public bool Required { get; set; }
        public string SelectedType { get; set; }
        public string? AnswerOptions { get; set; }
    }

}
