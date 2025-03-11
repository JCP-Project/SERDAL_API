using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SERDAL_API.Data;
using SERDAL_API.ServiceLayer;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using ExcelDataReader;
using SERDAL_API.Application.Models.Datasets;
using SERDAL_API.Application.DTOs.DatasetDTO;
using System.Linq;
using DocumentFormat.OpenXml.Office2010.Excel;


namespace SERDAL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatasetController : ControllerBase
    {

        private readonly DataContext _context;
        //SurveyService _SurveyService;
        public DatasetController(DataContext context)
        {
            _context = context;
            //_SurveyService = new SurveyService(context);
        }

        [HttpPost("UploadExcel")]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            DataSet dataSet = new DataSet(); // Store all sheets
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    memoryStream.Position = 0;

                    using (var reader = ExcelReaderFactory.CreateReader(memoryStream))
                    {
                        var result = reader.AsDataSet();

                        // Iterate over all sheets and add them to the dataset
                        foreach (DataTable table in result.Tables)
                        {
                            dataSet.Tables.Add(table.Copy()); // Copy each sheet into the dataset
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing file: {ex.Message}");
            }

            // Process each sheet in the dataset

                ExtractDataset(dataSet);


            return Helper.Response.Code(200, "File uploaded successfully.");
        }



        private void ExtractDataset(DataSet dataTable)
        {
            var chartDataset = new ChartDataset
            {
                CreatedDate = DateTime.UtcNow,
                CreatedBy = 1,
                IsDeleted = 0,
                dataGroups = new List<DataGroup>()
            };


            chartDataset.Title = dataTable.Tables[0].Rows[0][1].ToString();
            dataTable.Tables[0].Rows.RemoveAt(0);

            foreach (DataTable dataitem in dataTable.Tables)
            {
                DataGroup group = new DataGroup();
                group.Production = dataitem.Rows[0][1].ToString();
                group.Description = dataitem.Rows[1][1].ToString();

                for (int i = 3; i < dataitem.Rows.Count; i++)
                {
                    Commodity commodity = new Commodity
                    {
                        year = dataitem.Rows[i][0].ToString()
                    };

                   group.Commodity.Add(commodity);
                }


                for (int i = 1; i < dataitem.Columns.Count; i++)
                {
                    Variable variable = new Variable();
                    variable.name = dataitem.Rows[2][i].ToString();

                    for (int x = 3; x < dataitem.Rows.Count; x++)
                    {
                        VariableValue variableValue = new VariableValue();
                        variableValue.value = (dataitem.Rows[x][i] == DBNull.Value || dataitem.Rows[x][i] == null) ? (double?)null : Convert.ToDouble(dataitem.Rows[x][i].ToString().Replace(" ",""));

                        variable.variableValue.Add(variableValue);
                    }

                    group.Variable.Add(variable);
                }

                chartDataset.dataGroups.Add(group);
            }


            Task.Run(async () =>
            {
                await _context.chartDatasets.AddAsync(chartDataset);
                await _context.SaveChangesAsync();
            }).GetAwaiter().GetResult();


        }






        [HttpGet]
        [Route("DataById/{Id}")]
        public async Task<IActionResult> DataById(int Id)
        {
            var dataSets = await _context.chartDatasets.Where(x => x.Id == Id).FirstOrDefaultAsync();

            DataSets dataSetsDTO = new DataSets();
            dataSetsDTO.title = dataSets.Title;
            dataSetsDTO.dataGroup = new List<DataGroupDTO>();


            var dataGroups = await _context.dataGroups.Where(x => x.chartDatasetId == Id).ToListAsync();
            foreach (var item in dataGroups)
            {
                DataGroupDTO group = new DataGroupDTO();
                group.production = item.Production;
                group.description = item.Description;
                group.dataYear = new List<string>();
                group.series = new List<Series>();

                var year = await _context.commodity.Where(x => x.dataGroupId == item.Id).Select(y => y.year).ToListAsync();
                group.dataYear = year;

                var variable = await _context.variables.Where(x => x.dataGroupId == item.Id).ToListAsync();

                foreach (var variableItem in variable)
                {
                    Series series = new Series();
                    series.name  = variableItem.name;
                    series.data = await _context.variablesValue.Where(x => x.variableId == variableItem.Id).Select(s => s.value).ToListAsync();
                    group.series.Add(series);
                }


                dataSetsDTO.dataGroup.Add(group);
            }          

            return Ok(dataSetsDTO);
        }




        [HttpGet]
        [Route("GetAllDataSets")]
        public async Task<IActionResult> GetAllDataSets()
        {
            var dataSets = await _context.chartDatasets.ToListAsync();


            List<DataSets> dataSetsDTOList = new List<DataSets>();
            foreach (var dataSet in dataSets)
            {

                DataSets dataSetsDTO = new DataSets();
                dataSetsDTO.id = dataSet.Id;
                dataSetsDTO.title = dataSet.Title;
                dataSetsDTO.dataGroup = new List<DataGroupDTO>();

                var dataGroups = await _context.dataGroups.Where(x => x.chartDatasetId == dataSet.Id).Select(g => new { g.Id, g.Production, g.Description }).ToListAsync();
                foreach (var item in dataGroups)
                {
                    DataGroupDTO group = new DataGroupDTO();
                    group.production = item.Production;
                    group.description = item.Description;
                    group.dataYear = new List<string>();
                    group.series = new List<Series>();

                    var year = await _context.commodity.Where(x => x.dataGroupId == item.Id).Select(y => y.year).ToListAsync();
                    group.dataYear = year;

                    var variable = await _context.variables.Where(x => x.dataGroupId == item.Id).Select(n => n.name).ToListAsync();

                    //foreach (var variableItem in variable)
                    //{
                    //    Series series = new Series();
                    //    series.name = variableItem.name;
                    //    series.data = await _context.variablesValue.Where(x => x.variableId == variableItem.Id).Select(s => s.value).ToListAsync();
                    //    group.series.Add(series);
                    //}

                    dataSetsDTO.dataGroup.Add(group);
                }

                dataSetsDTOList.Add(dataSetsDTO);

            }


            return Ok(dataSetsDTOList);
        }


    }
}
