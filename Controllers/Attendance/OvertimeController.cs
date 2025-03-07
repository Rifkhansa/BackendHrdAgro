using BackendHrdAgro.Models.Attendance;
using BackendHrdAgro.Models.Master;
using BackendHrdAgro.Models.Employee;
using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BackendHrdAgro.Controllers.Attendance
{
    [Route("api/request")]
    [ApiController]
    public class OvertimeController : Controller
    {
        private readonly ILogger<OvertimeController> _logger;
        private readonly IWebHostEnvironment _env;
        public OvertimeController(ILogger<OvertimeController> logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }

        BaseModel baseModel = new BaseModel();
        UserDB userDB = new UserDB();
        ErrorCodes errorCodes = new ErrorCodes();
        ErrorMessege errorMessege = new ErrorMessege();
        Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> detail = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> resp = new Dictionary<string, dynamic>();
        List<dynamic> listResp = new List<dynamic>();
        List<dynamic> listData = new List<dynamic>();
        OvertimeModel overtimeModel = new OvertimeModel();
        PeriodeDB periodeDB = new PeriodeDB();


        [HttpPost("{id}/overtime")]
        public IActionResult overtime(string id = "USR-201710052")
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
                        clausa = $" and f.div_id='{divId}'";
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

                List<OvertimeQuery> overtime = overtimeModel.ListOvertimes(clausa);

                var linkOverTimeDel = "";
                var linkOverTimeApp = "";

                var revisi = "";
                foreach (var k in overtime)
                {

                    //revisi = $"{k.EmployeeId}|{k.RequestId}";
                    revisi = $"{k.RequestId}";

                    //Link Modal Edit
                    if (titleId == "DS002" || titleId == "DS003" || k.Status != 0 || k.Dimunculin == 0) // cek untuk menampilkan edit (tidak berlaku untuk manager dan asmen
                    {
                        linkOverTimeDel = "";
                    }
                    else
                    {
                        if (k.Status == 0)
                        {
                            linkOverTimeDel = $"<a href=\"#\" class=\"text-danger delete ms-2\" onclick=\"destroy('{revisi}')\"><i class=\"ti ti-trash fs-5\" title=\"Delete\"></i></a>";
                        }
                        else
                        {
                            linkOverTimeDel = "";
                        }
                    }

                    //Link Modal App
                    if (departmentId == "DP006" || titleId == "DS002" || employeeId == "0913021" || employeeId == "0711011") //HRD, manager, pak Yo, stepi
                    {
                        if (k.Status == 0)
                        {
                            if (employeeId != k.EmployeeId)
                            {

                                linkOverTimeApp = $" <a href = 'javascript:void(0)' class='approve-btn text-dark edit ms-2' data-bs-toggle='modal' data-bs-target='#modalFormTypeApp' data-id='{revisi}' data-from='TypeHrd'><i class='far fa-check-square'></i></a>";

                            }
                            else
                            {
                                linkOverTimeApp = "";
                            }
                        }
                        else
                        {
                            linkOverTimeApp = "";
                        }
                    }
                    else
                    {
                        linkOverTimeApp = "";
                    }

                    k.linkOverTimeDel = linkOverTimeDel;
                    k.LinkOverTimeApp = linkOverTimeApp;
                }

                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                detail.Add("Overtimes", overtime);
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

        [HttpPost("{id}/createOvertime")]
        public async Task<IActionResult> createOvertime([FromBody] CreateOvertimeModel value, string id = "USR-201710052")
        {
            TimeSpan timeEndSpan = TimeSpan.Parse(value.TimeEnd);

            DateTime requestDate = DateTime.Now;
            string dateString = "";
            dateString = requestDate.ToString("yyyyMM");

            bool checkPeriodeClosed = false;

            checkPeriodeClosed = periodeDB.IsPeriodeAbsentClosed(dateString);

            if (checkPeriodeClosed == true)
            {
                return BadRequest(new { message = "Periode Absent sudah ditutup.", success = 0 });
            }

            var requestId = BaseModel.GenerateId(tableName: "tp_request", str: "RO", primaryKey: "request_id", trailing: 4, lastKey: "NONE", date: DateTime.Now.ToString("yyMM"));
            Console.WriteLine(requestId + "requestID");
            TpRequest createOvertime = new TpRequest()
            {
                RequestId = requestId,
                EmployeeId = value.EmployeeId,
                RequestDate = DateTime.Now,
                TimeStart = "18:00",
                TimeEnd = timeEndSpan,
                Keperluan = value.Keperluan,
                RequestTypeId = "1",
                IsRead = 0,
                Status = 0,
                CreatedAt = DateTime.Now,
                CreatedBy = id,
                CutAbsent = 0,
                TotalOvertime = "0",
                IsNotify = 1
            };

            List<Dictionary<string, dynamic>> create = overtimeModel.CreateOvertime(createOvertime);

            bool myResult = create[0]["result"];
            var myMessage = create[0]["message"];

            if (myResult)
            {

                /*var department = EmployeeFactory.GetEmployeeDepartment(employeeId: value.EmployeeId) ?? throw new Exception($"Invalid Data, trace {JsonConvert.SerializeObject(value.EmployeeId)}");
                await NotificationFactory.Notify(
                   action: NotificationFactory.NotificationSenderAction.Management,
                   title: $"{department!.EmployeeName} mengajukan Overtime",
                   body: value.Keperluan,
                   employeeId: value.EmployeeId
                   );

                await NotificationFactory.NotifyWA(
                   action: NotificationFactory.NotificationSenderAction.Management,
                   title: $"*{department!.EmployeeName}* mengajukan Overtime karena keperluan *{value.Keperluan}*, dengan Id {requestId}",
                   body: $"ID Pengajuan : {requestId}{Environment.NewLine}Tanggal Diajukan : {DateTime.Now}{Environment.NewLine}{Environment.NewLine}_Segera lakukan approval pada sistem atau swipe pesan ini dengan perintah *!approve alasan*, atau *!reject alasan*_",
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

        [HttpPost("{id}/approvalOvertime")]
        public async Task<ActionResult> approvalOvertime([FromBody] ApprovalOvertimeModel value, string id)
        {

            var checkData = overtimeModel.CountOvertimeData(value.RequestId);
            var isApproved = overtimeModel.CheckIsApprove(value.RequestId);
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

            List<TpRequest> requests = overtimeModel.OvertimeFind(value.RequestId);

            DateTime requestDate;
            string dateString = "";
            requests.ForEach(x =>
            {
                requestDate = x.RequestDate;
                dateString = requestDate.ToString("yyyyMM");
            });

            bool checkPeriodeClosed = false;

            checkPeriodeClosed = periodeDB.IsPeriodeAbsentClosed(dateString);

            if (checkPeriodeClosed == true)
            {
                return BadRequest(new { message = "Periode Absent sudah ditutup.", success = 0 });
            }

            var approvedBy = value.ApproveBy;
            var uId = id;
            //kondisi jika approval by wa
            /*if (value.ApproveBy.StartsWith("628"))
            {
                var WANumber = "0" + value.ApproveBy.Substring(2); // Ganti "62" dengan "0" dan ambil sisa string
                Console.WriteLine("WANumber= : " + WANumber);
                var findEmployeeId = userDB.FindEmployeeIdByNumberWA(WANumber);
                approvedBy = findEmployeeId[0].EmployeeId;
            }
            if (id.StartsWith("628"))
            {
                var WANumber = "0" + id.Substring(2); // Ganti "62" dengan "0" dan ambil sisa string
                Console.WriteLine("WANumber= : " + WANumber);
                var findEmployeeId = userDB.FindEmployeeIdByNumberWA(WANumber);
                uId = findEmployeeId[0].userId;
            }*/
            //

            //access of approvall
            var findSessionData = userDB.FindSessionDataUser(uId);
            var findRequestData = userDB.FindRequestData("tp_request", "request_id", value.RequestId);
            Console.WriteLine("emplId = " + findRequestData[0].EmployeeId);
            var findSessionDataStaff = userDB.FindSessionDataByEmployeeId(findRequestData[0].EmployeeId);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(findSessionData));
            Console.WriteLine("usr = " + findSessionData[0].DepartmentId);
            Console.WriteLine("staff = " + findSessionDataStaff[0].DepartmentId);

            if (findSessionData[0].DepartmentId != "DP006")
            {
                if (findSessionData[0].EmployeeId != "0808003")
                {
                    if (findSessionDataStaff[0].DepartmentId == "DP011" && findSessionData[0].EmployeeId == "0711011")
                    {
                        Console.WriteLine("User : " + findSessionData[0].fullName);
                    }
                    else
                    {
                        if (EmployeeFactory.IsHeadDepartment(employeeId: findSessionDataStaff[0].EmployeeId))
                        {
                            var headDiv = EmployeeFactory.GetHeadDivision(divisionId: findSessionDataStaff[0].DivId);
                            if (findSessionDataStaff[0].EmployeeId != "0321080" && findSessionDataStaff[0].EmployeeId != "0808004")
                            {
                                if (findSessionData[0].EmployeeId != headDiv)
                                {
                                    return BadRequest(new { message = "Anda tidak diizinkan untuk melakukan approval.", success = 0 });
                                }
                            }
                            else
                            {
                                if (findSessionData[0].EmployeeId != "0808003")
                                {
                                    return BadRequest(new { message = "Anda tidak diizinkan untuk melakukan approval.", success = 0 });
                                }
                            }
                        }
                        else
                        {
                            var headDiv = EmployeeFactory.GetHeadDivision(divisionId: findSessionDataStaff[0].DivId);

                            if (findSessionData[0].EmployeeId == headDiv)
                            {
                                Console.WriteLine("Im Head Div " + findSessionData[0].EmployeeId);
                                // return BadRequest(new { message = "Anda tidak diizinkan untuk melakukan approvals.", success = 0 });
                            }
                            else
                            {
                                if (findSessionDataStaff[0].DepartmentId != findSessionData[0].DepartmentId)
                                {
                                    return BadRequest(new { message = "Anda tidak diizinkan untuk melakukan approval, dikarnakan berbeda departement", success = 0 });
                                }
                                else
                                {
                                    var headDept = EmployeeFactory.GetHeadDepartment(departmentId: findSessionDataStaff[0].DepartmentId);

                                    if (findSessionData[0].EmployeeId != headDept)
                                    {
                                        if (findSessionData[0].DepartmentId == "DP011")
                                        {
                                            if (findSessionData[0].EmployeeId != "0711011")
                                            {

                                                return BadRequest(new { message = $"Anda tidak diizinkan untuk melakukan approval", success = 0 });
                                            }
                                        }
                                        else
                                        {
                                            return BadRequest(new { message = $"Anda tidak diizinkan untuk melakukan approval", success = 0 });
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }


            if (findSessionDataStaff[0].DepartmentId == "")
            {

            }
            //end access off approvall

            TpRequest approvalOvertime = new TpRequest()
            {
                RequestId = value.RequestId,
                ApproveBy = approvedBy,
                Noted = value.Noted,
                Status = value.Status,
                IsNotify = 5,
                UpdatedAt = DateTime.Now,
                UpdatedBy = uId
            };

            List<Dictionary<string, dynamic>> create = overtimeModel.ApproveOvertime(approvalOvertime, id);

            bool myResult = create[0]["result"];
            var myMessage = create[0]["message"];

            DateTime findDateRequest = overtimeModel.FindOvertimeDate(value.RequestId);
            string findDateRequestView = findDateRequest.ToString("dd MMM yyyy");
            if (myResult)
            {
                /*await NotificationFactory.Notify(
                   action: NotificationFactory.NotificationSenderAction.Employee,
                   title: $"{EmployeeFactory.EmployeeName(approvedBy)} {(value.Status == 1 ? "menyetujui" : "menolak")} pengajuan Overtime ",
                   body: "Buka sistem HRIS untuk melihat detailnya",
                   employeeId: OvertimeDB.GetEmployee(requestId: value.RequestId) ?? throw new Exception("Invalid data, " + value.RequestId)
                   );

                await NotificationFactory.NotifyWA(
                  action: NotificationFactory.NotificationSenderAction.Employee,
                  title: $"{EmployeeFactory.EmployeeName(approvedBy)} {(value.Status == 1 ? "menyetujui" : "menolak")} pengajuan Overtime ",
                  body: $"dengan id pengajuan {value.RequestId}, dan tanggal pengajuan {findDateRequestView}{Environment.NewLine}_Note : {value.Noted}_",
                  employeeId: OvertimeDB.GetEmployee(requestId: value.RequestId) ?? throw new Exception("Invalid data, " + value.RequestId)
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

        [HttpPost("{id}/deleteOvertime")]
        public ActionResult delete([FromBody] DeleteOvertimeModel value, string id)
        {

            List<TpRequest> requests = overtimeModel.OvertimeFind(value.RequestId);

            DateTime requestDate;
            string dateString = "";
            requests.ForEach(x =>
            {
                requestDate = x.RequestDate;
                dateString = requestDate.ToString("yyyyMM");
            });

            bool checkPeriodeClosed = false;

            checkPeriodeClosed = periodeDB.IsPeriodeAbsentClosed(dateString);

            if (checkPeriodeClosed == true)
            {
                return BadRequest(new { message = "Periode Absent sudah ditutup.", success = 0 });
            }


            TpRequest deleteOvertime = new TpRequest()
            {

                RequestId = value.RequestId,
                DeletedBy = value.DeletedBy,
                DeletedAt = DateTime.Now,

            };

            List<Dictionary<string, dynamic>> create = overtimeModel.DeleteOvertime(deleteOvertime, id);

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
