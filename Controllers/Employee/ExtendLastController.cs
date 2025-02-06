using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Mvc;
using BackendHrdAgro.Models.Employee;

namespace BackendHrdAgro.Controllers.Employee
{
    [Route("api/extendLast")]
    [ApiController]
    public class ExtendLastController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ExtendLastController> _logger;
        public ExtendLastController(IWebHostEnvironment env, ILogger<ExtendLastController> logger)
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
        DatabaseContext databaseContext = new DatabaseContext();
        ExtendLastModel extendLastModel = new ExtendLastModel();

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            try
            {
                var extendLast = extendLastModel.extendLastGroupQueries();
                Console.WriteLine(extendLast);
                var reminder = extendLastModel.reminders();
                Console.WriteLine(reminder);

                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("extendLast", extendLast);
                Detail.Add("reminder", reminder);

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

        [HttpPost("{id}/signextendlast")]
        public ActionResult SignExtendLast([FromBody] ExtendLastSignRequestBody value)
        {
            TpExtend tpExtendLast = new TpExtend();

            if (string.IsNullOrEmpty(value.EmployeeId))
            {
                return BadRequest(new { msg = "Employee id harus diisi.", success = 0 });
            }
            if (string.IsNullOrEmpty(value.Reason))
            {
                return BadRequest(new { msg = "Reason harus diisi.", success = 0 });
            }

            tpExtendLast.ExtendId = BaseModel
                .GenerateId(
                tableName: "tp_extend",
                primaryKey: "extend_id",
                str: "ED",
                trailing: 3,
                lastKey: "NONE");
            tpExtendLast.EmployeeId = value.EmployeeId;
            tpExtendLast.StartExtendDate = value.StartExtendDate;
            tpExtendLast.EndExtendDate = value.EndExtendDate;
            tpExtendLast.Reason = value.Reason;
            tpExtendLast.Status = 0;

            tpExtendLast.FileName = "";
            tpExtendLast.Type = "";
            tpExtendLast.Size = 0;

            List<Dictionary<string, dynamic>> create = extendLastModel.CreateExtendLast(tpExtendLast);

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



    }
}
