using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Master;
using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendHrdAgro.Controllers.Master
{
    [Route("api/password")]
    [ApiController]
    public class ChangePassword : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ChangePassword> _logger;
        public ChangePassword(IWebHostEnvironment env, ILogger<ChangePassword> logger)
        {
            _env = env;
            _logger = logger;
        }

        readonly BaseModel BaseModel = new BaseModel();
        UserDB userDB = new UserDB();
        readonly ErrorCodes ErrorCode = new ErrorCodes();
        readonly ErrorMessege ErrorMessege = new ErrorMessege();
        Dictionary<string, dynamic> Data = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> Responses = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> Detail = new Dictionary<string, dynamic>();
        List<dynamic> ListResponse = new List<dynamic>();
        List<dynamic> ListData = new List<dynamic>();
        DatabaseContext databaseContext = new DatabaseContext();
        UsersDB usersDB = new UsersDB();
        TpHistoryChangePassword tpHistoryChangePassword = new TpHistoryChangePassword();

        [HttpPost("{id}")]
        public IActionResult Index([FromBody] IndexChangePassword value)
        {
            try
            {
                var myFullName = string.Empty;
                var myDepartment = string.Empty;
                var myresult = usersDB.employeeDeparts(value);
                foreach (var item in myresult)
                {
                    myFullName = item.EmployeeFirstName + "" + item.EmployeeLastName;
                    myDepartment = item.DepartmentName;
                }

                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
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

        [HttpPost("{id}/changepassword")]
        public ActionResult ChangePass([FromBody] ChangePass value, string id = "USR-201710052")
        {
            try
            {
                var findSessionData = userDB.FindSessionDataUser(id);
                string userId = findSessionData[0].UserId;

                var myCurrentPassword = string.Empty;
                var qpassword = usersDB.password(value);
                Console.WriteLine(qpassword.Count);
                foreach (var user in qpassword)
                {
                    myCurrentPassword = user.Password;
                }

                var md5 = BaseModel.HashedPassword(value.CurrentPassword); //md5
                Console.WriteLine("Password = " + md5);
                Console.WriteLine("myCurrentPass = " + myCurrentPassword);
                if (myCurrentPassword != md5)
                {
                    return BadRequest(new { msg = "Old Password tidak sama dengan database.", success = 0 });
                }

                var totalChangePassword = 0;
                var query = usersDB.TotalChange(value);
                Console.WriteLine(query);
                foreach (var user in query)
                {
                    totalChangePassword = user.JumlahChangePassword;
                }

                if (totalChangePassword < 3)
                {
                    tpHistoryChangePassword.EmployeeId = value.EmployeeId;
                    tpHistoryChangePassword.Password = md5;
                    tpHistoryChangePassword.DateChangePassword = DateTime.Now;

                    databaseContext.TpHistoryChangePasswords.Add(tpHistoryChangePassword);
                    databaseContext.SaveChanges();
                }
                else
                {
                    var idTerlama = 0;
                    var historyId = usersDB.historyIds(value);
                    foreach (var user in historyId)
                    {
                        idTerlama = user.HistoryId;
                    }

                    // update idTerlama
                    var updateId = databaseContext.TpHistoryChangePasswords.FirstOrDefault(x => x.IdHistoryPassword == idTerlama);
                    Console.WriteLine("updateID = " + updateId);
                    if (updateId != null)
                    {
                        updateId.EmployeeId = value.EmployeeId;
                        updateId.Password = md5;
                        updateId.DateChangePassword = DateTime.Now;

                        databaseContext.SaveChanges();
                    }
                }

                TmUser tmUser = new TmUser();
                var newPassword = BaseModel.HashedPassword(value.NewPassword); //md5
                Console.WriteLine(newPassword);

                // update 
                var update = databaseContext.TmUsers.FirstOrDefault(x => x.EmployeeId == value.EmployeeId);
                if (update != null)
                {
                    update.Password = newPassword;
                    update.LastChangePassword = DateTime.Now;
                    update.DtEtr = DateTime.Now;
                    update.UserEtr = userId;
                    update.DtUpdate = DateTime.Now;
                    update.UserUpdate = userId;

                    databaseContext.SaveChanges();
                }

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
            catch (Exception ex)
            {
                Responses.Add("code", ErrorCode.BadRequest);
                Responses.Add("message", ErrorMessege.BadRequest);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", ex.Message);
                return BadRequest(Data);
            }
        }

    }
}
