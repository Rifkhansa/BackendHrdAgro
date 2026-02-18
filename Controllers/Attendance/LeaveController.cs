using BackendHrdAgro.Models.Attendance;
using BackendHrdAgro.Models.Leave;
using BackendHrdAgro.Models.Master;
using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Mvc;
using BackendHrdAgro.Models.Employee;
using Newtonsoft.Json;
using BackendHrdAgro.Models.Database.MySql;

namespace BackendHrdAgro.Controllers.Attendance
{
    [Route("api/request")]
    [ApiController]
    public class LeaveController : Controller
    {
        private readonly ILogger<LeaveController> _logger;
        private readonly IWebHostEnvironment _env;
        public LeaveController(ILogger<LeaveController> logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }

        BaseModel baseModel = new BaseModel();
        LeaveDB leaveDB = new LeaveDB();
        UserDB userDB = new UserDB();
        PeriodeDB periodeDB = new PeriodeDB();
        ErrorCodes errorCodes = new ErrorCodes();
        ErrorMessege errorMessege = new ErrorMessege();
        Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> detail = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> resp = new Dictionary<string, dynamic>();
        List<dynamic> listResp = new List<dynamic>();
        List<dynamic> listData = new List<dynamic>();
        DatabaseContext databaseContext = new DatabaseContext();


        [HttpPost("{id}/leave")]
        public IActionResult leave(string id = "USR-201709001")
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

                // ================= FULL ACCESS =================
                if (departmentId == "DP003"
                 || departmentId == "DP004" || employeeId == "070516")
                {
                    clausa = "";
                }

                // ================= USER LAIN =================
                else
                {
                    clausa = $" and a.employee_id='{employeeId}'";
                }

                List<LeaveQuery> leave = leaveDB.ListLeave(clausa);

                var LinkLeaveDel = "";
                var LinkLeaveApp = "";
                var revisi = "";

                if (leave == null)
                {
                    leave = new List<LeaveQuery>();
                }

                foreach (var k in leave)
                {
                    revisi = $"{k.CutiId}";

                    //Link Modal Edit
                    if (k.Status != 0) // cek untuk menampilkan edit (tidak berlaku untuk manager dan asmen
                    {
                        LinkLeaveDel = "";
                    }
                    else
                    {
                        if (k.Status == 0 && k.EmployeeId == employeeId)
                        {
                            LinkLeaveDel = $"<a href=\"#\" class=\"text-danger delete ms-2\" onclick=\"destroy('{revisi}')\"><i class=\"ti ti-trash fs-5\" title=\"Delete\"></i></a>";
                        }
                        else
                        {
                            LinkLeaveDel = "";
                        }
                    }

                    //Link Modal App
                    if (departmentId == "DP003" || titleId == "DS002" || departmentId == "DP004") //HRD, BOD
                    {
                        if (k.Status == 0)
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

                    k.LinkLeaveDel = LinkLeaveDel;
                    k.LinkLeaveApp = LinkLeaveApp;
                }

                float ReplacementRemaining = 0;
                float AnnualRemaining = 0;
                float LongRemaining = 0;

                var leaveRemaining = leaveDB.LeaveRemaining(employeeId).FirstOrDefault();

                if (leaveRemaining != null)
                {
                    ReplacementRemaining = leaveRemaining.SisaCutiReplacement;
                    AnnualRemaining = leaveRemaining.SisaCutiAnnual;
                    LongRemaining = leaveRemaining.SisaCutiLong;
                }

                List<TmTypeCuti> leaveTypes = new();

                // ambil cuti utama sesuai prioritas
                if (ReplacementRemaining > 0)
                    leaveTypes = leaveDB.LeaveTypeFindOnlyReplacement();
                else if (AnnualRemaining > 0)
                    leaveTypes = leaveDB.LeaveTypeFindOnlyAnnual();
                else if (LongRemaining > 0)
                    leaveTypes = leaveDB.LeaveTypeFindOnlyLong();

                // kalau ada cuti utama → tambahkan cuti lain
                if (leaveTypes.Any())
                {
                    // ambil ID cuti utama yang sedang aktif
                    var activeTypeIds = leaveTypes.Select(x => x.TypeCutiId).ToList();

                    var otherTypes = databaseContext.TmTypeCutis
                        .Where(x => x.Status == 1
                            // jangan ambil cuti utama lain
                            && !new[] { "CUT001", "CUT003", "CUT004", "CUT005", "CUT006", "CUT007", "CUT008" }
                                .Contains(x.TypeCutiId)
                        )
                        .ToList();

                    leaveTypes.AddRange(otherTypes);

                    // anti duplikat
                    leaveTypes = leaveTypes
                        .GroupBy(x => x.TypeCutiId)
                        .Select(x => x.First())
                        .ToList();
                }

                //List<TmSisaCuti> LeaveRemaining = leaveDB.LeaveRemaining(employeeId);
                //LeaveRemaining.ForEach(x =>
                //{
                //    longRemaining = x.SisaCutiLong;
                //});

                //List<TmTypeCuti> leaveTypes;

                //if (longRemaining > 0.75)
                //{
                //    leaveTypes = leaveDB.LeaveTypeFindOnlyLong();
                //}
                //else
                //{
                //    leaveTypes = leaveDB.LeaveTypeFind();
                //}

                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                detail.Add("Leaves", leave);
                detail.Add("leaveTypes", leaveTypes);
                detail.Add("leaveRemaining", leaveDB.LeaveRemaining(employeeId));
                detail.Add("totalLeave", leaveDB.TotalLeave(departmentId,employeeId));
                listData.Add(detail);


                data.Add("response", listResp);
                data.Add("message", "");
                data.Add("data", listData);
                return Ok(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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

        [HttpPost("{id}/createLeave")]
        public async Task<IActionResult> createLeave([FromBody] CreateLeaveModel value, string id)
        {
            float JumlahCuti = 0;
            int isCut = 0;
            List<string> insertDetailDate = new List<string>();

            string[] myDate = value.DateString.Split(',');
            float leaveTypeDay = 1;

            /*Mengecek Periode Closing*/
            string dateString = "";
            string[] myArrDate;
            bool checkPeriodeClosed = false;
            for (int i = 0; i < myDate.Length; i++)
            {
                myArrDate = myDate[i].Split('-');
                dateString = myArrDate[2] + myArrDate[1];

                checkPeriodeClosed = periodeDB.IsPeriodeAbsentClosed(dateString);
                if (checkPeriodeClosed == true)
                {
                    return BadRequest(new { message = "Periode Absent sudah ditutup.", success = 0 });
                }

            }

            switch (value.TypeCutiId)
            {
                case "CUT001":
                case "CUT003":
                    leaveTypeDay = float.Parse(value.TypeDay);
                    JumlahCuti = myDate.Length * leaveTypeDay;
                    isCut = 1;
                    break;
                case "CUT002":
                case "CUT004":
                case "CUT005":
                case "CUT006":
                case "CUT007":
                case "CUT008":
                case "CUT009":
                case "CUT010":
                case "CUT011":
                case "CUT013":
                case "CUT012":
                    JumlahCuti = myDate.Length;

                    break;
            }

            var requestId = BaseModel.GenerateId(tableName: "tp_cuti", str: "RC", primaryKey: "cuti_id", trailing: 4, lastKey: "NONE", date: DateTime.Now.ToString("yyMM"));
            TpCuti createLeave = new TpCuti()
            {
                CutiId = requestId,
                EmployeeId = value.EmployeeId,
                RequestDate = DateTime.Now,
                TypeCutiId = value.TypeCutiId,
                JumlahCuti = JumlahCuti,
                Keperluan = value.Keperluan,
                ApproveBy = " ",
                Noted = " ",
                IsRead = 0,
                Status = 0,
                CreatedAt = DateTime.Now,
                CreatedBy = id,
                IsNotify = 1
            };

            for (int i = 0; i < myDate.Length; i++)
            {
                insertDetailDate.Add(myDate[i]);
            }

            List<Dictionary<string, dynamic>> create = leaveDB.CreateLeave(createLeave, createLeave.CutiId, leaveTypeDay, insertDetailDate);

            bool myResult = create[0]["result"];
            var myMessage = create[0]["message"];

            if (myResult)
            {
                var department = EmployeeFactory.GetEmployeeDepartment(employeeId: value.EmployeeId) ?? throw new Exception($"Invalid Data, trace {JsonConvert.SerializeObject(value.EmployeeId)}");
                var leaveTypeName = LeaveDB.GetLeaveType(id: value.TypeCutiId) ?? throw new Exception("Invalid data, " + value.TypeCutiId);
                /*await NotificationFactory.Notify(
                   action: NotificationFactory.NotificationSenderAction.Management,
                   title: $"{department!.EmployeeName} mengajukan cuti {leaveTypeName}",
                   body: "Buka sistem HRIS untuk melihat detailnya",
                   employeeId: value.EmployeeId
                   );

                await NotificationFactory.NotifyWA(
                   action: NotificationFactory.NotificationSenderAction.Management,
                   title: $"*{department!.EmployeeName}* Mengajukan Cuti karena keperluan *{value.Keperluan}*",
                   body: $"ID Pengajuan : {requestId}{Environment.NewLine}Tanggal Diajukan : {value.DateString}{Environment.NewLine}{Environment.NewLine}_Segera lakukan approval pada sistem atau swipe pesan ini dengan perintah *!approve alasan*, atau *!reject alasan*_",
                   employeeId: value.EmployeeId
                   );*/

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

        [HttpPost("{id}/approvalLeave")]
        public async Task<ActionResult> approvalLeave([FromBody] ApprovalLeaveModel value, string id)
        {
            var checkData = leaveDB.CountLeaveData(value.CutiId);
            var isApproved = leaveDB.CheckIsApprove(value.CutiId);
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
            /*Mengecek Periode Closing*/
            string dateString = "";
            string[] myArrDate;
            bool checkPeriodeClosed = false;
            DateTime leaveDate;

            List<TpDetailCuti> leaves = leaveDB.ListLeaveDetail(value.CutiId);

            foreach (var leave in leaves)
            {
                leaveDate = leave.CutiDate;
                dateString = leaveDate.ToString("yyyyMM");

                checkPeriodeClosed = periodeDB.IsPeriodeAbsentClosed(dateString);
                if (checkPeriodeClosed == true)
                {
                    return BadRequest(new { message = "Periode Absent sudah ditutup.", success = 0 });
                }

            };

            var approvedBy = value.ApproveBy;
            var uId = id;
            //kondisi jika approval by wa
            /*if (value.ApproveBy.StartsWith("628"))
            {
                var WANumber = "0" + value.ApproveBy.Substring(2);
                Console.WriteLine("WANumber= : " + WANumber);
                var findEmployeeId = userDB.FindEmployeeIdByNumberWA(WANumber);
                approvedBy = findEmployeeId[0].EmployeeId;
            }
            if (id.StartsWith("628"))
            {
                var WANumber = "0" + id.Substring(2);
                Console.WriteLine("WANumber= : " + WANumber);
                var findEmployeeId = userDB.FindEmployeeIdByNumberWA(WANumber);
                uId = findEmployeeId[0].userId;
            }*/
            //

            // ================= ACCESS OF APPROVAL =================
            var findSessionData = userDB.FindSessionDataUser(uId); // Approver
            var findRequestData = userDB.FindRequestData("tp_cuti", "cuti_id", value.CutiId);
            var findSessionDataStaff = userDB.FindSessionDataByEmployeeId(findRequestData[0].EmployeeId);

            var approverId = findSessionData[0].EmployeeId;
            var approverDept = findSessionData[0].DepartmentId;
            var staffId = findSessionDataStaff[0].EmployeeId;
            var staffDept = findSessionDataStaff[0].DepartmentId;

            // SUPER ADMIN → BEBAS
            if (approverDept == "DP003" || approverDept == "DP004" || approverId == "070516")
            {
                // allow
            }
            else
            {

                return BadRequest(new { message = "Anda tidak diizinkan untuk melakukan approval", success = 0 });

            }
            // ================= END ACCESS OF APPROVAL =================

            TpCuti approvalLeave = new TpCuti()
            {
                CutiId = value.CutiId,
                ApproveBy = approvedBy,
                ApproveDate = DateTime.Now,
                Noted = value.Noted,
                Status = value.Status,
                IsNotify = 5,
                UpdatedAt = DateTime.Now,
                UpdatedBy = uId
            };

            List<Dictionary<string, dynamic>> approve = leaveDB.ApproveLeave(approvalLeave, id);

            bool myResult = approve[0]["result"];
            var myMessage = approve[0]["message"];

            DateTime leaveDateRequest = leaveDB.findLeaveDate(value.CutiId);
            string leaveDateRequestView = leaveDateRequest.ToString("dd MMM yyyy");
            if (myResult)
            {
                var leaveTypeId = LeaveDB.GetLeaveTypeId(leaveId: value.CutiId) ?? throw new Exception("Invalid data, " + value.CutiId);
                var leaveTypeName = LeaveDB.GetLeaveType(id: leaveTypeId) ?? throw new Exception("Invalid data, " + leaveTypeId);

                /*await NotificationFactory.Notify(
                    action: NotificationFactory.NotificationSenderAction.Employee,
                    title: $"{EmployeeFactory.EmployeeName(approvedBy)} {(value.Status == 1 ? "menyetujui" : "menolak")} pengajuan cuti {leaveTypeName}",
                    body: "Buka sistem HRIS untuk melihat detailnya",
                    employeeId: LeaveDB.GetEmployee(leaveId: value.CutiId) ?? throw new Exception("Invalid data, " + value.CutiId)
                    );

                await NotificationFactory.NotifyWA(
                    action: NotificationFactory.NotificationSenderAction.Employee,
                    title: $"{EmployeeFactory.EmployeeName(approvedBy)} {(value.Status == 1 ? "menyetujui" : "menolak")} pengajuan cuti {leaveTypeName}",
                    body: $"dengan id pengajuan {value.CutiId}, dan tanggal pengajuan {leaveDateRequestView}{Environment.NewLine}_Note : {value.Noted}_",
                    employeeId: LeaveDB.GetEmployee(leaveId: value.CutiId) ?? throw new Exception("Invalid data, " + value.CutiId)
                    );*/

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

        [HttpPost("{id}/deleteLeave")]
        public ActionResult delete([FromBody] DeleteLeaveModel value, string id = "USR-201710052")
        {
            /*Mengecek Periode Closing*/
            string dateString = "";
            string[] myArrDate;
            bool checkPeriodeClosed = false;
            DateTime leaveDate;

            List<TpDetailCuti> leaves = leaveDB.ListLeaveDetail(value.CutiId);

            foreach (var leave in leaves)
            {
                leaveDate = leave.CutiDate;
                dateString = leaveDate.ToString("yyyyMM");

                checkPeriodeClosed = periodeDB.IsPeriodeAbsentClosed(dateString);
                if (checkPeriodeClosed == true)
                {
                    return BadRequest(new { message = "Periode Absent sudah ditutup.", success = 0 });
                }

            };

            TpCuti deleteLeave = new TpCuti()
            {
                CutiId = value.CutiId,
                DeletedBy = value.DeletedBy,
                DeletedAt = DateTime.Now,
            };

            List<Dictionary<string, dynamic>> delete = leaveDB.DeleteLeave(deleteLeave, id);

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


    }
}
