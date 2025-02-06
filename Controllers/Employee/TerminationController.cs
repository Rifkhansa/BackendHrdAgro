using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Employee;
using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using BackendHrdAgro.Models.Master;

namespace BackendHrdAgro.Controllers.Employee
{
    [Route("api/termination")]
    [ApiController]
    public class TerminationController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<TerminationController> _logger;

        public TerminationController(IWebHostEnvironment env, ILogger<TerminationController> logger)
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
        TpTermination tpTermination = new TpTermination();
        TerminationModel terminationModel = new TerminationModel();
        DatabaseContext databaseContext = new DatabaseContext();

        [HttpGet("{id}")]
        public IActionResult Index()
        {
            try
            {
                var termination = terminationModel.terminationGroupQueries();
                Console.WriteLine(termination);
                var reason = terminationModel.reasons();
                Console.WriteLine(reason);

                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("termination", termination);
                Detail.Add("reasonStatus", reason);

                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("message", "");
                Data.Add("data", ListData);

                return Ok(Data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

        [HttpPost("{id}/signout")]
        public ActionResult SignOut([FromForm] TerminationSignOut value)
        {
            if (string.IsNullOrEmpty(value.EmployeeId))
            {
                return BadRequest(new { msg = "Employee id harus diisi.", success = 0 });
            }

            if (string.IsNullOrEmpty(value.Reason))
            {
                return BadRequest(new { msg = "Reason harus diisi.", success = 0 });
            }

            if (value.statusApp == -1)
            {
                return BadRequest(new { msg = "Status Termination harus dipilih.", success = 0 });
            }

            //Inisialisasi model nya
            tpTermination.TerminationId = BaseModel
                .GenerateId(
                tableName: "tp_termination",
                primaryKey: "termination_id",
                str: "AP",
                trailing: 3,
                lastKey: "NONE");
            tpTermination.EmployeeId = value.EmployeeId;
            tpTermination.TerminationDate = value.TerminationDate;
            tpTermination.Reason = value.Reason;
            tpTermination.ReasonId = value.statusApp;
            tpTermination.Status = 0;

            if (value.File != null && value.File.Length > 0)
            {
                Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.Termination, isProduction: _env.IsProduction()));
                string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.Termination, isProduction: _env.IsProduction()), value.EmployeeId);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                IFormFile file = value.File;
                string fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(directoryPath, fileName);

                // Save file
                using (var stream = new FileStream(path: filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    // fileName = filePath;
                }

                tpTermination.FileName = fileName;
                tpTermination.Type = value.File.ContentType;
                tpTermination.Size = file.Length;
            }

            List<Dictionary<string, dynamic>> create = terminationModel.CreateTermination(tpTermination);

            var employeeUpdate = databaseContext.TmEmployeeAffairs.FirstOrDefault(x => x.EmployeeId == value.EmployeeId);
            if (employeeUpdate != null)
            {
                employeeUpdate.EmployeeId = value.EmployeeId;
                employeeUpdate.Status = value.statusApp;

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
        public IActionResult UpdateTermination([FromForm] UpdateTermination value)
        {
            if (string.IsNullOrEmpty(value.EmployeeId))
            {
                return BadRequest(new { msg = "Employee id harus diisi.", success = 0 });
            }

            if (string.IsNullOrEmpty(value.Reason))
            {
                return BadRequest(new { msg = "Reason harus diisi.", success = 0 });
            }

            if (value.statusApp == -1)
            {
                return BadRequest(new { msg = "Status Termination harus dipilih.", success = 0 });
            }

            if (value.File != null && value.File.Length > 0)
            {
                Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.Termination, isProduction: _env.IsProduction()));
                string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.Termination, isProduction: _env.IsProduction()), value.EmployeeId);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                IFormFile file = value.File;
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(directoryPath, fileName);
                var fileSize = file.Length;

                // Cek file dgn nama yang sama sudah ada, jika ada, hapus
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Save file
                using (var stream = new FileStream(path: filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    //fileName = filePath;
                }

                tpTermination.FileName = fileName;
                tpTermination.Type = value.File.ContentType;
                tpTermination.Size = fileSize;

            }

            List<Dictionary<string, dynamic>> update = terminationModel.UpdateTermination(value);

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
        public IActionResult DeleteTermination([FromBody] TerminationIdModel value, string id)
        {
            List<Dictionary<string, dynamic>> delete = terminationModel.DeleteTermination(value, id);

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
