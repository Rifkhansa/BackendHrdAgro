using BackendHrdAgro.Models.Attendance;
using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendHrdAgro.Controllers.Attendance
{
    public class BirthdayController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<BirthdayController> _logger;
        public BirthdayController(IWebHostEnvironment env, ILogger<BirthdayController> logger)
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
        DatabaseContext dbContext = new DatabaseContext();
        BirthdayModel birthdayModel = new BirthdayModel();
        TpExtend tpExtend = new TpExtend();

        [HttpGet("{id}")]
        public IActionResult Get()
        {
            try
            {
                var reminders = birthdayModel.reminders();

                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("reminders", reminders);

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
        public ActionResult SignExtendBirthday([FromBody] BirthdayReqBody value)
        {
            tpExtend.ExtendId = BaseModel
               .GenerateId(
               tableName: "tp_extend",
               primaryKey: "extend_id",
               str: "ED",
               trailing: 3,
               lastKey: "NONE");
            tpExtend.EmployeeId = value.EmployeeId;
            tpExtend.StartExtendDate = value.startExtendDate;
            tpExtend.EndExtendDate = value.endExtendDate;
            tpExtend.Reason = value.Reason;
            tpExtend.Status = 0;

            // create
            List<Dictionary<string, dynamic>> create = birthdayModel.CreateExtendBirthday(tpExtend);

            // update
            var employeeUpdate = dbContext.TmEmployeeAffairs.FirstOrDefault(x => x.EmployeeId == value.EmployeeId);
            if (employeeUpdate != null)
            {
                employeeUpdate.EndOfContract = value.endExtendDate;

                dbContext.SaveChanges();
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


    }
}
