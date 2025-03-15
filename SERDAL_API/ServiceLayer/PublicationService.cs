using SERDAL_API.Data;
using SERDAL_API.Application.Services;
using Microsoft.EntityFrameworkCore;
using SERDAL_API.Application.Models.Publication;
using static SERDAL_API.Application.DTOs.PublicationDTO.PublicationDTO;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Azure.Core;
using SERDAL_API.Application.DTOs.PublicationDTO;
using SERDAL_API.Application.Models.User;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SERDAL_API.Helper;

public class PublicationService : ServiceReponse
{
    private readonly DataContext _context;
    public PublicationService(DataContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<List<Publication>>> GetPublicationsAsync(PublicationFilter request)
    {
        var publications = await _context.publication.Where(x => x.Status == 1 && x.isDeleted == 0).ToListAsync();

        if (!publications.Any())
        {
            return new ServiceResponse<List<Publication>>
            {
                Code = 404,
                Success = false,
                ErrorMessage = "No Publication data found"
            };
        }


        //Search by Input text
        if (!string.IsNullOrEmpty(request.search))
        {
            publications = publications
            .Where(p => p.Status == 1 && p.isDeleted == 0 &&
                       (p.Title.Contains(request.search, StringComparison.OrdinalIgnoreCase)
                        || p.Author.Contains(request.search, StringComparison.OrdinalIgnoreCase)
                        || p.Summary.Contains(request.search, StringComparison.OrdinalIgnoreCase)
                        || p.Keywords.Contains(request.search, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        }

        //Filter by Keywords
        if (!string.IsNullOrEmpty(request.keywords))
        {
            publications = publications.Where(x => x.Keywords.Contains(request.keywords)).ToList();
        }

        //Filter by University
        if (!string.IsNullOrEmpty(request.universities))
        {
            publications = publications.Where(x => x.University == Convert.ToInt32(request.universities)).ToList();
        }


        //Set Current Page
        var skip = (request.page - 1) * request.pagesize;
        var publication = publications
            .Skip(skip)
            .Take(request.pagesize)
            .OrderBy(t => t.Title)
            .ToList();


        //Order by
        if (!string.IsNullOrEmpty(request.order))
        {
            if (request.order.Contains("Title_DESC"))
            {
                publication = publications
                .Skip(skip)
                .Take(request.pagesize)
                .OrderByDescending(t => t.Title)
                .ToList();
            }

            if (request.order.Contains("PublicationDate_MostRecent"))
            {
                publication = publications
                .Skip(skip)
                .Take(request.pagesize)
                .OrderByDescending(d => d.PublicationDate).ThenBy(x => x.Title)
                .ToList();
            }

            if (request.order.Contains("PublicationDate_OldestFirst"))
            {
                publication = publications
                .Skip(skip)
                .Take(request.pagesize)
                .OrderBy(d => d.PublicationDate).ThenBy(x => x.Title)
                .ToList();
            }
        }

        if (!publication.Any())
        {
            return new ServiceResponse<List<Publication>>
            {
                Code = 404,
                Success = false,
                ErrorMessage = "No Publication data found"
            };
        }

        return new ServiceResponse<List<Publication>>
        {
            Code = 200,
            Success = true,
            Data = publication
        };
    }

    public List<string> FilterKeywords(List<string> keywords)
    {
        List<string> KeywordList = new List<string>();
        try
        {
            foreach (var keyword in keywords)
            {
                string[] keyList = keyword.Split(',');
                foreach (string word in keyList)
                {
                    if (!KeywordList.Contains(word.Trim()))
                    {
                        KeywordList.Add(word.Trim());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw;
        }

        return KeywordList;
    }

    public async Task<ServiceResponse<List<Publication>>> SearchSuggestions(string searchInput)
    {
        var publications = await _context.publication.Where(x => x.isDeleted == 0 && x.Status == 1).ToListAsync();
        if (!publications.Any())
        {
            return new ServiceResponse<List<Publication>>
            {
                Code = 404,
                Success = false,
                ErrorMessage = "No data found"
            };
        }

        var suggestions = publications
            .Where(p => p.Title.Contains(searchInput, StringComparison.OrdinalIgnoreCase)
                        || p.Author.Contains(searchInput, StringComparison.OrdinalIgnoreCase)
                        || p.Summary.Contains(searchInput, StringComparison.OrdinalIgnoreCase)
                        || p.Keywords.Contains(searchInput, StringComparison.OrdinalIgnoreCase))
            .Take(10)
            .ToList();

        if (!suggestions.Any())
        {
            return new ServiceResponse<List<Publication>>
            {
                Code = 404,
                Success = false,
                ErrorMessage = "No data found"
            };
        }


        return new ServiceResponse<List<Publication>>
        {
            Code = 200,
            Success = true,
            Data = suggestions
        };
    }

    public async Task<ServiceResponse<List<Publication>>> Search(string searchInput)
    {
        var publication = await _context.publication.ToListAsync();

        var filtered = publication
            .Where(p => p.Status == 1 && p.isDeleted == 0 &&
                       (p.Title.Contains(searchInput, StringComparison.OrdinalIgnoreCase)
                        || p.Author.Contains(searchInput, StringComparison.OrdinalIgnoreCase)
                        || p.Summary.Contains(searchInput, StringComparison.OrdinalIgnoreCase)
                        || p.Keywords.Contains(searchInput, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (!filtered.Any())
        {
            return new ServiceResponse<List<Publication>>
            {
                Code = 404,
                Success = false,
                ErrorMessage = "No publication found"
            };
        }

        return new ServiceResponse<List<Publication>>
        {
            Code = 200,
            Success = true,
            Data = filtered
        };
    }


    public async Task<ServiceResponse<List<Publication>>> RelatedArticle(string keywords, int Id)
    {

        List<int> ints = new List<int>();
        var publication = await _context.publication.Where(x => x.Status == 1 && x.isDeleted == 0 && x.University == Id && x.Keywords.ToLower().Trim() != keywords.ToLower().Trim()).ToListAsync();

        string[] keywordList = keywords.Split(',');

        foreach (var p in publication)
        {
            foreach (string key in keywordList)
            {
                if (p.Keywords.Replace(" ", "").ToLower().Contains(key.Replace(" ", "").ToLower()))
                {
                    ints.Add(p.Id);
                }
            }
        }

        var top3Ids = ints.GroupBy(x => x).OrderByDescending(g => g.Count()).Take(3).Select(g => g.Key).ToList();
        var publications = await _context.publication.Where(x => x.Status == 1 && x.isDeleted == 0 && top3Ids.Contains(x.Id)).ToListAsync();


        if (!publications.Any())
        {
            return new ServiceResponse<List<Publication>>
            {
                Code = 404,
                Success = false,
                ErrorMessage = "No data found"
            };
        }

        return new ServiceResponse<List<Publication>>
        {
            Code = 200,
            Success = true,
            Data = publications
        };
    }



    public async Task<ServiceResponse<List<PublicationRequest>>> GetAllRequest(string domain)
    {
        Publication publication = new Publication();
        List<PublicationRequest> requestList = new List<PublicationRequest>();

        var university = await _context.university.ToListAsync();

        var publicationList = await _context.publication.Where(x => x.isDeleted == 0).ToListAsync();
        var userList = await _context.users.ToListAsync();

        if (!publicationList.Any())
        {
            return new ServiceResponse<List<PublicationRequest>>
            {
                Code = 404,
                Success = false,
                ErrorMessage = "No data found"
            };
        }

        for (int i = 0; i < publicationList.Count; i++)
        {
            User users = userList.FirstOrDefault(u => u.ID == publicationList[i].CreatedBy);
            if (users == null)
            {
                continue;
            }

            PublicationRequest request = GetRequest(publicationList[i], users, domain);

            request.University = university.Where(x => x.Id == publicationList[i].University).Select(u => u.label).FirstOrDefault() ?? "";
            requestList.Add(request);
        }

        return new ServiceResponse<List<PublicationRequest>>
        {
            Code = 200,
            Success = true,
            Data = requestList
        };
    }

    private PublicationRequest GetRequest(Publication publicationModel, User users, string domain)
    {
        PublicationRequest request = new PublicationRequest();
        request.Id = publicationModel.Id;
        request.Author = publicationModel.Author;
        request.Title = publicationModel.Title;
        request.Summary = publicationModel.Summary;
        request.Status = publicationModel.Status;
        request.CreatedDate = publicationModel.CreatedDate;
        request.CreatedBy = publicationModel.CreatedBy;
        request.PDFLink = publicationModel.PDFLink;
        request.Keywords = publicationModel.Keywords;
        request.ModifiedDate = publicationModel.ModifiedDate;
        request.ModifiedBy = publicationModel.ModifiedBy;

        if (!string.IsNullOrEmpty(publicationModel.ImgPath))
        {
            request.ImgPath = $"{domain}/{publicationModel.ImgPath}";
        }

        request.PDFFile = $"{domain}/{publicationModel.PDFFile}";

        request.Firstname = users.FirstName;
        request.Lastname = users.LastName;
        request.email = users.Email;
        return request;
    }


    public async Task<ServiceResponse<Publication>> GetPublication(FileUpload model)
    {
        DateTime dateTime = DateTime.Now;

        string PDFfilename = $"Publication/PDF/{model.CreatedBy}/{model.CreatedBy}{dateTime.ToString("MMddyyyyHHmmss")}.pdf";
        string PDFpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", PDFfilename);
        var checkTtile = await _context.publication.FirstOrDefaultAsync(e => e.Title == model.title && e.isDeleted == 0);
        if (checkTtile != null)
        {
            return new ServiceResponse<Publication>
            {
                Code = 404,
                Success = false,
                ErrorMessage = "Title is already exist"
            };

        }


        string Imagepath, Imagefilename;

        if (model.Img == null)
        {
            Imagefilename = "";
            Imagepath = "";
        }
        else
        {
            Imagefilename = $"Publication/Images/{model.CreatedBy.ToString()}/{model.CreatedBy}{dateTime.ToString("MMddyyyyHHmmss")}{Path.GetExtension(model.Img.FileName)}";
            Imagepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Imagefilename);
        }


        Publication publicationResult = new Publication
        {
            Title = model.title,
            Author = model.author,
            Summary = model.summary,
            Keywords = model.keywords ?? "",
            CreatedBy = model.CreatedBy,
            CreatedDate = dateTime,
            ModifiedBy = model.CreatedBy,
            ModifiedDate = dateTime,
            Status = 0,
            ImgPath = Imagefilename,
            PDFFile = PDFfilename,
            PDFLink = model.pdflink ?? "",
            University = model.university ?? 0,
            PublicationDate = model.publicationDate,
        };

        Common.isFolderExist(PDFpath);

        if (model.file != null)
        {
            Common.CreateFile(model.file, PDFpath);
        }

        if (!string.IsNullOrEmpty(Imagepath))
        {
            Common.isFolderExist(Imagepath);
            Common.CreateFile(model.Img, Imagepath);
        }


        return new ServiceResponse<Publication>
        {
            Code = 200,
            Success = true,
            Data = publicationResult
        };

    }

}
