using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SERDAL_API.BusinessLayer;

namespace SERDAL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {


        //[HttpPost]
        //public IActionResult UploadPDF(IFormFile file)
        //{
        //    return Ok(new fileUploadHelper().UploadPDF(file));
        //}

        [HttpPost]
        public async Task<ActionResult> UploadFileWithDetails([FromForm] FileUpload model)
        {
            if (model.file == null || model.file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Now you have access to Title, Author, Description, PdfLink along with the File
            var result = new fileUploadHelper().UploadPDFWithDetails(model.file, model.title, model.author, model.summary, model.pdflink);
            return Ok(result);
        }
    }
}
