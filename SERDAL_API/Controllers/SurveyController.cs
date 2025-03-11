using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SERDAL_API.Application.DTOs.PublicationDTO;
using SERDAL_API.Application.DTOs.Survey;
using SERDAL_API.Application.Models.Survey;
using SERDAL_API.Data;
using SERDAL_API.ServiceLayer;
using static SERDAL_API.Application.DTOs.PublicationDTO.PublicationDTO;

namespace SERDAL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyController : Controller
    {
        private readonly DataContext _context;
        SurveyService _SurveyService;
        public SurveyController(DataContext context)
        {
            _context = context;
            _SurveyService = new SurveyService(context);
        }

        [HttpPost("Create")]//Publication Main Page
        public async Task<IActionResult> CreateSurvey([FromBody] SurveyCreate survey)
        {
            DateTime dt = DateTime.Now;
            var surveyResult = await _SurveyService.CreateSurvey(survey);

            if (!surveyResult.Success)
            {
               return Helper.Response.Code(surveyResult.Code, surveyResult.ErrorMessage);
            }

            return Ok();
        }


        [HttpGet("AllSurvey")]//Publication Main Page
        public async Task<IActionResult> AllSurvey()
        {

            var surveyList = await _context.survey.OrderByDescending(x=> x.isActive).ToListAsync();

            if (!surveyList.Any())
            {
                return BadRequest();
            }

            return Ok(surveyList);
        }


        [HttpGet("GetAnswerSheet/{Id}")]
        public async Task<IActionResult> GetAnswerSheet(int Id)
        {

            var survey = await _context.survey.OrderByDescending(x => x.isActive == 1 && x.Id == Id).FirstOrDefaultAsync();
            if (survey == null)
            {
                return BadRequest();
            }


            var fields = await _context.fields.Where(x => x.SurveyId == survey.Id).ToListAsync();
            if (!fields.Any())
            {
                return BadRequest();
            }

            ReturnAnswerSheetDTO ans = new ReturnAnswerSheetDTO();
            ans.Id = survey.Id;
            ans.Title = survey.Title;
            ans.Description = survey.Description;


            foreach (var SurveyField in fields)
            {
                ReturnFieldDTO field = new ReturnFieldDTO();

                field.Id = SurveyField.Id;
                field.Index = SurveyField.Index;
                field.SurveyId = SurveyField.SurveyId;
                field.Question = SurveyField.Question;
                field.Required = SurveyField.Required;
                field.SelectedType = SurveyField.SelectedType;

                if (SurveyField.AnswerOptions != null)
                {
                    field.AnswerOptions = SurveyField.AnswerOptions?.Split(',').Select(x => x.Trim()).ToArray();
                }
                

                ans.Fields.Add(field);
            }
                      
          //  survey.Fields = fields;

            return Ok(ans);
        }

    }
}
