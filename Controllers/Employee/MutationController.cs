using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Mvc;
using BackendHrdAgro.Models.Employee;

namespace BackendHrdAgro.Controllers.Employee
{
    [Route("api/mutation")]
    [ApiController]
    public class MutationController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<MutationController> _logger;

        public MutationController(IWebHostEnvironment env, ILogger<MutationController> logger)
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
        TpMutasi tpMutation = new TpMutasi();
        DatabaseContext databaseContext = new DatabaseContext();
        MutationModel mutationModel = new MutationModel();

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            try
            {
                var mutation = mutationModel.mutationGroups();
                Console.WriteLine(mutation);
                var department = mutationModel.departments();
                var title = mutationModel.titles();
                var level = mutationModel.levels();
                var type = mutationModel.types();

                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("mutation", mutation);
                Detail.Add("department", department);
                Detail.Add("title", title);
                Detail.Add("level", level);
                Detail.Add("type", type);

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

        [HttpPost("{id}/signmutation")]
        public ActionResult SignMutation([FromForm] MutationRequestBody value)
        {
            if (string.IsNullOrEmpty(value.EmployeeId))
            {
                return BadRequest(new { msg = "Employee id harus diisi.", success = 0 });
            }
            if (string.IsNullOrEmpty(value.TypeId))
            {
                return BadRequest(new { msg = "Type harus dipilih.", success = 0 });
            }
            if (string.IsNullOrEmpty(value.Deskripsi))
            {
                return BadRequest(new { msg = "Deskripsi harus diisi.", success = 0 });
            }

            tpMutation.MutasiId = BaseModel
                .GenerateId(
                tableName: "tp_mutasi",
                primaryKey: "mutasi_id",
                str: "MP",
                trailing: 3,
                lastKey: "NONE");
            tpMutation.EmployeeId = value.EmployeeId;
            tpMutation.TypeId = value.TypeId;
            tpMutation.DepartmentOldId = value.DepartmentOldId;
            tpMutation.DepartmentId = value.DepartmentId;
            tpMutation.TitleOldId = value.TitleOldId;
            tpMutation.TitleId = value.TitleId;
            tpMutation.LevelOldId = value.LevelOldId;
            tpMutation.LevelId = value.LevelId;
            tpMutation.MutasiDate = value.MutasiDate;
            tpMutation.Deskripsi = value.Deskripsi;
            tpMutation.Status = 0;

            tpMutation.DepartmentId = value.DepartmentId;
            tpMutation.TitleId = value.TitleId;
            tpMutation.LevelId = value.LevelId;

            if (value.File != null && value.File.Length > 0)
            {
                Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.Mutasi, isProduction: _env.IsProduction()));
                string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.Mutasi, isProduction: _env.IsProduction()), value.EmployeeId);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                IFormFile file = value.File;
                string fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(directoryPath, fileName);

                // Save file
                using (var stream = new FileStream(path: filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    //fileName = filePath;
                }

                tpMutation.FileName = fileName;
                tpMutation.Type = value.File.ContentType;
                tpMutation.Size = file.Length;
            } 

            List<Dictionary<string, dynamic>> create = mutationModel.CreateMutation(tpMutation);

            var employeeUpdate = databaseContext.TmEmployeeAffairs.FirstOrDefault(x => x.EmployeeId == value.EmployeeId);
            if (employeeUpdate != null)
            {
                employeeUpdate.DepartmentId = value.DepartmentId;
                employeeUpdate.TitleId = value.TitleId;
                employeeUpdate.LevelId = value.LevelId;

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
        public IActionResult UpdateMutation([FromForm] MutationUpdate value)
        {
            if (string.IsNullOrEmpty(value.EmployeeId))
            {
                return BadRequest(new { msg = "Employee id harus diisi.", success = 0 });
            }
            if (string.IsNullOrEmpty(value.TypeId))
            {
                return BadRequest(new { msg = "Type harus dipilih.", success = 0 });
            }

            var filename = string.Empty;
            long size = 0;
            var fileType = string.Empty;

            if (value.File != null && value.File.Length > 0)
            {
                Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.Mutasi, isProduction: _env.IsProduction()));
                string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.Mutasi, isProduction: _env.IsProduction()), value.EmployeeId);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                IFormFile file = value.File;
                string fileName = Path.GetFileName(file.FileName);
                Console.WriteLine(fileName);
                var filePath = Path.Combine(directoryPath, fileName);

                // Cek file dgn nama yang sama sudah ada, jika ada, hapus
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                using (var stream = new FileStream(path: filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    //fileName = filePath;
                }

                fileName = value.File.FileName;
                fileType = value.File.ContentType;
                size = value.File.Length;
            }

            List<Dictionary<string, dynamic>> update = mutationModel.UpdateMutasi(value);

            var employeeUpdate = databaseContext.TmEmployeeAffairs.FirstOrDefault(x => x.EmployeeId == value.EmployeeId);
            if (employeeUpdate != null)
            {
                employeeUpdate.DepartmentId = value.DepartmentId;
                employeeUpdate.TitleId = value.TitleId;
                employeeUpdate.LevelId = value.LevelId;

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
        public IActionResult DeleteMutation([FromBody] MutationId value)
        {
            List<Dictionary<string, dynamic>> delete = mutationModel.DeleteMutation(value);

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
