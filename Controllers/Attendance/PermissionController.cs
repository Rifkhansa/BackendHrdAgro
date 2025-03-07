using BackendHrdAgro.Models.Attendance;
using BackendHrdAgro.Models.Master;
using BackendHrdAgro.Models.Permission;
using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Mvc;
using BackendHrdAgro.Models.Database.MySql.View;
using BackendHrdAgro.Models.Employee;
using Newtonsoft.Json;

namespace BackendHrdAgro.Controllers.Attendance
{
    [Route("api/request")]
    [ApiController]
    public class PermissionController : Controller
    {
        private readonly ILogger<PermissionController> _logger;
        private readonly IWebHostEnvironment _env;
        public PermissionController(ILogger<PermissionController> logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }

        BaseModel baseModel = new BaseModel();
        PermissionDB permissionDB = new PermissionDB();
        UserDB userDB = new UserDB();
        PeriodeDB periodeDB = new PeriodeDB();
        ErrorCodes errorCodes = new ErrorCodes();
        ErrorMessege errorMessege = new ErrorMessege();
        Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> detail = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> resp = new Dictionary<string, dynamic>();
        List<dynamic> listResp = new List<dynamic>();
        List<dynamic> listData = new List<dynamic>();


        [HttpPost("{id}/permission")]
        public IActionResult permission(string id = "USR-201710052")
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
                        clausa = $" and h.div_id='{divId}'";
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

                List<PermissionQuery> permission = permissionDB.ListPermission(clausa);

                var LinkPermissionDel = "";
                var LinkPermissionApp = "";

                var revisi = "";

                if (permission == null)
                {
                    permission = new List<PermissionQuery>();
                }

                foreach (var k in permission)
                {
                    revisi = $"{k.PermissionId}";

                    //Link Modal Edit
                    if (k.Status != 0) // cek untuk menampilkan edit (tidak berlaku untuk manager dan asmen
                    {
                        LinkPermissionDel = "";
                    }
                    else
                    {
                        if (k.Status == 0 && k.EmployeeId == employeeId)
                        {
                            //linkOverTimeDel = $" <a href = 'javascript:void(0)' class='edit-btn text-dark edit ms-2' data-bs-toggle='modal' data-bs-target='#modalFormTypeUser' data-id='{revisi}' data-from='edit'><i class='fas fa-pencil-alt'></i></a>";
                            LinkPermissionDel = $"<a href=\"#\" class=\"text-danger delete ms-2\" onclick=\"destroy('{revisi}')\"><i class=\"ti ti-trash fs-5\" title=\"Delete\"></i></a>";
                        }
                        else
                        {
                            LinkPermissionDel = "";
                        }
                    }

                    //Link Modal App
                    if (departmentId == "DP006" || titleId == "DS002" || employeeId == "0913021" || employeeId == "0711011") //HRD, manager, pak Yo, stepi
                    {
                        if (k.Status == 0)
                        {
                            if (employeeId != k.EmployeeId)
                            {

                                LinkPermissionApp = $" <a href = 'javascript:void(0)' class='approve-btn text-dark edit ms-2' data-bs-toggle='modal' data-bs-target='#modalFormTypeApp' data-id='{revisi}' data-from='TypeHrd'><i class='far fa-check-square'></i></a>";

                            }
                            else
                            {
                                LinkPermissionApp = "";
                            }
                        }
                        else
                        {
                            LinkPermissionApp = "";
                        }
                    }
                    else
                    {
                        LinkPermissionApp = "";
                    }

                    k.LinkPermissionDel = LinkPermissionDel;
                    k.LinkPermissionApp = LinkPermissionApp;
                }

                List<PermissionQuery> permissionWithoutLetterThisYear = permissionDB.ListPermissionWithoutLetterThisYear(employeeId);

                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                detail.Add("Permissions", permission);
                detail.Add("PermissionsWithoutLetter", permissionWithoutLetterThisYear);
                detail.Add("PermissionTypes", permissionDB.PermissionTypeFind());
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

        [HttpPost("{id}/createPermission")]
        public async Task<IActionResult> createPermission([FromForm] CreatePermissionModel value, string id)
        {
            if (string.IsNullOrEmpty(value.EmployeeId))
            {
                return BadRequest(new { message = "Employee id harus diisi.", success = 0 });
            }

            if (string.IsNullOrEmpty(value.DateString))
            {
                return BadRequest(new { message = "tanggal harus diisi.", success = 0 });
            }

            if (string.IsNullOrEmpty(value.Reason))
            {
                return BadRequest(new { message = "Reason harus diisi.", success = 0 });
            }

            if (value.PermissionTypeId == "-1")
            {
                return BadRequest(new { message = "Type harus dipilih.", success = 0 });
            }
            if (string.IsNullOrEmpty(value.PermissionTypeId))
            {
                return BadRequest(new { message = "Type harus dipilih.", success = 0 });
            }

            List<string> insertDetailDate = new List<string>();

            string[] myDate = value.DateString.Split(',');

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

            /*mengecek izin sakit tanpa surat dokter*/
            if (value.PermissionTypeId == "PER002")
            {
                int getNumber = 0;
                int getNumberNext = 0;

                getNumber = permissionDB.GetPermissionWithoutLetter(value.EmployeeId);
                getNumberNext = getNumber + myDate.Length;
                if (getNumber >= 5)
                {
                    return BadRequest(new { message = "Izin sakit tanpa surat dokter tercatat di sistem sudah atau lebih dari 5 hari, silahkan lakukan pengajuan cuti di sistem HRIS.", success = 0 });
                }
                if (getNumberNext > 5)
                {
                    return BadRequest(new { message = "Pengajuan izin sakit tanpa surat dokter akan melebihi 5 hari, silahkan coba lagi dengan mengurangi jumlah harinya.", success = 0 });
                }
            }

            /*mengecek datang terlambat atau pulang cepat */
            if (value.PermissionTypeId == "PER004" || value.PermissionTypeId == "PER005")
            {
                int getNumber = 0;
                getNumber = permissionDB.CheckPermissionComeLate(value.EmployeeId);

                //return BadRequest(new { message = $"You have applied for a permit with this type exceeding the maximum limit per month, which is  2x/month.  Please fill leave form in HRIS System {getNumber},  {value.EmployeeId}", success = 0 });
                if (getNumber >= 1 && myDate.Length > 1)
                {
                    return BadRequest(new { message = "You have applied for a permit with this type exceeding the maximum limit per month, which is  2x/month. Please fill leave form in HRIS System", success = 0 });
                }
                if (myDate.Length > 2)
                {
                    return BadRequest(new { message = $"You have applied for a permit with this type exceeding the maximum limit per month, which is  2x/month.  Please fill leave form in HRIS System", success = 0 });
                }

                if (getNumber >= 2)
                {
                    return BadRequest(new { message = $"You have applied for a permit with this type exceeding the maximum limit per month, which is  2x/month.  Please fill leave form in HRIS System", success = 0 });
                }
            }

            var requestId = BaseModel.GenerateId(tableName: "tp_permission", str: "RP", primaryKey: "permission_id", trailing: 4, lastKey: "NONE", date: DateTime.Now.ToString("yyMM"));
            TpPermission createPermission = new TpPermission()
            {
                PermissionId = requestId,
                EmployeeId = value.EmployeeId,
                RequestDate = DateTime.Now,
                Reason = value.Reason,
                PermissionTypeId = value.PermissionTypeId,
                ApproveBy = " ",
                Noted = " ",
                IsRead = 0,
                Status = 0,
                CreatedBy = id,
                CreatedAt = DateTime.Now,
                IsNotify = 1
            };

            for (int i = 0; i < myDate.Length; i++)
            {
                insertDetailDate.Add(myDate[i]);
            }

            if (value.FileName != null && value.FileName.Length > 0)
            {
                Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.Permitt, isProduction: _env.IsProduction()));
                string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.Permitt, isProduction: _env.IsProduction()), value.EmployeeId);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                IFormFile file = value.FileName;
                var fileName = Path.GetFileName(file.FileName);
                fileName = fileName.Replace(" ", "_");
                var filePath = Path.Combine(directoryPath, fileName);

                // Cek file dgn nama yang sama sudah ada, jika ada, hapus
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Save file
                using (var stream = new FileStream(path: filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    fileName = fileName;
                }

                createPermission.FileName = fileName;
                createPermission.Type = value.FileName.ContentType;
                createPermission.Size = 0;
            }

            List<Dictionary<string, dynamic>> create = permissionDB.CreatePermission(createPermission, createPermission.PermissionId, insertDetailDate);

            bool myResult = create[0]["result"];
            var myMessage = create[0]["message"];

            if (myResult)
            {
                string urlFile = "";
                if (value.FileName != null && value.FileName.Length > 0)
                {
                    IFormFile file = value.FileName;
                    var fileName = Path.GetFileName(file.FileName);
                    fileName = fileName.Replace(" ", "_");
                    urlFile = $"{Environment.NewLine}Link Dokument : https://hrd.sedana.co.id/service/api/support/open/viewfile/{fileName}";
                }
                var department = EmployeeFactory.GetEmployeeDepartment(employeeId: value.EmployeeId) ?? throw new Exception($"Invalid Data, trace {JsonConvert.SerializeObject(value.EmployeeId)}");
                var permissionTypeName = PermissionDB.GetPermissionType(id: value.PermissionTypeId) ?? throw new Exception("Invalid data, " + value.PermissionTypeId);
               /* await NotificationFactory.Notify(
                   action: NotificationFactory.NotificationSenderAction.Management,
                   title: $"{department!.EmployeeName} mengajukan izin {permissionTypeName}",
                   body: "Buka sistem HRIS untuk melihat detailnya",
                   employeeId: value.EmployeeId
                   );


                await NotificationFactory.NotifyWA(
                   action: NotificationFactory.NotificationSenderAction.Management,
                   title: $"*{department!.EmployeeName}* Mengajukan izin {permissionTypeName} karena keperluan *{value.Reason}*",
                   body: $"ID Pengajuan : {requestId}{Environment.NewLine}Tanggal Diajukan : {value.DateString}{urlFile}{Environment.NewLine}{Environment.NewLine}_segera lakukan approval pada sistem atau swipe pesan ini dengan perintah *!approve alasan*, atau *!reject alasan*_",
                   employeeId: value.EmployeeId
                );*/

                /*  await NotificationFactory.NotifyWA(
                     action: NotificationFactory.NotificationSenderAction.Management,
                     title: $"{department!.EmployeeName} mengajukan izin {permissionTypeName}",
                     body: value.Reason,
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

        [HttpPost("{id}/approvalPermission")]
        public async Task<ActionResult> approvalLeave([FromBody] ApprovalPermissionModel value, string id)
        {
            //checking Data
            var checkData = permissionDB.CountPermissionData(value.PermissionId);
            var isApproved = permissionDB.CheckIsApprove(value.PermissionId);
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
            //

            /*Mengecek Periode Closing*/
            string dateString = "";
            string[] myArrDate;
            bool checkPeriodeClosed = false;
            DateTime permissionDate = new DateTime();

            List<TpPermissionDetail> permissions = permissionDB.ListPermissionDetail(value.PermissionId);

            foreach (var permission in permissions)
            {
                permissionDate = permission.PermissionDate;
                dateString = permissionDate.ToString("yyyyMM");

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

            //access of approvall
            var findSessionData = userDB.FindSessionDataUser(uId);
            var findRequestData = userDB.FindRequestData("tp_permission", "permission_id", value.PermissionId);
            Console.WriteLine("emplId = " + findRequestData[0].EmployeeId);
            var findSessionDataStaff = userDB.FindSessionDataByEmployeeId(findRequestData[0].EmployeeId);
            Console.WriteLine("usr = " + findSessionData[0].DepartmentId);
            Console.WriteLine("staff = " + findSessionDataStaff[0].DepartmentId);
            
            if (findSessionData[0].DepartmentId != "DP006") //jika depertment selain hrd masuk sini 
            {
                if (findSessionData[0].EmployeeId != "0808003") //bu yuli //jika yang approve bukan bu yuli masuk sini 
                {
                    if (EmployeeFactory.IsHeadDepartment(employeeId: findSessionDataStaff[0].EmployeeId)) //jika yang approve adalah head dept masuk sini 
                    {
                        if (findSessionDataStaff[0].EmployeeId != "0321080" && findSessionDataStaff[0].EmployeeId != "0808004") //bu KD /bu lia
                        {
                            var headDiv = EmployeeFactory.GetHeadDivision(divisionId: findSessionDataStaff[0].DivId);
                            if (findSessionData[0].EmployeeId != headDiv)
                            {
                                return BadRequest(new { message = "Anda tidak diizinkan untuk melakukan approval.", success = 0 });
                            }
                        }
                        else
                        {
                            if (findSessionData[0].EmployeeId != "0808003") //bu yuli
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
            //end access off approvall

            TpPermission approvalPermission = new TpPermission()
            {
                PermissionId = value.PermissionId,
                ApproveBy = approvedBy,
                ApproveDate = DateTime.Now,
                Noted = value.Noted,
                Status = value.Status,
                IsNotify = 5,
                UpdatedAt = DateTime.Now,
                UpdatedBy = uId
            };

            List<Dictionary<string, dynamic>> approve = permissionDB.ApprovePermission(approvalPermission, id);

            bool myResult = approve[0]["result"];
            var myMessage = approve[0]["message"];

            DateTime permissionDateRequest = permissionDB.FindPermissionDate(value.PermissionId);
            string permissionDateRequestView = permissionDate.ToString("dd MMM yyyy");
            if (myResult)
            {
                var permissionTypeId = PermissionDB.GetPermissionTypeId(permissionId: value.PermissionId) ?? throw new Exception("Invalid data, " + value.PermissionId);
                var permissionTypeName = PermissionDB.GetPermissionType(id: permissionTypeId) ?? throw new Exception("Invalid data, " + permissionTypeId);
                /*await NotificationFactory.Notify(
                    action: NotificationFactory.NotificationSenderAction.Employee,
                    title: $"{EmployeeFactory.EmployeeName(approvedBy)} {(value.Status == 1 ? "menyetujui" : "menolak")} pengajuan izin {permissionTypeName}",
                    body: "Buka sistem HRIS untuk melihat detailnya",
                    employeeId: PermissionDB.GetEmployee(permissionId: value.PermissionId) ?? throw new Exception("Invalid data, " + value.PermissionId)
                    );

                await NotificationFactory.NotifyWA(
                  action: NotificationFactory.NotificationSenderAction.Employee,
                  title: $"*{EmployeeFactory.EmployeeName(approvedBy)} {(value.Status == 1 ? "Menyetujui" : "Menolak")} Pengajuan Izin {permissionTypeName}*",
                  body: $"dengan id pengajuan {value.PermissionId}, dan tanggal pengajuan {permissionDateRequestView}{Environment.NewLine}_Note : {value.Noted}_",
                  employeeId: PermissionDB.GetEmployee(permissionId: value.PermissionId) ?? throw new Exception("Invalid data, " + value.PermissionId)
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

        [HttpPost("{id}/deletePermission")]
        public ActionResult delete([FromBody] DeletePermissionModel value, string id = "USR-201710052")
        {
            /*Mengecek Periode Closing*/
            string dateString = "";
            string[] myArrDate;
            bool checkPeriodeClosed = false;
            DateTime permissionDate;

            List<TpPermissionDetail> permissions = permissionDB.ListPermissionDetail(value.PermissionId);

            foreach (var permission in permissions)
            {
                permissionDate = permission.PermissionDate;
                dateString = permissionDate.ToString("yyyyMM");

                checkPeriodeClosed = periodeDB.IsPeriodeAbsentClosed(dateString);
                if (checkPeriodeClosed == true)
                {
                    return BadRequest(new { message = "Periode Absent sudah ditutup.", success = 0 });
                }

            };

            TpPermission deletePermission = new TpPermission()
            {
                PermissionId = value.PermissionId,
                DeletedBy = value.DeletedBy,
                DeletedAt = DateTime.Now,
            };

            List<Dictionary<string, dynamic>> delete = permissionDB.DeletePermission(deletePermission, id);

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
