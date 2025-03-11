using System.IO;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SERDAL_API.Application.DTOs.PublicationDTO;
using SERDAL_API.Application.Models.Publication;
using SERDAL_API.Application.Models.User;
using SERDAL_API.Application.Services;
using SERDAL_API.Data;
using SERDAL_API.Helper;
using static System.Net.WebRequestMethods;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static SERDAL_API.Application.DTOs.PublicationDTO.PublicationDTO;

namespace SERDAL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicationController : ControllerBase
    {
        private readonly DataContext _context;
        PublicationService _publicationService;

        public PublicationController(DataContext context)
        {
            _context = context;
            _publicationService = new PublicationService(_context);
        }

        #region Publication Page
        [HttpPost("PublicationPerPage")]//Publication Main Page
        public async Task<IActionResult> GetPublicationsPerPage([FromBody] PublicationFilter request)
        {
            var publication = await _publicationService.GetPublicationsAsync(request);

            if (!publication.Success)
            {
                return Helper.Response.Code(publication.Code, publication.ErrorMessage);              
            }

            var publications = publication.Data;
            for (int i = 0; i < publications.Count; i++)
            {
                publications[i].ImgPath = $"{Request.Scheme}://{Request.Host}/{publications[i].ImgPath}";

                if (!string.IsNullOrEmpty(publications[i].PDFFile))
                {
                    publications[i].PDFFile = $"{Request.Scheme}://{Request.Host}/{publications[i].PDFFile}";
                }
            }

            return Ok(new
            {
                publications = publication.Data,
                request.page,
                request.pagesize,
                totalCount = await _context.publication.Where(x => x.Status == 1 && x.isDeleted == 0).CountAsync()
            });
        }


        [HttpGet]
        [Route("Dropdown")] //Main Page Dropdown Option
        public async Task<IActionResult> Dropdown()
        {
            var university = await _context.university.Where(x => x.isDeleted == 0).ToListAsync();
            if (!university.Any())
            {
                return Helper.Response.Code(404, "No university found");
            }

            var keywords = await _context.publication.Where(x => x.Status == 1 && x.isDeleted == 0).Select(k => k.Keywords).ToListAsync();
            if (!keywords.Any())
            {
                return Helper.Response.Code(404, "No keywords found");
            }

            List<string> KeywordList = _publicationService.FilterKeywords(keywords);

            return Ok(new { university, KeywordList });
        }


        [HttpGet("searchSuggestions")] //Search Suggestion
        public async Task<IActionResult> GetSearchSuggestions(string query)
        {
            if (string.IsNullOrEmpty(query.Trim()))
            {
               return Ok();
            }

            var suggestions = await _publicationService.SearchSuggestions(query.Trim());
            if (!suggestions.Success)
            {
                return Helper.Response.Code(suggestions.Code, suggestions.ErrorMessage);
            }

            return Ok(suggestions);
        }

        [HttpGet]
        [Route("Search/{Id}")] //Suggestion Clicked
        public async Task<IActionResult> Search(int Id)
        {
            var publication = await _context.publication.Where(x => x.Status == 1 && x.isDeleted == 0 && x.Id == Id).ToListAsync();
            if (publication == null || publication.Count == 0)
            {
                return Helper.Response.Code(404, "No Publication data found");
            }

            for (int i = 0; i < publication.Count; i++)
            {
                publication[i].ImgPath = $"{Request.Scheme}://{Request.Host}/{publication[i].ImgPath}";

                if (!string.IsNullOrEmpty(publication[i].PDFFile))
                {
                    publication[i].PDFFile = $"{Request.Scheme}://{Request.Host}/{publication[i].PDFFile}";
                }
            }

            return Ok(publication);
        }

        [HttpGet("SearchAll")] //Search Button
        public async Task<IActionResult> SearchAll(string query, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(query.Trim()))
            {
                return Ok("");
            }

            var filter = await _publicationService.Search(query.Trim());
            if (!filter.Success)
            {
                return Helper.Response.Code(filter.Code, filter.ErrorMessage);
            }

            // Pagination: Skip items based on the current page and page size
            var pagedPublications = filter.Data
                .Skip((page - 1) * pageSize) // Skip the previous pages' records
                .Take(pageSize)
                .ToList();

            return Ok( new
            {
                publications = pagedPublications,
                totalCount = filter.Data.Count, // Total count for pagination purposes
                page = page,
                pageSize = pageSize,
                totalPages = (int)Math.Ceiling((double)filter.Data.Count / pageSize) // Calculate total number of pages
            });
        }



        [HttpPost("Create")] //Create publication request
        public async Task<IActionResult> CreatePublication([FromForm] FileUpload model)
        {
            try
            {
                var publicationResult = await _publicationService.GetPublication(model);

                if (!publicationResult.Success) 
                {
                    return Helper.Response.Code(publicationResult.Code, publicationResult.ErrorMessage);
                }

                _context.publication.Add(publicationResult.Data);
                await _context.SaveChangesAsync();

                return Ok(publicationResult.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion



        #region Function
        [HttpGet("University")] // Select University
        public async Task<ActionResult> GetUniversity()
        {

            var university = await _context.university.Where(p => p.isDeleted == 0).ToListAsync();
            if (university == null)
            {
                return NotFound(new { code = 404, message = $"No university found" });
            }

            return Ok(university);
        }

        [HttpPost("Download/{Id}")]
        public async Task<IActionResult> Download(int Id)
        {
            var publication = await _context.publication.FindAsync(Id);

            if (publication == null)
            {
                return Helper.Response.Code(404, "No data found");
            }

            publication.Download++;
            _context.publication.Update(publication);
            await _context.SaveChangesAsync();

            return Ok();
        }
        #endregion



        #region Publication User Upload

        [HttpGet("Publication/{id}")] //for Publication Request by User (Publlication request page)
        public async Task<ActionResult> GetPublication(int id)
        {
            var user = await _context.users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { code = 404, message = $"User not found" });
            }

            var publication = await _context.publication.Where(p => p.CreatedBy == id && p.isDeleted == 0).ToListAsync();
            if (!publication.Any())
            {
                return NotFound(new { code = 404, message = $"No data found" });
            }

            for (int i = 0; i < publication.Count; i++)
            {
                if (!string.IsNullOrEmpty(publication[i].ImgPath))
                {
                    publication[i].ImgPath = $"{Request.Scheme}://{Request.Host}/{publication[i].ImgPath}";
                }

            }

            return Ok(publication.OrderByDescending(d => d.CreatedDate).OrderBy(t => t.Title));
        }



        #endregion



        #region Page Info

        [HttpGet("Info/{id}")] // for publication Info Page
        public async Task<IActionResult> GetPublicationInfo(int id)
        {
            var publication = await _context.publication.FindAsync(id);
            if (publication == null)
            {
                return NotFound($"User not found");
            }


            if (!string.IsNullOrEmpty(publication.ImgPath))
            {
                publication.ImgPath = $"{Request.Scheme}://{Request.Host}/{publication.ImgPath}";
            }

            publication.PDFFile = $"{Request.Scheme}://{Request.Host}/{publication.PDFFile}";

            var university = await _context.university.Where(p => p.isDeleted == 0).ToListAsync();
            if (university == null)
            {
                return NotFound(new { code = 404, message = $"University not found" });
            }

            return Ok(new { publication, university });
        }


        [HttpGet("RelatedArticle")]
        public async Task<IActionResult> RelatedArticle(string keywords, int Id)
        {
            var publications = await _publicationService.RelatedArticle(keywords, Id);
            if (!publications.Success)
            {
                return Helper.Response.Code(publications.Code, publications.ErrorMessage);
            }

            for (int i = 0; i < publications.Data.Count; i++)
            {
                if (!string.IsNullOrEmpty(publications.Data[i].ImgPath))
                {
                    publications.Data[i].ImgPath = $"{Request.Scheme}://{Request.Host}/{publications.Data[i].ImgPath}";
                }

                if (!string.IsNullOrEmpty(publications.Data[i].PDFFile))
                {
                    publications.Data[i].PDFFile = $"{Request.Scheme}://{Request.Host}/{publications.Data[i].PDFFile}";
                }
            }

            return Ok(publications.Data);
        }

        #endregion



        #region Admin

        [HttpGet] //Get all publication request
        public async Task<IActionResult> GetPublicationRequest()
        {
            string domain = $"{Request.Scheme}://{Request.Host}";

            var requestList = await _publicationService.GetAllRequest(domain);

            if (!requestList.Success)
            {
                return Helper.Response.Code(requestList.Code, requestList.ErrorMessage);
            }

            return Ok(requestList.Data);
        }


        [HttpPost("UpdateStatus")] //Approve, Delete
        public async Task<ActionResult> UpdateStatus(Status status)
        {
            DateTime dateTime = DateTime.Now;

            var publication = await _context.publication.FindAsync(status.Id);
            if (publication == null)
            {
                return NotFound($"Publication not found");
            }

            publication.Status = status.status;
            publication.ModifiedBy = status.ModifiedBy;
            publication.ModifiedDate = dateTime;
            publication.isDeleted = status.isDeleted;

            _context.publication.Update(publication);
            await _context.SaveChangesAsync();

            return Ok("status updated");
        }



        #endregion


























        //[HttpGet("AllPublication")]
        //public async Task<IActionResult> AllPublication()
        //{

        //        var publication = await _context.publication.Where(x => x.Status == 1 && x.isDeleted == 0).ToListAsync();

        //        if (publication == null || publication.Count == 0)
        //        {
        //            return NotFound(new { code = 404, message = $"No Publication data found" });
        //        }

        //        for (int i = 0; i < publication.Count; i++)
        //        {
        //            publication[i].ImgPath = $"{Request.Scheme}://{Request.Host}/{publication[i].ImgPath}";

        //            if (!string.IsNullOrEmpty(publication[i].PDFFile))
        //            {
        //                publication[i].PDFFile = $"{Request.Scheme}://{Request.Host}/{publication[i].PDFFile}";
        //            }
        //        }

        //        return Ok(publication.OrderByDescending(d => d.CreatedDate).OrderBy(t => t.Title));          
        //}
















    }
}
