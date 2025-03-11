using SERDAL_API.Application.Models.Survey;

namespace SERDAL_API.Application.DTOs.PublicationDTO
{
    public class ReturnAnswerSheetDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<ReturnFieldDTO> Fields { get; set; } = new List<ReturnFieldDTO>();
        public int isActive { get; set; }
        public int isDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

    }

    public class ReturnFieldDTO
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public int Index { get; set; }
        public string Question { get; set; }
        public bool Required { get; set; }
        public string SelectedType { get; set; }
        public string[] AnswerOptions { get; set; } = Array.Empty<string>();
    }

}
