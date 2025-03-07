using BackendHrdAgro.Models.Attendance;
using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Master;
using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendHrdAgro.Controllers.Attendance
{
    public class PeriodeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<PeriodeController> _logger;

        public PeriodeController(IWebHostEnvironment env, ILogger<PeriodeController> logger)
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
        PeriodeDB periodeDb = new PeriodeDB();
        UserDB userDB = new UserDB();

        [HttpPost("{id}/periode")]
        public IActionResult leave(string id = "USR-201710052")
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


                List<PeriodeQuery> periodes = periodeDb.Periodes();
                List<PeriodeCreateQuery> periodesCreate = periodeDb.PeriodesCreate();

                var LinkCLosed = "";
                var LinkCreate = "";

                if (periodes == null)
                {
                    periodes = new List<PeriodeQuery>();
                }

                foreach (var k in periodes)
                {
                    //Link closed
                    if (k.Status == 99)
                    {
                        LinkCLosed = "<i class=\"fas fa-lock\"></i>";
                    }
                    else
                    {
                        LinkCLosed = $"<a href=\"#\" class=\"text-danger delete ms-2\" onclick=\"closed('{k.PeriodeId}')\"><i class=\"fas fa-lock-open\"></i></a>";

                    }

                    k.LinkCLosed = LinkCLosed;
                }


                if (periodesCreate == null)
                {
                    periodesCreate = new List<PeriodeCreateQuery>();
                }

                foreach (var m in periodesCreate)
                {
                    LinkCreate = $"<a href=\"#\" class=\"text-danger delete ms-2\" onclick=\"create('{m.PeriodeId}')\"><i class=\"fas fa-plus-square\"></i></a>";

                    m.LinkCreate = LinkCreate;
                }

                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                detail.Add("periodes", periodes);
                detail.Add("periodesCreate", periodesCreate);
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

        [HttpPost("{id}/periode/closed")]
        public ActionResult closed([FromBody] ClosedPeriodeModel value, string id = "USR-201710052")
        {
            TpAbsenteePeriode closed = new TpAbsenteePeriode()
            {

                PeriodeId = value.PeriodeId,
                UserEtrClosing = value.ClosedBy,
                DtEtrClosing = DateTime.Now,
                Status = 99
            };

            List<Dictionary<string, dynamic>> delete = periodeDb.ClosedPeriode(closed, id);

            bool myResult = delete[0]["result"];
            var myMessage = delete[0]["message"];

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

        [HttpPost("{id}/periode/create")]
        public ActionResult create([FromBody] CreatePeriodeModel value, string id = "USR-201710052")
        {
            var myYear = value.PeriodeId.Substring(0, 4);
            var myMonth = value.PeriodeId.Substring(4, 2);

            TpAbsenteePeriode createPeriode = new TpAbsenteePeriode()
            {

                PeriodeId = value.PeriodeId,
                UserEtr = value.UserEtrBy,
                Year = myYear,
                Month = myMonth,
                DtEtr = DateTime.Now,
                Status = 1
            };

            List<Dictionary<string, dynamic>> create = periodeDb.CreatePeriode(createPeriode, id);

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


    }
}
