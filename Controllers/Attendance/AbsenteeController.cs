using BackendHrdAgro.Models.Attendance;
using BackendHrdAgro.Models.Master;
using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Mvc;
using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Database.MySql.Support;

namespace BackendHrdAgro.Controllers.Attendance
{
    [Route("api/absentee")]
    [ApiController]
    public class AbsenteeController : Controller
    {
        private readonly ILogger<AbsenteeController> _logger;
        private readonly IWebHostEnvironment _env;
        public AbsenteeController(ILogger<AbsenteeController> logger, IWebHostEnvironment env)
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
        AbsenteeModel absenteeModel = new AbsenteeModel();

        [HttpPost("{id}/absentee")]
        public IActionResult absentee(string id = "USR-201710052")
        {

            var clausaLogin = "";
            var clausaDept = "";
            var clausaPeriod = "";


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

                if (departmentId == "DP006" || employeeId == "0808003" || employeeId == "260224" || departmentId == "DP004")
                {  //untuk hrd
                    if (titleId == "DS006")
                    {
                        clausaLogin = $" and a.employee_id='{employeeId}'";
                        clausaDept = $" and b.department_id='{departmentId}'";
                    }
                    else
                    {
                        clausaLogin = " ";
                        clausaDept = " ";
                    }

                }
                else
                {
                    bool tittleExist = Array.Exists(arrTittleId, element => element == titleId);

                    if (tittleExist == true && levelId == "TL019")
                    {
                        clausaLogin = $" and b.div_id='{divId}'";
                        clausaDept = $" and b.div_id='{divId}'";
                    }
                    else if (tittleExist == true)
                    {
                        clausaLogin = $" and b.department_id='{departmentId}'";
                        clausaDept = $" and b.department_id='{departmentId}'";
                    }
                    else
                    {
                        clausaLogin = $" and a.employee_id='{employeeId}'";
                        clausaDept = $" and b.department_id='{departmentId}'";
                    }
                }
                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                detail.Add("listAbsentee", absenteeModel.attendanceRecordQueries(clausaLogin));
                detail.Add("departments", absenteeModel.departments(clausaDept));
                detail.Add("periods", absenteeModel.periodsQueries());

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

        [HttpPost("{id}/retrive")]
        public IActionResult retrive([FromBody] AbsenteeRetrive value, string id = "USR-201710052")
        {
            var clausaLogin = "";
            var clausaDept = "";
            var clausaPeriod = "";

            string[] arrTittleId = { "DS002", "DS003" };

            string paramPeriodeId = value.periodeId;
            string paramDepartmentId = value.departmentId;

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
                { //untuk hrd
                    if (titleId == "DS006") //staff
                    {
                        clausaLogin = $" and a.employee_id='{employeeId}'";
                        clausaDept = $" and b.department_id='{departmentId}'";
                    }
                    else
                    {
                        clausaLogin = " ";
                        clausaDept = " ";
                    }
                }
                else
                {
                    bool tittleExist = Array.Exists(arrTittleId, element => element == titleId);                    
                    if (tittleExist == true)
                    {
                        clausaLogin = $" and b.department_id='{departmentId}'";
                        clausaDept = $" and b.department_id='{departmentId}'";
                    }
                    else
                    {
                        clausaLogin = $" and a.employee_id='{employeeId}'";
                        clausaDept = $" and b.department_id='{departmentId}'";
                    }
                }

                var clausa = " ";

                if (paramDepartmentId == null || paramDepartmentId == "none" || paramDepartmentId == "-1")
                {
                    clausa += " ";
                }
                else
                {
                    clausa += $" and b.department_id='{paramDepartmentId}'";

                }

                clausa = clausa + clausaLogin;

                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                detail.Add("listAbsentee", absenteeModel.attendanceRecordQueries(clausa));
                detail.Add("departments", absenteeModel.departments(clausaDept));
                detail.Add("periods", absenteeModel.periodsQueries());

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

        [HttpPost("{id}/employee")]
        public IActionResult employee([FromBody] AbsenteeEmployeeModel value, string id = "USR-201710052")
        {
            string[] arrTittleId = { "DS002", "DS003" };

            string paramPeriodeId = value.periodeId;
            string paramEmployeeId = value.employeeId;
            string year = paramPeriodeId.Substring(0, 4);
            string month = paramPeriodeId.Substring(4, 2);
            string myPeriodeId2 = year + '-' + month;


            try
            {
                var findSessionData = userDB.FindSessionDataUser(id);
                string employeeId = findSessionData[0].EmployeeId;
                string departmentId = findSessionData[0].DepartmentId;
                string userId = findSessionData[0].UserId;
                string titleId = findSessionData[0].TitleId;
                string divId = findSessionData[0].DivId;
                string levelId = findSessionData[0].LevelId;


                var attendenceData = userDB.FindSessionDataByEmployeeId(value.employeeId);
                string attendenceFullName = attendenceData[0].fullName;
                string attendenceDepartmentName = attendenceData[0].DepartmentName;

                var clausa = " ";
                var clausb = "";

                if (paramPeriodeId == null || paramPeriodeId == "" || paramPeriodeId == "-1")
                {
                    clausa += " ";
                }
                else
                {
                    clausa += $" and DATE_FORMAT(b.absent_date,'%Y%m')='{paramPeriodeId}'";

                }

                if (paramEmployeeId == null || paramEmployeeId == "none" || paramEmployeeId == "-1")
                {
                    clausa += " ";
                    clausb += " ";
                }
                else
                {
                    clausa += $" and a.employee_id='{paramEmployeeId}'";
                    clausb += $" and a.employee_id='{paramEmployeeId}'";

                }

                List<AttendanceEmployeeQuery> employee = absenteeModel.AbsenteeEmployee(clausa, clausb, paramEmployeeId, paramPeriodeId, myPeriodeId2);

                float ok = 0;
                var myStart = "08:00";
                TimeSpan myStartDate = DateTime.Parse(myStart).TimeOfDay;
                TimeSpan myTimeInDate = DateTime.Parse(myStart).TimeOfDay;
                var cutOffAbsent = "09:00";
                var cutOffAbsent2 = "09:14";
                var lembur18 = "18:00";
                var lembur17 = "17:00";

                List<dynamic> myArrIn = new List<dynamic>();
                List<dynamic> myArrOut = new List<dynamic>();
                List<dynamic> myArrDate = new List<dynamic>();

                var i = 0;
                var myValIn = 0;
                var myValOut = 0;

                var _25 = "09:15";
                var _50 = "10:00";
                var _75 = "12:00";
                var _100 = "14:00";

                var proses = "On Proses";
                var terima = "Diterima";
                var tolak = "Ditolak";
                var clear = "clear";

                var myIn = "";
                var myOut = "";

                TimeSpan myCekStart = TimeSpan.Zero;
                TimeSpan varTemp = TimeSpan.Zero;

                string revisi = " ";

                float totHour = 0.00F;
                float myHour = 0.00F;
                float myHourEffective = 0.00F;
                float selisih = 0.00F;
                float overtime = 0.00F;
                string isOvertime = "N";

                float myCekcutOffAbsent = 0;
                float myCekcutOffAbsent2 = 0;

                string cutOffAbsentDes = " ";
                string statusPotongCuti = "N";
                int isCutCuti = 0;

                string linkModalCutOff = " ";
                foreach (var k in employee)
                {
                    revisi = $"{k.EmployeeId}|{k.AbsentDateDb}";

                    myTimeInDate = DateTime.Parse(k.TimeIn).TimeOfDay;
                    myCekStart = (myStartDate - myTimeInDate);
                    totHour = (float)myCekStart.TotalHours;

                    if (totHour > 0) { myIn = myStart; } else { myIn = k.TimeIn; } //jika ada, jam datang di inisialisasi ke jam 8 yang mana digunakan untuk menghitung overtime
                    myOut = k.TimeOut;

                    //Menghitung Jam Kerja
                    varTemp = (DateTime.Parse(k.TimeOut).TimeOfDay - DateTime.Parse(k.TimeIn).TimeOfDay);
                    myHour = (float)varTemp.TotalHours;

                    //Menghitung Jam Kerja Efektif
                    varTemp = (DateTime.Parse(myOut).TimeOfDay - DateTime.Parse(myIn).TimeOfDay);
                    myHourEffective = (float)varTemp.TotalHours;

                    ////////////menghitung lembur
                    /// menentukan kapan masuknya
                    varTemp = (DateTime.Parse(myIn).TimeOfDay - DateTime.Parse(myStart).TimeOfDay);
                    selisih = (float)varTemp.TotalHours;

                    if (selisih > 0)
                    {
                        varTemp = (DateTime.Parse(myOut).TimeOfDay - DateTime.Parse(lembur18).TimeOfDay);
                        overtime = (float)varTemp.TotalHours;
                    }
                    else
                    {
                        varTemp = (DateTime.Parse(myOut).TimeOfDay - DateTime.Parse(lembur17).TimeOfDay);
                        overtime = (float)varTemp.TotalHours;
                    }

                    if (overtime > 0)
                    {
                        isOvertime = "Y";
                    }
                    else
                    {
                        isOvertime = "N";
                    }



                    //////////////////////////POTONGAN CUTI
                    ///
                    varTemp = (DateTime.Parse(k.TimeIn).TimeOfDay - DateTime.Parse(cutOffAbsent).TimeOfDay);
                    myCekcutOffAbsent = (float)varTemp.TotalHours;
                    if (myCekcutOffAbsent > 0)
                    {
                        varTemp = (DateTime.Parse(k.TimeIn).TimeOfDay - DateTime.Parse(cutOffAbsent2).TimeOfDay);
                        myCekcutOffAbsent2 = (float)varTemp.TotalHours;

                        if (myCekcutOffAbsent2 > 0)
                        {
                            if (departmentId == "DP006")
                            { //untuk hrd
                                if (k.Status == 1)
                                {
                                    cutOffAbsentDes = "On Proses";
                                    linkModalCutOff = $" <a href = 'javascript:void(0)' class='approve-btn text-dark edit ms-2' data-bs-toggle='modal' data-bs-target='#modalFormTypeHrdRev' data-id='{revisi}' data-from='TypeHrd'><i class='far fa-check-square'></i></a>";
                                }
                                else if (k.Status == -1)
                                {
                                    cutOffAbsentDes = tolak;
                                }
                                else if (k.Status == 5)
                                {
                                    cutOffAbsentDes = terima;
                                    statusPotongCuti = "N";
                                    isCutCuti = 0;
                                    linkModalCutOff = "";
                                }
                                else if (k.Status == 9)
                                {
                                    cutOffAbsentDes = clear;
                                    statusPotongCuti = "N";
                                    isCutCuti = 0;
                                    linkModalCutOff = "";
                                }
                                else
                                {
                                    //DateTime.Parse(k.TimeIn).TimeOfDay
                                    if (DateTime.Parse(k.TimeIn).TimeOfDay > DateTime.Parse(_25).TimeOfDay && DateTime.Parse(k.TimeIn).TimeOfDay < DateTime.Parse(_50).TimeOfDay)
                                    {
                                        cutOffAbsentDes = "cut 0.25";

                                    }
                                    else if (DateTime.Parse(k.TimeIn).TimeOfDay > DateTime.Parse(_50).TimeOfDay && DateTime.Parse(k.TimeIn).TimeOfDay < DateTime.Parse(_75).TimeOfDay)
                                    {
                                        cutOffAbsentDes = "cut 0.50";
                                    }
                                    else if (DateTime.Parse(k.TimeIn).TimeOfDay > DateTime.Parse(_75).TimeOfDay && DateTime.Parse(k.TimeIn).TimeOfDay < DateTime.Parse(_100).TimeOfDay)
                                    {
                                        cutOffAbsentDes = "cut 0.75";
                                    }
                                    else
                                    {
                                        cutOffAbsentDes = "cut 1";
                                    }
                                    linkModalCutOff = $" <a href = 'javascript:void(0)' class='approve-btn text-dark edit ms-2' data-bs-toggle='modal' data-bs-target='#modalFormTypeHrdRev' data-id='{revisi}' data-from='TypeHrd'><i class='far fa-check-square'></i></a>";
                                   
                                }
                            }
                            else
                            { //untuk user

                                if (k.Status == 1)
                                {
                                    cutOffAbsentDes = proses;
                                    linkModalCutOff = $" <a href = 'javascript:void(0)' class='edit-btn text-dark edit ms-2' data-bs-toggle='modal' data-bs-target='#modalFormTypeUserRev' data-id='{revisi}' data-from='TypeUser'><i class='fas fa-pencil-alt'></i></a>";

                                }
                                else if (k.Status == -1)
                                {
                                    cutOffAbsentDes = tolak;
                                }
                                else if (k.Status == 5)
                                {
                                    cutOffAbsentDes = terima;
                                    statusPotongCuti = "N";
                                    isCutCuti = 0;
                                    linkModalCutOff = "";
                                }
                                else if (k.Status == 9)
                                {
                                    cutOffAbsentDes = clear;
                                    statusPotongCuti = "N";
                                    isCutCuti = 0;
                                    linkModalCutOff = "";
                                }
                                else
                                {

                                    if (DateTime.Parse(k.TimeIn).TimeOfDay > DateTime.Parse(_25).TimeOfDay && DateTime.Parse(k.TimeIn).TimeOfDay < DateTime.Parse(_50).TimeOfDay)
                                    {
                                        cutOffAbsentDes = "cut 0.25";
                                    }
                                    else if (DateTime.Parse(k.TimeIn).TimeOfDay > DateTime.Parse(_50).TimeOfDay && DateTime.Parse(k.TimeIn).TimeOfDay < DateTime.Parse(_75).TimeOfDay)
                                    {
                                        cutOffAbsentDes = "cut 0.50";
                                    }
                                    else if (DateTime.Parse(k.TimeIn).TimeOfDay > DateTime.Parse(_75).TimeOfDay && DateTime.Parse(k.TimeIn).TimeOfDay < DateTime.Parse(_100).TimeOfDay)
                                    {
                                        cutOffAbsentDes = "cut 7.25";
                                    }
                                    else
                                    {
                                        cutOffAbsentDes = "cut 1";
                                    }
                                    linkModalCutOff = $" <a href = 'javascript:void(0)' class='edit-btn text-dark edit ms-2' data-bs-toggle='modal' data-bs-target='#modalFormTypeUserRev' data-id='{revisi}' data-from='TypeUser'><i class='fas fa-pencil-alt'></i></a>";

                                }
                                
                            }
                            statusPotongCuti = "C";
                            isCutCuti = 1;

                            //disini
                        }
                        else
                        {
                            cutOffAbsentDes = "warning";
                            statusPotongCuti = "W";
                            isCutCuti = 1;

                        }
                    }
                    else
                    {
                        cutOffAbsentDes = "";
                        statusPotongCuti = "N";
                        isCutCuti = 0;
                    }


                    if (k.IsAbsent == 0)
                    {
                        if (departmentId == "DP006")
                        { //untuk hrd
                            if (k.Status == 1)
                            {
                                cutOffAbsentDes = "On Proses";
                                linkModalCutOff = $" <a href = 'javascript:void(0)' class='approve-btn text-dark edit ms-2' data-bs-toggle='modal' data-bs-target='#modalFormTypeHrdRev' data-id='{revisi}' data-from='TypeHrd'><i class='far fa-check-square'></i></a>";

                            }
                            else if (k.Status == -1)
                            {
                                cutOffAbsentDes = tolak;
                            }
                            else if (k.Status == 5)
                            {
                                cutOffAbsentDes = terima;
                                statusPotongCuti = "N";
                                isCutCuti = 0;
                                linkModalCutOff = "";
                            }
                            else if (k.Status == 9)
                            {
                                cutOffAbsentDes = clear;
                                statusPotongCuti = "N";
                                isCutCuti = 0;
                                linkModalCutOff = "";
                            }
                            else
                            {

                                if (DateTime.Parse(k.TimeIn).TimeOfDay > DateTime.Parse(_25).TimeOfDay && DateTime.Parse(k.TimeIn).TimeOfDay < DateTime.Parse(_50).TimeOfDay)
                                {
                                    cutOffAbsentDes = "cut 0.25";
                                }
                                else if (DateTime.Parse(k.TimeIn).TimeOfDay > DateTime.Parse(_50).TimeOfDay && DateTime.Parse(k.TimeIn).TimeOfDay < DateTime.Parse(_75).TimeOfDay)
                                {
                                    cutOffAbsentDes = "cut 0.50";
                                }
                                else if (DateTime.Parse(k.TimeIn).TimeOfDay > DateTime.Parse(_75).TimeOfDay && DateTime.Parse(k.TimeIn).TimeOfDay < DateTime.Parse(_100).TimeOfDay)
                                {
                                    cutOffAbsentDes = "cut 0.75";
                                }
                                else
                                {
                                    cutOffAbsentDes = "cut 1";
                                }

                                linkModalCutOff = $" <a href = 'javascript:void(0)' class='approve-btn text-dark edit ms-2' data-bs-toggle='modal' data-bs-target='#modalFormTypeHrdRev' data-id='{revisi}' data-from='TypeHrd'><i class='far fa-check-square'></i></a>";                              
                            }
                        }
                        else
                        {
                            cutOffAbsentDes = "cut 1";
                            linkModalCutOff = $" <a href = 'javascript:void(0)' class='edit-btn text-dark edit ms-2' data-bs-toggle='modal' data-bs-target='#modalFormTypeUserRev' data-id='{revisi}' data-from='TypeUser'><i class='fas fa-pencil-alt'></i></a>";

                            // cek ulang href="#modalFormTypeUserRev" data-target="#modalFormTypeHrd"  ???


                            if (k.Status == 1)
                            {
                                cutOffAbsentDes = proses;
                            }
                            else if (k.Status == -1)
                            {
                                cutOffAbsentDes = tolak;
                            }
                            else if (k.Status == 5)
                            {
                                linkModalCutOff = "";
                                cutOffAbsentDes = terima;
                                statusPotongCuti = "N";
                                isCutCuti = 0;
                            }

                        }

                        if (k.Status == 9)
                        {
                            cutOffAbsentDes = clear;
                            statusPotongCuti = "N";
                            isCutCuti = 0;
                        }

                        statusPotongCuti = "C";
                        isCutCuti = 1;
                    }


                    //////////inisialisasi data ke dalam variabel
                    k.MyHour = myHour.ToString("F2");
                    k.MyHourEfektif = myHourEffective.ToString("F2");
                    k.IsOvertime = isOvertime;
                    k.Overtime = overtime.ToString("F2");
                    k.StatusPotongCuti = statusPotongCuti;
                    k.CutOffAbsentDes = cutOffAbsentDes;
                    k.LinkModalCutOff = linkModalCutOff;
                }

                var findIsOb = absenteeModel.FindIsOB(paramEmployeeId);
                int isOB = findIsOb[0].isOB;

                List<UnAbsenteeEmployeeQuery> unAbsent = absenteeModel.UnAbsenteeEmployee(clausa, isOB, paramEmployeeId, year, month, myPeriodeId2);

                List<ComeLateQuery> comeLate = absenteeModel.ComeLateEmployee(isOB, "none", paramEmployeeId, year, month);
                List<ComeLateQuery> incompleteeAttendence = absenteeModel.IncompleteAttendance(isOB, "none", paramEmployeeId, year, month);

                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                detail.Add("AbsenteeEmployees", employee);
                detail.Add("UnAbsent", unAbsent);
                detail.Add("ComeLate", comeLate);
                detail.Add("IncompleteeAttendence", incompleteeAttendence);
                detail.Add("EmployeeName", attendenceFullName);
                detail.Add("attendenceDepartmentName", attendenceDepartmentName);

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

        [HttpPost("{id}/employee/unAbsentDept")]
        public IActionResult unAbsentDept([FromBody] UnAbsenteeDeptModel value, string id = "USR-201710052")
        {
            string[] arrTittleId = { "DS002", "DS003" };
            string paramPeriodeId = value.periodeId;
            string paramDepartmentId = value.departmentId;
            string year = paramPeriodeId.Substring(0, 4);
            string month = paramPeriodeId.Substring(4, 2);
            string myPeriodeId2 = year + '-' + month;

            try
            {
                var findSessionData = userDB.FindSessionDataUser(id);
                string employeeId = findSessionData[0].EmployeeId;
                string departmentId = findSessionData[0].DepartmentId;
                string userId = findSessionData[0].UserId;
                string titleId = findSessionData[0].TitleId;
                string divId = findSessionData[0].DivId;
                string levelId = findSessionData[0].LevelId;

                int isOB = 1;

                if (paramDepartmentId == null || paramDepartmentId == "all" || paramDepartmentId == "none")
                {
                    if (departmentId == "DP006" || employeeId == "0808003" || employeeId == "0000000")//depart HRD dan employee terpilih
                    {
                        if (titleId == "DS006")//staff
                        {

                            paramDepartmentId = departmentId;
                        }
                    }
                    else
                    {
                        paramDepartmentId = departmentId;
                    }
                }
                else
                {
                    paramDepartmentId = paramDepartmentId;
                }

                List<UnAbsenteeDepartmentQuery> unAbsent = absenteeModel.UnAbsenteeDepartment(paramDepartmentId, year, month, myPeriodeId2);

                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);


                detail.Add("unAbsentDept", unAbsent);

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

        [HttpPost("{id}/employee/comeLateDept")]
        public IActionResult ComeLateDept([FromBody] UnAbsenteeDeptModel value, string id = "USR-201710052")
        {
            string[] arrTittleId = { "DS002", "DS003" };

            string paramPeriodeId = value.periodeId;
            string paramDepartmentId = value.departmentId;
            string year = paramPeriodeId.Substring(0, 4);
            string month = paramPeriodeId.Substring(4, 2);
            string myPeriodeId2 = year + '-' + month;

            try
            {
                var findSessionData = userDB.FindSessionDataUser(id);
                string employeeId = findSessionData[0].EmployeeId;
                string departmentId = findSessionData[0].DepartmentId;
                string userId = findSessionData[0].UserId;
                string titleId = findSessionData[0].TitleId;
                string divId = findSessionData[0].DivId;
                string levelId = findSessionData[0].LevelId;


                int isOB = 0;

                if (paramDepartmentId == null || paramDepartmentId == "all" || paramDepartmentId == "none")
                {
                    if (departmentId == "DP006" || employeeId == "0808003" || employeeId == "0000000")//depart HRD dan employee terpilih
                    {
                        if (titleId == "DS006")//staff
                        {

                            paramDepartmentId = departmentId;
                        }
                    }
                    else
                    {
                        paramDepartmentId = departmentId;
                    }
                }
                else
                {
                    paramDepartmentId = paramDepartmentId;
                }

                List<ComeLateQuery> comeLate = absenteeModel.ComeLateEmployee(isOB, paramDepartmentId, "none", year, month);
                List<ComeLateSumQuery> comeLateSum = absenteeModel.ComeLateEmployeeSum(isOB, paramDepartmentId, "none", year, month);

                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);


                detail.Add("comeLate", comeLate);
                detail.Add("comeLateSum", comeLateSum);

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

        [HttpPost("{id}/employee/incompleteAttendance")]
        public IActionResult IncompleteAttendance([FromBody] IncompleteAttendanceModel value, string id = "USR-201710052")
        {
            string[] arrTittleId = { "DS002", "DS003" };

            string paramPeriodeId = value.periodeId;
            string paramDepartmentId = value.departmentId;
            string year = paramPeriodeId.Substring(0, 4);
            string month = paramPeriodeId.Substring(4, 2);
            string myPeriodeId2 = year + '-' + month;

            try
            {
                var findSessionData = userDB.FindSessionDataUser(id);
                string employeeId = findSessionData[0].EmployeeId;
                string departmentId = findSessionData[0].DepartmentId;
                string userId = findSessionData[0].UserId;
                string titleId = findSessionData[0].TitleId;
                string divId = findSessionData[0].DivId;
                string levelId = findSessionData[0].LevelId;


                int isOB = 0;

                if (paramDepartmentId == null || paramDepartmentId == "all" || paramDepartmentId == "none")
                {
                    if (departmentId == "DP006" || employeeId == "0808003" || employeeId == "0000000")//depart HRD dan employee terpilih
                    {
                        if (titleId == "DS006")//staff
                        {

                            paramDepartmentId = departmentId;
                        }
                    }
                    else
                    {
                        paramDepartmentId = departmentId;
                    }
                }
                else
                {
                    paramDepartmentId = paramDepartmentId;
                }

                List<ComeLateQuery> comeLate = absenteeModel.IncompleteAttendance(isOB, paramDepartmentId, "none", year, month);
                List<ComeLateSumQuery> comeLateSum = absenteeModel.IncompleteAttendanceSum(isOB, paramDepartmentId, "none", year, month);

                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                detail.Add("comeLate", comeLate);
                detail.Add("comeLateSum", comeLateSum);

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

        [HttpPost("{id}/createReqPermitt")]
        public IActionResult createReqPermitt([FromForm] RequestPermitt request, string id = "USR-201710052")
        {
            if (string.IsNullOrEmpty(request.EmployeeId))
            {
                return BadRequest(new { msg = "Employee id harus diisi.", success = 0 });
            }

            if (string.IsNullOrEmpty(request.Reason))
            {
                return BadRequest(new { msg = "Reason harus diisi.", success = 0 });
            }

            if (request.Category == "-1")
            {
                return BadRequest(new { msg = "Category harus dipilih.", success = 0 });
            }
            if (request.Category == "-1")
            {
                return BadRequest(new { msg = "Category harus dipilih.", success = 0 });
            }
            if (request.TypeRevisi == "")
            {
                return BadRequest(new { msg = "Type harus dipilih.", success = 0 });
            }


            TpAbsentRevisi createReqPermitt = new TpAbsentRevisi()
            {

                EmployeeId = request.EmployeeId,
                AbsentDate = request.AbsentDate,
                Reason = request.Reason,
                Category = request.Category,
                TypeRevisi = request.TypeRevisi,
                Status = request.Status,
                RequestBy = id,
                CreatedBy = id,
                CreatedAt = DateTime.Now,
            };

            if (request.FileName != null && request.FileName.Length > 0)
            {
                Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.Permitt, isProduction: _env.IsProduction()));
                string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.Permitt, isProduction: _env.IsProduction()), request.EmployeeId);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                IFormFile file = request.FileName;
                var fileName = Path.GetFileName(file.FileName);
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

                createReqPermitt.FileName = fileName;
                createReqPermitt.Type = request.FileName.ContentType;
                createReqPermitt.Size = 0;

            }

            List<Dictionary<string, dynamic>> create = absenteeModel.CreateReqPermitt(createReqPermitt);

            bool myResult = create[0]["result"];
            var myMessage = create[0]["message"];

            if (myResult)
            {
                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                data.Add("response", listResp);
                data.Add("message", myMessage);
                return Ok(data);
            }
            else
            {
                resp.Add("code", errorCodes.BadRequest);
                resp.Add("message", errorMessege.BadRequest);
                listResp.Add(resp);

                data.Add("response", listResp);
                data.Add("message", myMessage);
                return BadRequest(data);

            }
        }

        [HttpPost("{id}/appPermitt")]
        public ActionResult appPermitt([FromBody] ApprovePermitt value, string id = "USR-201710052")
        {

            TpAbsentRevisi appPermitt = new TpAbsentRevisi()
            {

                EmployeeId = value.EmployeeId,
                AbsentDate = value.AbsentDate,
                Status = value.Status,
                ApproveReason = value.ApproveReason,
                ApproveBy = id,
                ApproveDate = DateTime.Now,

            };

            List<Dictionary<string, dynamic>> create = absenteeModel.AppPermitt(appPermitt);

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
