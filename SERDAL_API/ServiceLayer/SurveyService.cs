using Microsoft.EntityFrameworkCore;
using SERDAL_API.Application.DTOs.Survey;
using SERDAL_API.Application.Models.Publication;
using SERDAL_API.Application.Models.Survey;
using SERDAL_API.Data;
using static SERDAL_API.Application.Services.ServiceReponse;

namespace SERDAL_API.ServiceLayer
{
    public class SurveyService
    {
        private readonly DataContext _context;
        public SurveyService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<Survey>> CreateSurvey(SurveyCreate survey)
        {
            DateTime dt = DateTime.Now;
            try
            {
                bool isSurveyExist = await _context.survey.AnyAsync(x => x.Title.ToUpper().Trim() == survey.title.ToUpper().Trim() && x.isActive == 1 && x.isDeleted == 0);

                if (isSurveyExist)
                {
                    return new ServiceResponse<Survey>
                    {
                        Code = 404,
                        Success = false,
                        ErrorMessage = "Title is already exist"
                    };
                }

                var Createsurvey = new Survey
                {
                    Title = survey.title,
                    Description = survey.description,
                    CreatedDate = dt,
                    isActive = 1,
                    isDeleted = 0,
                    Fields = new List<Field>()
                    
                };

                foreach (var fieldDTO in survey.fields)
                {
                    Field field = new Field();

                    field.Index = fieldDTO.ID;
                    field.Question = fieldDTO.Question;
                    field.Required = fieldDTO.Required;
                    field.SelectedType = fieldDTO.SelectedType;

                    if (fieldDTO.AnswerOption != null)
                    {
                        field.AnswerOptions = string.Join(",", fieldDTO.AnswerOption);
                    }

                    Createsurvey.Fields.Add(field);
                }

                _context.survey.Add(Createsurvey);
                await _context.SaveChangesAsync();

                return new ServiceResponse<Survey>
                {
                    Code = 200,
                    Success = true,
                    Data = Createsurvey
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Survey>
                {
                    Code = 400,
                    Success = false,
                    ErrorMessage = "Unexpected Error"
                };               
            }
        }
    }
}
