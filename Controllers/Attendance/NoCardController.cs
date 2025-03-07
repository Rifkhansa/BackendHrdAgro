using BackendHrdAgro.Models.Attendance;
using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Master;
using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Mvc;
using BackendHrdAgro.Models.Database.MySql.Support;

namespace BackendHrdAgro.Controllers.Attendance
{
    [Route("api/absentee")]
    [ApiController]
    public class NoCardController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<NoCardController> _logger;

        public NoCardController(IWebHostEnvironment env, ILogger<NoCardController> logger)
        {
            _env = env;
            _logger = logger;
        }

        readonly BaseModel BaseModel = new BaseModel();
        ErrorCodes errorCodes = new ErrorCodes();
        ErrorMessege errorMessege = new ErrorMessege();
        Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> detail = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> resp = new Dictionary<string, dynamic>();
        List<dynamic> listResp = new List<dynamic>();
        List<dynamic> listData = new List<dynamic>();
        PeriodeDB periodeDB = new PeriodeDB();
        UserDB userDB = new UserDB();
        List<dynamic> ListResponse = new List<dynamic>();
        List<dynamic> ListData = new List<dynamic>();
        DatabaseContext databaseContext = new DatabaseContext();
        NoCardModel noCardModel = new NoCardModel();

        [HttpPost("{id}/nocard")]
        public IActionResult nocard(string id = "USR-201710052")
        {

            var clausa = "";

            string[] arrTittleId = { "DS002", "DS003" };
            try
            {
                var findSessionData = userDB.FindSessionDataUser(id);
                string employeeId = findSessionData[0].EmployeeId;
                string departmentId = findSessionData[0].DepartmentId;
                string userId = findSessionData[0].UserId;
                string titleId = findSessionData[0].TitleId;
                string divId = findSessionData[0].DivId;
                string levelId = findSessionData[0].LevelId;

                if (departmentId == "DP006" || employeeId == "0808003" || employeeId == "0000000")
                { //untuk hrd dan admin
                    if (titleId == "DS006")
                    {
                        clausa = $" and a.employee_id='{employeeId}'";
                    }
                    else
                    {
                        clausa = " ";
                    }

                }
                else
                {
                    bool tittleExist = Array.Exists(arrTittleId, element => element == titleId);

                    if (tittleExist == true && levelId == "TL019")
                    {
                        clausa = $" and b.div_id='{divId}'";
                    }
                    else if (tittleExist == true)
                    {
                        clausa = $" and b.department_id='{departmentId}'";
                    }
                    else
                    {
                        clausa = $" and a.employee_id='{employeeId}'";
                    }
                }
                if (employeeId == "0711011") //stefi
                {
                    clausa = $" and b.department_id='{departmentId}'";
                }

                List<NoCardQuery> nocard = noCardModel.ListNoCard(clausa);

                var LinkDel = "";
                var revisi = "";

                if (nocard == null)
                {
                    nocard = new List<NoCardQuery>();
                }

                foreach (var k in nocard)
                {
                    revisi = $"{k.EmployeeId}|{k.AbsentDate}";

                    //Link Modal Edit
                    if (k.Dimunculin == 0) // cek untuk menampilkan delete (tidak berlaku untuk manager dan asmen
                    {
                        LinkDel = "";
                    }
                    else
                    {
                        if (k.Dimunculin == 1 && k.EmployeeId == employeeId)
                        {
                            LinkDel = $"<a href=\"#\" class=\"text-danger delete ms-2\" onclick=\"destroy('{revisi}')\"><i class=\"ti ti-trash fs-5\" title=\"Delete\"></i></a>";
                        }
                        else
                        {
                            LinkDel = "";
                        }
                    }

                    k.LinkDel = LinkDel;

                }

                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                detail.Add("NoCard", nocard);
                listData.Add(detail);

                data.Add("response", listResp);
                data.Add("message", "");
                data.Add("data", listData);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: HttpContext.Request.Path, exception: ex);
                resp.Add("code", errorCodes.BadRequest);
                resp.Add("message", errorMessege.BadRequest);
                listResp.Add(resp);

                data.Add("response", listResp);
                data.Add("data", null);
                data.Add("message", ex.Message);
                return BadRequest(data);
            }
        }

        [HttpPost("{id}/createNoCard")]
        public IActionResult CreateNoCard([FromForm] CreateNoCardModel value, string id = "USR-201710052")
        {
            if (string.IsNullOrEmpty(value.EmployeeId))
            {
                return BadRequest(new { message = "Employee id harus diisi.", success = 0 });
            }

            DateTime requestDate = DateTime.Now;
            string dateString = "";
            dateString = requestDate.ToString("yyyyMM");

            bool checkPeriodeClosed = false;
            checkPeriodeClosed = periodeDB.IsPeriodeAbsentClosed(dateString);
            if (checkPeriodeClosed == true)
            {
                return BadRequest(new { message = "Periode Absent sudah ditutup.", success = 0 });
            }

            TpAbsentNoCard createNoCard = new TpAbsentNoCard()
            {
                EmployeeId = value.EmployeeId,
                AbsentDate = DateTime.Now,
                DtEtr = DateTime.Now
            };


            if (value.FileName != null && value.FileName.Length > 0)
            {
                Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.NoCard, isProduction: _env.IsProduction()));
                string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.NoCard, isProduction: _env.IsProduction()), value.EmployeeId);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                IFormFile file = value.FileName;
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(directoryPath, fileName);

                var fileSize = file.Length;

                // Save file
                using (var stream = new FileStream(path: filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }


                // Generate a new file name based on the current date and time
                DateTime currentDate = DateTime.Now;
                string fileNameNew = value.EmployeeId + "-" + currentDate.ToString("yyyyMMddHHmmss") + Path.GetExtension(fileName);
                var filePathNew = Path.Combine(directoryPath, fileNameNew);

                // Check if a file with the new name already exists, if so, delete it
                if (System.IO.File.Exists(filePathNew))
                {
                    System.IO.File.Delete(filePathNew);
                }

                // Rename the file by moving it to the new file path
                System.IO.File.Move(filePath, filePathNew);

                createNoCard.FileName = fileNameNew;

            }

            List<Dictionary<string, dynamic>> create = noCardModel.CreateNoCard(createNoCard);

            bool myResult = create[0]["result"];
            var myMessage = create[0]["message"];

            if (myResult)
            {

                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                detail.Add("datas", value);
                listData.Add(detail);

                data.Add("response", listResp);
                data.Add("data", listData);
                data.Add("message", myMessage);
                return Ok(data);
            }
            else
            {
                resp.Add("code", errorCodes.BadRequest);
                resp.Add("message", errorMessege.BadRequest);
                listResp.Add(resp);

                detail.Add("datas", value);
                listData.Add(detail);

                data.Add("response", listResp);
                data.Add("data", listData);
                data.Add("message", myMessage);
                return BadRequest(data);

            }



        }

        [HttpPost("{id}/createNoCardWhatsapp")]
        public IActionResult CreateNoCardWhatsapp([FromForm] CreateNoCardWhatsappModel value, string id = "USR-201710052")
        {
            if (string.IsNullOrEmpty(value.Phone))
            {
                resp.Add("success", 0);
                resp.Add("message", "Harus melalui no HP Whatsapp");
                listResp.Add(resp);
                data.Add("response", listResp);
                return Ok(data);
            }

            DateTime requestDate = DateTime.Now;
            string dateString = "";
            dateString = requestDate.ToString("yyyyMM");

            using DatabaseContext context = new DatabaseContext(); //Call DatabaseContext to connect with database

            List<ListWAUser> myUser = noCardModel.listUser(value.Phone);
            var employeeId = "";
            var fullName = "";

            foreach (var k in myUser)
            {
                employeeId = k.EmployeeId;
                fullName = k.UserFullName;

            }

            if (employeeId.Length == 0) //check if password from request body match with the password from stored tm_user data in var user

            {
                resp.Add("success", 0);
                resp.Add("message", "Nomor HP tidak terdaftar, silahkan hubungi IT (Administrator)");
                listResp.Add(resp);
                data.Add("response", listResp);
                return Ok(data);

            }

            bool checkPeriodeClosed = false;
            checkPeriodeClosed = periodeDB.IsPeriodeAbsentClosed(dateString);
            if (checkPeriodeClosed == true)
            {
                resp.Add("success", 0);
                resp.Add("message", "Periode Absent sudah ditutup");
                listResp.Add(resp);
                data.Add("response", listResp);
                return Ok(data);
            }

            TpAbsentNoCard createNoCard = new TpAbsentNoCard()
            {
                EmployeeId = employeeId,
                AbsentDate = DateTime.Now,
                DtEtr = DateTime.Now
            };


            if (value.FileName != null && value.FileName.Length > 0)
            {
                Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.NoCard, isProduction: _env.IsProduction()));
                string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.NoCard, isProduction: _env.IsProduction()), employeeId);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                IFormFile file = value.FileName;
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(directoryPath, fileName);

                var fileSize = file.Length;

                // Save file
                using (var stream = new FileStream(path: filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Generate a new file name based on the current date and time
                DateTime currentDate = DateTime.Now;
                string fileNameNew = employeeId + "-" + currentDate.ToString("yyyyMMddHHmmss") + Path.GetExtension(fileName);
                var filePathNew = Path.Combine(directoryPath, fileNameNew);

                // Check if a file with the new name already exists, if so, delete it
                if (System.IO.File.Exists(filePathNew))
                {
                    System.IO.File.Delete(filePathNew);
                }

                // Rename the file by moving it to the new file path
                System.IO.File.Move(filePath, filePathNew);

                createNoCard.FileName = fileNameNew;

            }

            List<Dictionary<string, dynamic>> create = noCardModel.CreateNoCard(createNoCard);

            bool myResult = create[0]["result"];
            var myMessage = create[0]["message"];

            if (myResult)
            {
                resp.Add("success", 1);
                resp.Add("message", errorMessege.Ok);
                resp.Add("employeeName", fullName);
                listResp.Add(resp);

                data.Add("response", listResp);
                return Ok(data);
            }
            else
            {
                resp.Add("success", 0);
                resp.Add("message", "Error By System");
                listResp.Add(resp);

                data.Add("response", listResp);
                return BadRequest(data);

            }


        }


    }
}
