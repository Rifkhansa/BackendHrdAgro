using BackendHrdAgro.Models.Attendance;
using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Master;
using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Mvc;
using BackendHrdAgro.Models.Employee;

namespace BackendHrdAgro.Controllers.Attendance
{
    [Route("api/holiday")]
    [ApiController]
    public class HolidayController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<HolidayController> _logger;
        public HolidayController(IWebHostEnvironment env, ILogger<HolidayController> logger)
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
        UserDB userDB = new UserDB();
        TmHolidayDate tmHolidayDate = new TmHolidayDate();
        DatabaseContext databaseContext = new DatabaseContext();
        HolidayModel holidayModel = new HolidayModel();

        [HttpGet("{id}")]
        public IActionResult Index(string id = "USR-201710052")
        {
            var kriteria = "";
            string[] arrayTitle = { "DS002", "DS003" };

            var findSessionData = userDB.FindSessionDataUser(id);
            string employeeId = findSessionData[0].EmployeeId;
            string departmentId = findSessionData[0].DepartmentId;
            string titleId = findSessionData[0].TitleId;
            string userId = findSessionData[0].UserId;

            try
            {
                if (departmentId == "DP006") // untuk hrd
                {
                    if (titleId == "DS006") // untuk manager
                    {
                        kriteria = $"and a.employee_id='{employeeId}'";
                    }
                    else
                    {
                        kriteria = "";
                    }
                }
                else
                {
                    bool tittleExist = Array.Exists(arrayTitle, element => element == titleId);

                    if (tittleExist)
                    {
                        kriteria = $"and b.department_id={departmentId}";
                    }
                    else
                    {
                        kriteria = $"and a.employee_id={employeeId}";
                    }
                }

                var holiday = holidayModel.holidayGroups();

                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("holiday", holiday);

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

        [HttpPost("{id}/create")]
        public ActionResult Create([FromBody] CreateHoliday value, string id = "USR-201710052")
        {
            var findSessionData = userDB.FindSessionDataUser(id);
            string userId = findSessionData[0].UserId;

            tmHolidayDate.HolidayId = BaseModel
                .GenerateId(
                tableName: "tm_holiday_date",
                primaryKey: "holiday_id",
                str: "DAY" + DateTime.Now.ToString("yyMM"),
                trailing: 3,
                lastKey: "NONE");
            tmHolidayDate.HolidayName = value.HolidayName;
            tmHolidayDate.HolidayDate = value.HolidayDate;
            tmHolidayDate.Status = 1;

            tmHolidayDate.DtEtr = DateTime.Now;
            tmHolidayDate.UserEtr = userId;
            tmHolidayDate.DtUpdate = DateTime.Now;
            tmHolidayDate.UserUpdate = userId;

            List<Dictionary<string, dynamic>> create = holidayModel.CreateHolidays(tmHolidayDate);

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

        [HttpPost("{id}/update")]
        public IActionResult Update([FromBody] UpdateHoliday value)
        {
            List<Dictionary<string, dynamic>> update = holidayModel.UpdateHolidays(value);

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
        public IActionResult Delete([FromBody] DeleteHoliday value)
        {
            List<Dictionary<string, dynamic>> delete = holidayModel.DeleteHolidays(value);

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
