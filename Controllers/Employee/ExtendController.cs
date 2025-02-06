using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Employee;
using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendHrdAgro.Controllers.Employee
{
    [Route("api/extend")]
    [ApiController]
    public class ExtendController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ExtendController> _logger;
        public ExtendController(IWebHostEnvironment env, ILogger<ExtendController> logger)
        {
            _env = env;
            _logger = logger;
        }

        readonly BaseModel BaseModel = new BaseModel();
        readonly ErrorCodes ErrorCode = new ErrorCodes();
        readonly ErrorMessege ErrorMessege = new ErrorMessege();
        Dictionary<string, dynamic> Data = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> Responses = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> Detail = new Dictionary<string, dynamic>();
        List<dynamic> ListResponse = new List<dynamic>();
        List<dynamic> ListData = new List<dynamic>();
        TpExtend tpExtend = new TpExtend();
        DatabaseContext databaseContext = new DatabaseContext();
        ExtendModel extendModel = new ExtendModel();

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            try
            {
                var extend = extendModel.extendGroupQueries();
                var department = extendModel.departments();
                var type = extendModel.types();
                var title = extendModel.titles();
                var level = extendModel.levels();
                var reasonStatus = extendModel.reasonStatuses();

                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("extend", extend);
                Detail.Add("department", department);
                Detail.Add("type", type);
                Detail.Add("title", title);
                Detail.Add("level",  level);
                Detail.Add("reasonStatus", reasonStatus);

                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("message", "");
                Data.Add("data", ListData);

                return Ok(Data);
            }
            catch (Exception ex)
            {
                Responses.Add("code", ErrorCode.BadRequest);
                Responses.Add("message", ErrorMessege.BadRequest);
                ListResponse.Add(Responses);

                Detail.Add("datas", null);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", ex.Message);
                return BadRequest(Data);
            }
        }

        [HttpPost("{id}/signextend")]
        public ActionResult SignExtend([FromForm] ExtendSignRequestBody value)
        {
            if (string.IsNullOrEmpty(value.EmployeeId))
            {
                return BadRequest(new { msg = "Employee id harus diisi.", success = 0 });
            }
            if (string.IsNullOrEmpty(value.Reason))
            {
                return BadRequest(new { msg = "Reason harus diisi.", success = 0 });
            }

            tpExtend.ExtendId = BaseModel
                .GenerateId(
                tableName: "tp_extend",
                primaryKey: "extend_id",
                str: "ED",
                trailing: 3,
                lastKey: "NONE");
            tpExtend.EmployeeId = value.EmployeeId;
            tpExtend.StartExtendDate = value.StartExtendDate;
            tpExtend.EndExtendDate = value.EndExtendDate;
            tpExtend.Reason = value.Reason;
            tpExtend.Status = 0;

            if (value.File != null && value.File.Length > 0)
            {
                Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.Extend, isProduction: _env.IsProduction()));
                string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.Extend, isProduction: _env.IsProduction()), value.EmployeeId);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                IFormFile file = value.File;
                string fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(directoryPath, fileName);

                // Cek file dgn nama yang sama sudah ada, jika ada, hapus
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                //save file
                using (var stream = new FileStream(path: filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    //fileName = filePath;
                }

                tpExtend.FileName = fileName;
                tpExtend.Type = value.File.ContentType;
                tpExtend.Size = file.Length;
            }

            List<Dictionary<string, dynamic>> create = extendModel.CreateExtend(tpExtend);

            var employeeUpdate = databaseContext.TmEmployeeAffairs.FirstOrDefault(x => x.EmployeeId == value.EmployeeId);
            if (employeeUpdate != null)
            {
                employeeUpdate.EndOfContract = value.EndExtendDate;

                databaseContext.SaveChanges();
            }

            bool myResult = create[0]["result"];
            var myMessage = create[0]["message"];

            if (myResult)
            {
                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("message", "Data berhasil disimpan.");
                Data.Add("data", ListData);
                return Ok(Data);
            }
            else
            {
                Responses.Add("code", ErrorCode.BadRequest);
                Responses.Add("message", ErrorMessege.BadRequest);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", myMessage);
                return BadRequest(Data);

            }
        }

        [HttpPost("{id}/edit")]
        public IActionResult UpdateExtend([FromForm] ExtendUpdate value)
        {
            if (string.IsNullOrEmpty(value.EmployeeId))
            {
                return BadRequest(new { msg = "Employee id harus diisi.", success = 0 });
            }
            if (string.IsNullOrEmpty(value.Reason))
            {
                return BadRequest(new { msg = "Reason harus diisi.", success = 0 });
            }


            if (value.File != null && value.File.Length > 0)
            {
                Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.Extend, isProduction: _env.IsProduction()));
                string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.Extend, isProduction: _env.IsProduction()), value.EmployeeId);


                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                IFormFile file = value.File;
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(directoryPath, fileName);

                using (var stream = new FileStream(path: filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    //fileName = filePath;
                }

                tpExtend.FileName = fileName;
                tpExtend.Type = value.File.ContentType;
                tpExtend.Size = file.Length;
            }

            List<Dictionary<string, dynamic>> update = extendModel.UpdateExtend(value);

            var employeeUpdate = databaseContext.TmEmployeeAffairs.FirstOrDefault(x => x.EmployeeId == value.EmployeeId);
            if (employeeUpdate != null)
            {
                employeeUpdate.EndOfContract = value.EndExtendDate;

                databaseContext.SaveChanges();
            }

            bool myResult = update[0]["result"];
            var myMessage = update[0]["message"];

            if (myResult)
            {

                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("message", "Data berhasil diubah.");
                Data.Add("data", ListData);
                return Ok(Data);
            }
            else
            {
                Responses.Add("code", ErrorCode.BadRequest);
                Responses.Add("message", ErrorMessege.BadRequest);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", myMessage);
                return BadRequest(Data);

            }
        }

        [HttpPost("{id}/delete")]
        public IActionResult DeleteExtend([FromBody] ExtendIdModel value)
        {
            List<Dictionary<string, dynamic>> delete = extendModel.DeleteExtend(value);

            bool myResult = delete[0]["result"];
            var myMessage = delete[0]["message"];

            if (myResult)
            {
                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("message", "Data berhasil dihapus.");
                Data.Add("data", ListData);
                return Ok(Data);
            }
            else
            {
                Responses.Add("code", ErrorCode.BadRequest);
                Responses.Add("message", ErrorMessege.BadRequest);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", myMessage);
                return BadRequest(Data);
            }
        }


    }
}
