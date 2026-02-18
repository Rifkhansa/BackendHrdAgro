using Microsoft.AspNetCore.Mvc;
using BackendHrdAgro.Controllers.Employee;
using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Employee;
using BackendHrdAgro.Models;
using BackendHrdAgro.Models.Attendance;
using BackendHrdAgro.Models.Master;
using BackendHrdAgro.Models.Leave;
using BackendHrdAgro.Models.Database.MySql.Support;

namespace BackendHrdAgro.Controllers.Attendance
{
    [Route("api/replacement")]
    [ApiController]
    public class ReplacementController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ReplacementController> _logger;

        public ReplacementController(IWebHostEnvironment env, ILogger<ReplacementController> logger)
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
        DatabaseContext databaseContext = new DatabaseContext();
        ReplacementModel replacementModel = new ReplacementModel();
        TpReplacementDetail tpReplacementDetail = new TpReplacementDetail();
        TpReplacementLeave tpReplacementLeave = new TpReplacementLeave();

        [HttpGet("{id}")]
        public IActionResult Index(string id = "USR-201709001")
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

                if (departmentId == "DP003" || employeeId == "0808003" || departmentId == "DP004") // untuk hrd
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

                List<ReplacementView> replacement = replacementModel.ListReplacement(kriteria);
                Console.WriteLine("replacement" + replacement);

                if (replacement == null)
                {
                    replacement = new List<ReplacementView>();
                }


                var LinkLeaveApp = "";
                var revisi = "";

                foreach (var k in replacement)
                {
                    revisi = $"{k.ReplacementId}";
                    Console.WriteLine($"DEBUG: dept={departmentId}, title={titleId}, empId={employeeId}, k.EmpId={k.EmployeeId}, status={k.ApproveStatus}");

                    //Link Modal App
                    if (departmentId == "DP003" || titleId == "DS002" || departmentId == "DP004" ) //HRD, manager,
                    {
                        if (k.ApproveStatus == 0)
                        {
                            if (employeeId != k.EmployeeId)
                            {

                                LinkLeaveApp = $" <a href = 'javascript:void(0)' class='approve-btn text-dark edit ms-2' data-bs-toggle='modal' data-bs-target='#modalFormTypeApp' data-id='{revisi}' data-from='TypeHrd'><i class='far fa-check-square'></i></a>";

                            }
                            else
                            {
                                LinkLeaveApp = "";
                            }
                        }
                        else
                        {
                            LinkLeaveApp = "";
                        }
                    }
                    else
                    {
                        LinkLeaveApp = "";
                    }

                    k.LinkLeaveApp = LinkLeaveApp;
                }

                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("replacement", replacement);

                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("message", "");
                Data.Add("data", ListData);

                return Ok(Data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException == null ? ex : ex.InnerException);
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
        public ActionResult create([FromBody] CreateReplacement value, string id = "USR-201710052")
        {

            if (string.IsNullOrEmpty(value.Description))
            {
                return BadRequest(new { msg = "Description harus diisi.", success = 0 });
            }

            var replacementId = BaseModel.GenerateId(tableName: "tp_replacement_leave", primaryKey: "replacement_id", str: "REL", trailing: 3, lastKey: "NONE");
            Console.WriteLine(replacementId);
            TpReplacementLeave createReplacementLeave = new TpReplacementLeave()
            {
                ReplacementId = replacementId,
                EmployeeId = value.EmployeeId,
                RequestDate = DateTime.Now,
                Description = value.Description,
                CreatedAt = DateTime.Now,
                CreatedBy = id
            };

            List<string> insertDetailDate = new List<string>();

            string[] myDate = value.DateString.Split(',');

            for (int i = 0; i < myDate.Length; i++)
            {
                insertDetailDate.Add(myDate[i]);
            }

            List<Dictionary<string, dynamic>> create = replacementModel.CreateReplacements(createReplacementLeave, createReplacementLeave.ReplacementId, insertDetailDate);

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
                Data.Add("message", myMessage);
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

        [HttpPost("{id}/approval")]
        public async Task<ActionResult> approvalReplacement([FromBody] ApprovalReplacementModel value, string id)
        {
            Console.WriteLine("masuk approval");
            var checkData = replacementModel.CountLeaveData(value.replacementId);
            var isApproved = replacementModel.CheckIsApprove(value.replacementId);
            Console.WriteLine("checkData : " + checkData);
            Console.WriteLine("checkData : " + isApproved.ToString());
            if (checkData == 0)
            {
                return BadRequest(new { message = "Data tidak ditemukan.", success = 0 });
            }
            if (!isApproved)
            {
                return BadRequest(new { message = "Data tersebut sudah pernah melakukan tahap approval.", success = 0 });
            }

            var approvedBy = value.ApproveBy;
            var uId = id;

            //ACCESS of approvall
            var findSessionData = userDB.FindSessionDataUser(uId);
            var findRequestData = userDB.FindRequestData("tp_replacement_leave", "replacement_id", value.replacementId);
            Console.WriteLine("emplId = " + findRequestData[0].EmployeeId);
            var findSessionDataStaff = userDB.FindSessionDataByEmployeeId(findRequestData[0].EmployeeId);
            Console.WriteLine("usr = " + findSessionData[0].DepartmentId);
            Console.WriteLine("staff = " + findSessionDataStaff[0].DepartmentId);
            Console.WriteLine("cek: " + EmployeeFactory.IsHeadDepartment(employeeId: findSessionData[0].EmployeeId));

            var approverId = findSessionData[0].EmployeeId;
            var approverDept = findSessionData[0].DepartmentId;
            var staffId = findSessionDataStaff[0].EmployeeId;
            var staffDept = findSessionDataStaff[0].DepartmentId;

            // SUPER ADMIN → BEBAS
            if (approverDept == "DP003" || approverDept == "DP004") //BOD, HR
            {
                // allow
            }
            else
            {
                // =========================
                // LISAN
                // =========================
                 if (approverId == "070516")
                {
                    if (!(staffDept == "DP002" || staffDept == "DP005" || staffDept == "DP006")) 
                    {
                        return BadRequest(new { message = "Anda tidak diizinkan untuk melakukan approval", success = 0 });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Anda tidak diizinkan untuk melakukan approval", success = 0 });
                }
            }
            // ================= END ACCESS OF APPROVAL =================


            TpReplacementLeave approvalReplacement = new TpReplacementLeave()
            {
                ReplacementId = value.replacementId,
                ApproveBy = approvedBy,
                ApproveDate = DateTime.Now,
                ApproveReason = value.ApproveReason,
                ApproveStatus = value.ApproveStatus,
                UpdatedAt = DateTime.Now,
                UpdatedBy = uId
            };

            List<Dictionary<string, dynamic>> approve = replacementModel.ApproveReplacement(approvalReplacement);

            bool myResult = approve[0]["result"];
            var myMessage = approve[0]["message"];

            if (myResult)
            {
                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", myMessage);
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
        public ActionResult Delete([FromBody] DeleteReplacementModel value, string id = "USR-201710052")
        {
            TpReplacementLeave deleteReplacement = new TpReplacementLeave()
            {
                ReplacementId = value.ReplacementId,
                DeletedBy = value.DeletedBy,
                DeletedAt = DateTime.Now
            };

            List<Dictionary<string, dynamic>> delete = replacementModel.DeleteReplacement(deleteReplacement, id);

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
