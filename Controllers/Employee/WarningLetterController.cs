using Microsoft.AspNetCore.Mvc;
using BackendHrdAgro.Models.Layout;
using Microsoft.EntityFrameworkCore;
using BackendHrdAgro.Models.Database.MySql.Support;
using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Employee;
using BackendHrdAgro.Models.Master;
using BackendHrdAgro.Models;

namespace BackendHrdAgro.Controllers.Employee
{
    [Route("api/warningletter")]
    [ApiController]
    public class WarningLetterController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<WarningLetterController> _logger;

        public WarningLetterController(IWebHostEnvironment env, ILogger<WarningLetterController> logger)
        {
            _env = env;
            _logger = logger;
        }

        readonly BaseModel BaseModel = new BaseModel();
        readonly ErrorCodes ErrorCode = new ErrorCodes();
        readonly ErrorMessege ErrorMessege = new ErrorMessege();
        UserDB userDB = new UserDB();
        Dictionary<string, dynamic> Data = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> Responses = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> Detail = new Dictionary<string, dynamic>();
        List<dynamic> ListResponse = new List<dynamic>();
        List<dynamic> ListData = new List<dynamic>();
        DatabaseContext databaseContext = new DatabaseContext();
        TpWarningLetter tpWarningLetter = new TpWarningLetter();
        WarningLetterModel warningLetterModel = new WarningLetterModel();

        [HttpGet("{id}")]
        public IActionResult Get(string id = "USR-201710052")
        {
            try
            {
                var kriteria = "";
                string[] arrayTitle = { "DS002", "DS003" };

                var findSessionData = userDB.FindSessionDataUser(id);
                string employeeId = findSessionData[0].EmployeeId;
                string departmentId = findSessionData[0].DepartmentId;
                string titleId = findSessionData[0].TitleId;
                string divId = findSessionData[0].DivId;
                string levelId = findSessionData[0].LevelId;

                if (departmentId == "DP006" || employeeId == "0808003") // untuk hrd
                {
                    if (titleId == "DS006")
                    {
                        kriteria = $" where g.employee_id='{employeeId}'";
                    }
                    else
                    {
                        kriteria = "";
                    }
                }
                else
                {
                    bool tittleExist = Array.Exists(arrayTitle, element => element == titleId);

                    if (tittleExist == true && levelId == "TL019")
                    {
                        kriteria = $" where d.div_id='{divId}'";
                    }
                    else if (tittleExist == true)
                    {
                        kriteria = $" where d.department_id = '{departmentId}'";
                    }
                    else
                    {
                        kriteria = $" where g.employee_id = '{employeeId}'";
                    }
                }

                var warningLetter = warningLetterModel.WarningLetterGroups(kriteria);
                var department = warningLetterModel.departments();
                var title = warningLetterModel.titles();
                var users = warningLetterModel.users();

                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("warningLetter", warningLetter);
                Detail.Add("department", department);
                Detail.Add("title", title);
                Detail.Add("users", users);

                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("message", "");
                Data.Add("data", ListData);

                return Ok(Data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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

        [HttpPost("{id}/create")]
        public ActionResult Create([FromForm] CreateWarningLetter value, string id = "USR-201710052")
        {
            try
            {
                if (string.IsNullOrEmpty(value.employeeId))
                {
                    return BadRequest(new { msg = "Employee id harus diisi.", success = 0 });
                }
                if (string.IsNullOrEmpty(value.number.ToString()))
                {
                    return BadRequest(new { msg = "Number harus diisi.", success = 0 });
                }
                if (string.IsNullOrEmpty(value.beginDate.ToString()))
                {
                    return BadRequest(new { msg = "Begin Date harus diisi.", success = 0 });
                }
                if (value.File.Length < 1)
                {
                    return BadRequest(new { msg = "File harus diisi.", success = 0 });
                }

                var findSessionData = userDB.FindSessionDataUser(id);
                string userId = findSessionData[0].UserId;

                // Update SP1 menjadi non aktif jika ada
                var sp1 = databaseContext.TpWarnings
                    .Where(w => w.EmployeeId == value.employeeId && w.Number == 1 && w.Status == 1)
                    .FirstOrDefault();

                if (sp1 != null)
                {
                    sp1.Status = 0; // non aktif
                    databaseContext.TpWarnings.Update(sp1);
                }

                // cek apakah karyawan sdh memiliki SP aktif dengan nomor yg sama
                var existingSp = databaseContext.TpWarnings.Where(w => w.EmployeeId == value.employeeId && w.Number == value.number && w.EndDate > DateTime.Now)
                       .FirstOrDefault(); // cek SP aktif berdasarkan endDate

                if (existingSp != null)
                {
                    Responses.Add("code", ErrorCode.BadRequest);
                    Responses.Add("message", "Karyawan masih memiliki SP yang aktif.");
                    ListResponse.Add(Responses);

                    Detail.Add("datas", value);
                    ListData.Add(Detail);

                    Data.Add("response", ListResponse);
                    Data.Add("data", ListData);
                    Data.Add("message", "Data gagal dibuat. SP dengan nomor yang sama sudah ada.");

                    return BadRequest(Data);
                }

                tpWarningLetter.Id = BaseModel
                    .GenerateId(
                    tableName: "tp_warning_letter",
                    primaryKey: "id",
                    str: "WL",
                    trailing: 3,
                    lastKey: "NONE");
                tpWarningLetter.Number = value.number;
                tpWarningLetter.CausedBy = value.causedBy;
                tpWarningLetter.LetterDate = value.letterDate;
                tpWarningLetter.EmployeeId = value.employeeId;
                tpWarningLetter.BeginDate = value.beginDate;
                tpWarningLetter.EndDate = value.beginDate.AddMonths(6);
                tpWarningLetter.Description = value.description;
                tpWarningLetter.CreatedAt = DateTime.Now;
                tpWarningLetter.CreatedBy = userId;
                tpWarningLetter.UpdatedAt = DateTime.Now;
                tpWarningLetter.UpdatedBy = userId;
                tpWarningLetter.DeletedAt = DateTime.Now;
                tpWarningLetter.DeletedBy = userId;
                tpWarningLetter.Status = 1;

                if (value.File != null && value.File.Length > 0)
                {
                    Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.WarningLetter, isProduction: _env.IsProduction()));
                    string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.WarningLetter, isProduction: _env.IsProduction()), value.employeeId);

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

                    tpWarningLetter.FileName = fileName;
                    tpWarningLetter.Type = value.File.ContentType;
                    tpWarningLetter.Size = file.Length;
                }

                // simpan perubahan SP1
                if (sp1 != null)
                {
                    databaseContext.TpWarnings.Update(sp1);
                }

                databaseContext.TpWarnings.Add(tpWarningLetter);
                databaseContext.SaveChanges();

                //Mengubah status menjadi teks untuk ditampilkan
                string statusText = tpWarningLetter.Status == 1 ? "Aktif" : "Non Aktif";

                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("datas", new { value, status = statusText });
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("message", "Data berhasil dibuat.");
                Data.Add("data", ListData);
                return Ok(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Responses.Add("code", ErrorCode.BadRequest);
                Responses.Add("message", ErrorMessege.BadRequest);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", e.Message);

                return BadRequest(Data);
            }

        }

        [HttpPost("{id}/update")]
        public IActionResult UpdateWarning([FromForm] UpdateWarningLetter value)
        {
            // cek apakah karyawan sdh memiliki SP aktif dengan nomor yg sama
            var existingSp = databaseContext.TpWarnings.Where(w => w.EmployeeId == value.employeeId && w.Number == value.number && w.EndDate > DateTime.Now)
                   .FirstOrDefault(); // cek SP aktif berdasarkan endDate

            if (existingSp != null)
            {
                Responses.Add("code", ErrorCode.BadRequest);
                Responses.Add("message", "Karyawan masih memiliki SP yang aktif.");
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", "Data gagal dibuat. SP dengan nomor yang sama sudah ada.");

                return BadRequest(Data);
            }

            if (value.File != null && value.File.Length > 0)
            {
                Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.WarningLetter, isProduction: _env.IsProduction()));
                string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.WarningLetter, isProduction: _env.IsProduction()), value.employeeId);

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

                tpWarningLetter.FileName = fileName;
                tpWarningLetter.Type = value.File.ContentType;
                tpWarningLetter.Size = fileSize;

            }

            List<Dictionary<string, dynamic>> update = warningLetterModel.UpdateWarningLetter(value);

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
        public IActionResult DeleteWarning([FromBody] DeleteReqBody value, string id = "USR-201710052")
        {
            var getWarning = warningLetterModel.warningFindById(value!.id!);
            if (getWarning == null || getWarning.Count() == 0)
            {
                Responses.Add("code", ErrorCode.NotFound);
                Responses.Add("message", ErrorMessege.NotFound);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", "Data tidak ditemukan");
                return NotFound(Data);
            }

            var deleteWarning = warningLetterModel.warningDelete(value, id);
            if (!deleteWarning)
            {
                Responses.Add("code", ErrorCode.BadRequest);
                Responses.Add("message", ErrorMessege.BadRequest);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", "Gagal hapus!");
                return BadRequest(Data);
            }
            Responses.Clear();
            ListResponse.Clear();
            ListData.Clear();
            Detail.Clear();
            Data.Clear();

            Responses.Add("code", ErrorCode.Ok);
            Responses.Add("message", ErrorMessege.Ok);
            ListResponse.Add(Responses);

            Detail.Add("datas", value.id);
            ListData.Add(Detail);

            Data.Add("response", ListResponse);
            Data.Add("data", ListData);
            Data.Add("message", "Berhasil Hapus!");
            return Ok(Data);
        }
    }
}
