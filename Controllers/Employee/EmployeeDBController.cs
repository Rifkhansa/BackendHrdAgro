using BackendHrdAgro.Models.Master;
using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Mvc;
using BackendHrdAgro.Models.Employee;
using BackendHrdAgro.Models.Database.MySql;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.Linq;

namespace BackendHrdAgro.Controllers.Employee
{
    [Route("api/employeedb")]
    [ApiController]
    public class EmployeeDBController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<EmployeeDBController> _logger;
        public EmployeeDBController(IWebHostEnvironment env, ILogger<EmployeeDBController> logger)
        {
            _env = env;
            _logger = logger;
        }

        readonly BaseModel BaseModel = new BaseModel();
        readonly ErrorCodes ErrorCode = new ErrorCodes();
        UserDB userDB = new UserDB();
        readonly ErrorMessege ErrorMessege = new ErrorMessege();
        Dictionary<string, dynamic> Data = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> Responses = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> Detail = new Dictionary<string, dynamic>();
        List<dynamic> ListResponse = new List<dynamic>();
        List<dynamic> ListData = new List<dynamic>();
        DatabaseContext databaseContext = new DatabaseContext();
        TmUser tmUser = new TmUser();
        TmEmployeeAffair tmEmployeeAffair = new TmEmployeeAffair();

        EmployeeDbModel employeeDbModel = new EmployeeDbModel();

        [HttpPost("{id}")]
        public IActionResult Index(string id = "USR-201709001")
        {
            try
            {
                var kriteria = "";
                string[] arrayTitle = { "DS002", "DS003" };
                string[] arrayDept =
                {
                    "DP003", //HRD
                    "DP004" //BOD
                };

                var findSessionData = userDB.FindSessionDataUser(id);
                string employeeId = findSessionData[0].EmployeeId;
                string departmentId = findSessionData[0].DepartmentId;
                string titleId = findSessionData[0].TitleId;
                string divId = findSessionData[0].DivId;
                string levelId = findSessionData[0].LevelId;

                bool departmentExist = Array.Exists(arrayDept, element => element == departmentId); 

                if (departmentExist == true || employeeId == "0808003") // untuk hrd
                {
                    if (titleId == "DS006")
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

                    if (tittleExist == true && levelId == "TL019")
                    {
                        kriteria = $"and b.div_id='{divId}'";
                    }
                    else if (tittleExist == true)
                    {
                        kriteria = $"and b.department_id = '{departmentId}'";
                    }
                    else
                    {
                        kriteria = $"and a.employee_id = '{employeeId}'";
                    }
                }

                var group = employeeDbModel.employeeGroupQueries(kriteria);
                Console.WriteLine("group" + group);
                var department = employeeDbModel.departments();
                var type = employeeDbModel.types();
                var religi = employeeDbModel.religions();
                var married = employeeDbModel.marrieds();
                var gender = employeeDbModel.genders();
                var title = employeeDbModel.titles();
                var level = employeeDbModel.levels();
                var employeeStatus = employeeDbModel.employeeStatuses();
                var bankStatus = employeeDbModel.bankStatuses();
                var statusTermination = employeeDbModel.reasonStatuses();
                var bloodTypes = employeeDbModel.bloodTypes();

                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("group", group);
                Detail.Add("department", department);
                Detail.Add("type", type);
                Detail.Add("religi", religi);
                Detail.Add("married", married);
                Detail.Add("gender", gender);
                Detail.Add("title", title);
                Detail.Add("level", level);
                Detail.Add("employeeStatus", employeeStatus);
                Detail.Add("bankStatus", bankStatus);
                Detail.Add("bloodTypes", bloodTypes);
                Detail.Add("statusTermination", statusTermination);

                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("message", "");
                Data.Add("data", ListData);

                return Ok(Data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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

        [HttpPost("{id}/getOne")]
        public IActionResult GetOne([FromBody] getModel value)
        {
            var listFamily = employeeDbModel.listFamilies(value);
            var relationship = employeeDbModel.relationships();
            var myresult = employeeDbModel.employeeDepartments(value);

            Responses.Add("code", ErrorCode.Ok);
            Responses.Add("message", ErrorMessege.Ok);
            ListResponse.Add(Responses);

            Detail.Add("listFamily", listFamily);
            Detail.Add("relationship", relationship);
            Detail.Add("employeeDepartment", myresult);

            ListData.Add(Detail);

            Data.Add("response", ListResponse);
            Data.Add("data", ListData);
            Data.Add("message", "Sukses getOne");
            return Ok(Data);
        }

        [HttpPost("{id}/getTwo")]
        public IActionResult GetTwo([FromBody] getModel value)
        {
            var listStudi = employeeDbModel.listStudis(value);
            var studi = employeeDbModel.studyIds();
            var insuranceType = employeeDbModel.insuranceTypes();
            var myresult = employeeDbModel.employeeDepartments(value);

            Responses.Add("code", ErrorCode.Ok);
            Responses.Add("message", ErrorMessege.Ok);
            ListResponse.Add(Responses);

            Detail.Add("listStudi", listStudi);
            Detail.Add("studi", studi);
            Detail.Add("insuranceType", insuranceType);
            Detail.Add("employeeDepartment", myresult);

            ListData.Add(Detail);

            Data.Add("response", ListResponse);
            Data.Add("data", ListData);
            Data.Add("message", "Sukses getTwo");
            return Ok(Data);
        }

        [HttpPost("{id}/getThree")]
        public IActionResult GetThree([FromBody] getModel value)
        {
            var listLicense = employeeDbModel.listLicenses(value);
            var license = employeeDbModel.licenses();
            var insuranceType = employeeDbModel.insuranceTypes();
            var myresult = employeeDbModel.employeeDepartments(value);

            Responses.Add("code", ErrorCode.Ok);
            Responses.Add("message", ErrorMessege.Ok);
            ListResponse.Add(Responses);

            Detail.Add("listLicense", listLicense);
            Detail.Add("license", license);
            Detail.Add("insuranceType", insuranceType);
            Detail.Add("employeeDepartment", myresult);

            ListData.Add(Detail);

            Data.Add("response", ListResponse);
            Data.Add("data", ListData);
            Data.Add("message", "Sukses getThree");
            return Ok(Data);
        }

        [HttpPost("{id}/getFour")]
        public IActionResult GetFour([FromBody] getModel value)
        {
            var listRelation = employeeDbModel.listRelations(value);
            var relation = employeeDbModel.relationships();
            var myresult = employeeDbModel.employeeDepartments(value);

            Responses.Add("code", ErrorCode.Ok);
            Responses.Add("message", ErrorMessege.Ok);
            ListResponse.Add(Responses);

            Detail.Add("listRelation", listRelation);
            Detail.Add("relation", relation);
            Detail.Add("employeeDepartment", myresult);

            ListData.Add(Detail);

            Data.Add("response", ListResponse);
            Data.Add("data", ListData);
            Data.Add("message", "Sukses getThree");
            return Ok(Data);
        }

        [HttpPost("{id}/viewdata")]
        public IActionResult ViewData([FromBody] getModel value)
        {
            var myFullName = string.Empty;
            var Department = string.Empty;
            var Title = string.Empty;
            var level = string.Empty;
            var EmpStatus = string.Empty;
            var Joint = string.Empty;
            var End = string.Empty;
            var Permanent = string.Empty;
            var TempatLahir = string.Empty;
            var TanggalLahir = string.Empty;
            var Alamat = string.Empty;
            var Alamattinggal = string.Empty;
            var Religi = string.Empty;
            var Married = string.Empty;
            var Bank = string.Empty;
            var Gender = string.Empty;
            var Rekening = string.Empty;
            var FileNameFoto = string.Empty;

            var myResult = employeeDbModel.viewDatas(value);
            foreach (var item in myResult)
            {
                myFullName = item.EmployeeFirstName + " " + item.EmployeeLastName;
                Department = item.DepartmentName;
                Title = item.TitleName;
                level = item.LevelName;
                EmpStatus = item.EmployeeStatusName;
                Joint = item.JointDate.ToString("yyyy MM dd");
                End = item.EndOfContract.ToString("yyyy MM dd");
                Permanent = item.PermanentDate?.ToString("yyyy MM dd");
                TempatLahir = item.TempatLahir;
                TanggalLahir = item.TanggalLahir.ToString("yyyy/MM/dd");
                Alamat = item.Alamat;
                Alamattinggal = item.AlamatTinggal;
                Religi = item.ReligiName;
                Married = item.MarriedName;
                Bank = item.BankName;
                Gender = item.GenderName;
                Rekening = item.NoRekening;
                FileNameFoto = item.FileName;

                /* return Ok(myResult);*/
            }

            var familys = employeeDbModel.listFamilies(value);
            var studis = employeeDbModel.listStudis(value);
            var licenses = employeeDbModel.viewLicenses(value);
            var relations = employeeDbModel.viewRelations(value);
            var document = employeeDbModel.documents(value);

            Responses.Add("code", ErrorCode.Ok);
            Responses.Add("message", ErrorMessege.Ok);
            ListResponse.Add(Responses);

            Detail.Add("myFullName", myFullName);
            Detail.Add("Department", Department);
            Detail.Add("Title", Title);
            Detail.Add("Level", level);
            Detail.Add("EmpStatus", EmpStatus);
            Detail.Add("Joint", Joint);
            Detail.Add("End", End);
            Detail.Add("Permanent", Permanent);
            Detail.Add("TempatLahir", TempatLahir);
            Detail.Add("TanggalLahir", TanggalLahir);
            Detail.Add("Alamat", Alamat);
            Detail.Add("Alamattinggal", Alamattinggal);
            Detail.Add("Religi", Religi);
            Detail.Add("Married", Married);
            Detail.Add("Bank", Bank);
            Detail.Add("Gender", Gender);
            Detail.Add("Rekening", Rekening);
            Detail.Add("FileNameFoto", FileNameFoto);

            Detail.Add("familys", familys);
            Detail.Add("studis", studis);
            Detail.Add("licenses", licenses);
            Detail.Add("relations", relations);
            Detail.Add("document", document);

            ListData.Add(Detail);

            Data.Add("response", ListResponse);
            Data.Add("data", ListData);
            Data.Add("message", "");
            return Ok(Data);
        }

        [HttpPost("{id}/create")]
        public ActionResult Create([FromBody] CreateModel value)
        {
            try
            {
                var lastEmployeeId = databaseContext.TmEmployeeAffairs.Where(x => !x.EmployeeId.Contains("admin")).OrderByDescending(x => x.EmployeeId.Substring(x.EmployeeId.Length - 3)).Select(x => x.EmployeeId).FirstOrDefault() ?? "0000000";
                Console.WriteLine($"Pegawai terakhir {lastEmployeeId}");
                //Generate employeeId untuk karyawan baru
                var newEmployeeId = $"{DateTime.Now:MMyy}{Convert.ToInt32(lastEmployeeId.Substring(lastEmployeeId.Length - 3)) + 1:D3}";
                Console.WriteLine($"Pegawai NEW {newEmployeeId}");

                tmEmployeeAffair.EmployeeId = newEmployeeId;
                tmEmployeeAffair.EmployeeFirstName = value.EmployeeFirstName;
                tmEmployeeAffair.EmployeeLastName = value.EmployeeLastName == null ? "" : value.EmployeeLastName;
                tmEmployeeAffair.DepartmentId = value.DepartmentId;
                tmEmployeeAffair.TitleId = value.TitleId;
                tmEmployeeAffair.LevelId = value.LevelId;
                tmEmployeeAffair.EmployeeStatusId = value.EmployeeStatusId;
                tmEmployeeAffair.JointDate = value.JointDate;
                tmEmployeeAffair.EndOfContract = value.EndOfContract;
                tmEmployeeAffair.PermanentDate = value.PermanentDate;
                tmEmployeeAffair.TempatLahir = value.TempatLahir;
                tmEmployeeAffair.TanggalLahir = value.TanggalLahir;
                tmEmployeeAffair.Alamat = value.Alamat;
                tmEmployeeAffair.AlamatTinggal = value.AlamatTinggal;
                tmEmployeeAffair.ReligiId = value.ReligiId;
                tmEmployeeAffair.MarriedId = value.MarriedId;
                tmEmployeeAffair.GenderId = value.GenderId;
                tmEmployeeAffair.BankId = value.BankId == null ? "" : value.BankId;
                tmEmployeeAffair.NoRekening = value.NoRekening == null ? "" : value.NoRekening;
                tmEmployeeAffair.Email = value.Email;
                tmEmployeeAffair.Status = 1;

                tmEmployeeAffair.DtEtr = DateTime.Now;
                tmEmployeeAffair.UserEtr = value.UserId;
                tmEmployeeAffair.DtUpdate = DateTime.Now;
                tmEmployeeAffair.UserUpdate = value.UserId;

                databaseContext.TmEmployeeAffairs.Add(tmEmployeeAffair);
                databaseContext.SaveChanges();

                var eid = tmEmployeeAffair.EmployeeId;
                var uid = BaseModel
                    .GenerateId(
                    tableName: "tm_users",
                    primaryKey: "user_id",
                    str: "USR-" + DateTime.Now.ToString("yyMM"),
                    trailing: 3,
                    lastKey: "NONE");

                //Create Tm_User
                tmUser.UserId = uid;
                tmUser.Remark = uid;
                tmUser.UserName = value.EmployeeFirstName;
                tmUser.UserFullName = value.EmployeeFirstName + " " + value.EmployeeLastName;
                tmUser.EmployeeId = eid;
                tmUser.Password = BaseModel.HashedPassword(eid);
                tmUser.Email = value.Email == null ? "-" : value.Email;
                tmUser.GroupId = "UG003";
                tmUser.AllEmployeeAllowed = 0;
                tmUser.IsAllowedBjb = 0;
                tmUser.Status = 1;
                tmUser.LastLogin = DateTime.Now;
                tmUser.LastChangePassword = DateTime.Now;
                tmUser.DtEtr = DateTime.Now;
                tmUser.UserEtr = value.UserId;
                tmUser.DtUpdate = DateTime.Now;
                tmUser.UserUpdate = value.UserId;

                databaseContext.TmUsers.Add(tmUser);
                databaseContext.SaveChanges();

                //Create tm_sisa_cuti
                var years = DateTime.Now.ToString("yyyy");
                TmSisaCuti tmSisaCuti = new TmSisaCuti();
                var scid = BaseModel
                    .GenerateId(
                    tableName: "tm_sisa_cuti",
                    primaryKey: "inisial_cuti_id",
                    str: "ICD",
                    trailing: 8,
                    lastKey: "NONE");

                tmSisaCuti.InisialCutiId = scid;
                tmSisaCuti.EmployeeId = eid;
                tmSisaCuti.Years = years;
                tmSisaCuti.SisaCutiAnnual = 0;
                tmSisaCuti.SisaCutiMaternity = 0;
                tmSisaCuti.SisaCutiLong = 0;
                tmSisaCuti.StatusCuti = 1;

                databaseContext.TmSisaCutis.Add(tmSisaCuti);
                databaseContext.SaveChanges();

                //Create tp_employee_affair_document
                //Geneate Id buat tp_employee_affair_document
                TpEmployeeAffairDocument tpEmployeeAffairDocument = new TpEmployeeAffairDocument();
                var id = BaseModel
                    .GenerateId(
                    tableName: "tp_employee_affair_document",
                    primaryKey: "id",
                    str: "ID",
                    trailing: 5,
                    lastKey: "NONE");

                tpEmployeeAffairDocument.Id = id;
                tpEmployeeAffairDocument.EmployeeId = eid;
                tpEmployeeAffairDocument.ClaimTypeId = "CT000";
                tpEmployeeAffairDocument.DocumentId = "1";
                tpEmployeeAffairDocument.DocumentStatus = 0;
                tpEmployeeAffairDocument.FcDocument = 0;
                tpEmployeeAffairDocument.FileName = "";
                tpEmployeeAffairDocument.Size = 0;
                tpEmployeeAffairDocument.Type = "";
                tpEmployeeAffairDocument.Submitted = DateTime.Now;
                tpEmployeeAffairDocument.SubmittedBy = value.UserId;
                tpEmployeeAffairDocument.IsDeleted = 0;

                databaseContext.TpEmployeeAffairDocuments.Add(tpEmployeeAffairDocument);
                databaseContext.SaveChanges();
                Console.WriteLine(id);

                //Karna keperluan nya masukin beberapa data sekaligus kedalam 1 table tp_employee_affair_document
                //Maka cara paling singkat, langsung dibuat di list berdasarkan model yang akan dimasukan ke dalam db

                //Ini LIST
                var idDocument1 = BaseModel
                    .GenerateId(
                    tableName: "tp_employee_affair_document",
                    primaryKey: "id",
                    str: "ID",
                    trailing: 5,
                    lastKey: "NONE");
                Console.WriteLine(idDocument1);

                //id yang keluar pasti ID01000 semua, karna id terkahir di db nya ID00999, walaupun secara logika kita seharusnya id nya ga sama
                //klo lastKey nya di isi "NONE" mau sebanyak apapun id yang digenerate untuk satu table yang sama id yang di dapat pasti sama

                // ini MODEL
                var emp = new List<TpEmployeeAffairDocument>
                        {
                            new TpEmployeeAffairDocument { Id = "", EmployeeId = eid, ClaimTypeId = "CT000", DocumentId = "1", DocumentStatus = 0, FcDocument = 0,
                                                           FileName = "", Size = 0, Type = "", Submitted = DateTime.Now, SubmittedBy = value.UserId, IsDeleted = 0},
                            new TpEmployeeAffairDocument { Id = "", EmployeeId = eid, ClaimTypeId = "CT000", DocumentId = "2", DocumentStatus = 0, FcDocument = 0,
                                                           FileName = "", Size = 0, Type = "", Submitted = DateTime.Now, SubmittedBy = value.UserId, IsDeleted = 0},
                            new TpEmployeeAffairDocument { Id = "", EmployeeId = eid, ClaimTypeId = "CT000", DocumentId = "3", DocumentStatus = 0, FcDocument = 0,
                                                           FileName = "", Size = 0, Type = "", Submitted = DateTime.Now, SubmittedBy = value.UserId, IsDeleted = 0},
                            new TpEmployeeAffairDocument { Id = "", EmployeeId = eid, ClaimTypeId = "CT000", DocumentId = "4", DocumentStatus = 0, FcDocument = 0,
                                                           FileName = "", Size = 0, Type = "", Submitted = DateTime.Now, SubmittedBy = value.UserId, IsDeleted = 0},
                            new TpEmployeeAffairDocument { Id = "", EmployeeId = eid, ClaimTypeId = "CT000", DocumentId = "5", DocumentStatus = 0, FcDocument = 0,
                                                           FileName = "", Size = 0, Type = "", Submitted = DateTime.Now, SubmittedBy = value.UserId, IsDeleted = 0},
                            new TpEmployeeAffairDocument { Id = "", EmployeeId = eid, ClaimTypeId = "CT000", DocumentId = "6", DocumentStatus = 0, FcDocument = 0,
                                                           FileName = "", Size = 0, Type = "", Submitted = DateTime.Now, SubmittedBy = value.UserId, IsDeleted = 0},
                            new TpEmployeeAffairDocument { Id = "", EmployeeId = eid, ClaimTypeId = "CT000", DocumentId = "7", DocumentStatus = 0, FcDocument = 0,
                                                           FileName = "", Size = 0, Type = "", Submitted = DateTime.Now, SubmittedBy = value.UserId, IsDeleted = 0},
                            new TpEmployeeAffairDocument { Id = "", EmployeeId = eid, ClaimTypeId = "CT000", DocumentId = "8", DocumentStatus = 0, FcDocument = 0,
                                                           FileName = "", Size = 0, Type = "", Submitted = DateTime.Now, SubmittedBy = value.UserId, IsDeleted = 0},
                            new TpEmployeeAffairDocument { Id = "", EmployeeId = eid, ClaimTypeId = "CT000", DocumentId = "9", DocumentStatus = 0, FcDocument = 0,
                                                           FileName = "", Size = 0, Type = "", Submitted = DateTime.Now, SubmittedBy = value.UserId, IsDeleted = 0},
                            new TpEmployeeAffairDocument { Id = "", EmployeeId = eid, ClaimTypeId = "CT000", DocumentId = "10", DocumentStatus = 0, FcDocument = 0,
                                                           FileName = "", Size = 0, Type = "", Submitted = DateTime.Now, SubmittedBy = value.UserId, IsDeleted = 0},
                            new TpEmployeeAffairDocument { Id = "", EmployeeId = eid, ClaimTypeId = "CT000", DocumentId = "11", DocumentStatus = 0, FcDocument = 0,
                                                           FileName = "", Size = 0, Type = "", Submitted = DateTime.Now, SubmittedBy = value.UserId, IsDeleted = 0},
                            new TpEmployeeAffairDocument { Id = "", EmployeeId = eid, ClaimTypeId = "CT000", DocumentId = "12", DocumentStatus = 0, FcDocument = 0,
                                                           FileName = "", Size = 0, Type = "", Submitted = DateTime.Now, SubmittedBy = value.UserId, IsDeleted = 0},
                            new TpEmployeeAffairDocument { Id = "", EmployeeId = eid, ClaimTypeId = "CT000", DocumentId = "13", DocumentStatus = 0, FcDocument = 0,
                                                           FileName = "", Size = 0, Type = "", Submitted = DateTime.Now, SubmittedBy = value.UserId, IsDeleted = 0},

                        };

                //misal list udah dibuat, model udah diisi dengan masing2 porsi datanya
                //step selanjutnya buat variable buat nampung id secara sementara

                var nextDocumentId = id; //disini nextDocumentId berisi id yang udah digenerate diatas
                Console.WriteLine("nextDocumentID awal/sebelum : " + nextDocumentId);

                // List nya udah ada, klo di hitung berarti ada 13 model didalam list
                //berarti klo id yang digenerate diatas itu ID01000 perlu id dari ID01000 sampe ID01013 kan supaya ga bentrok ketika dimasukan ke db
                //caranya LOOPING LIST tadi supaya bisa masukin id kedalam setiap model di list

                //LOOPING  LIST
                foreach (var documentEmployee in emp)
                {
                    nextDocumentId = BaseModel
                    .GenerateId(
                    tableName: "tp_employee_affair_document",
                    primaryKey: "id",
                    str: "ID",
                    trailing: 5,
                    lastKey: nextDocumentId);
                    documentEmployee.Id = nextDocumentId;
                    Console.WriteLine("nextDocumentID setelah digenerate : " + nextDocumentId);
                }

                databaseContext.TpEmployeeAffairDocuments.AddRange(emp);
                databaseContext.SaveChanges();

                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("message", "Data berhasil dibuat.");
                Data.Add("data", ListData);
                return Ok(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Responses.Add("code", ErrorCode.BadRequest);
                Responses.Add("message", ErrorMessege.BadRequest);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", e.Message);
                return BadRequest(Data);
            }
        }

        [HttpPost("{id}/create_getone")]
        public ActionResult CreateGetOne([FromBody] CreateGetOneModel value)
        {
            TpEmployeeFamily tpEmployeeFamily = new TpEmployeeFamily();

            var familyId = BaseModel
                .GenerateId(
                tableName: "tp_employee_family",
                primaryKey: "family_id",
                str: "FML",
                trailing: 3,
                lastKey: "NONE");
            Console.WriteLine(familyId);

            tpEmployeeFamily.FamilyId = familyId;
            tpEmployeeFamily.EmployeeId = value.EmployeeId;
            tpEmployeeFamily.HubunganId = value.HubunganId;
            tpEmployeeFamily.FamilyName = value.FamilyName;
            tpEmployeeFamily.TempatLahir = value.TempatLahir;
            tpEmployeeFamily.TanggalLahir = value.TanggalLahir;
            tpEmployeeFamily.NoTelp = value.NoTelp;
            tpEmployeeFamily.Status = 1;

            List<Dictionary<string, dynamic>> create = employeeDbModel.CreateGetOne(tpEmployeeFamily);

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
                Data.Add("message", "Data berhasil dibuat.");
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

        [HttpPost("{id}/create_gettwo")]
        public ActionResult CreateGetTwo([FromForm] CreateGetTwoModel value)
        {
            TpEmployeeStudi tpEmployeeStudi = new TpEmployeeStudi();
            var pendidikanId = BaseModel
                .GenerateId(
                tableName: "tp_employee_studi",
                primaryKey: "pendidikan_id",
                str: "SCH",
                trailing: 3,
                lastKey: "NONE");
            Console.WriteLine(pendidikanId);

            tpEmployeeStudi.PendidikanId = pendidikanId;
            tpEmployeeStudi.EmployeeId = value.EmployeeId;
            tpEmployeeStudi.TypeInsId = value.TypeInsId;
            tpEmployeeStudi.StudiId = value.StudiId;
            tpEmployeeStudi.SchoolName = value.SchoolName;
            tpEmployeeStudi.Periode = value.Periode;
            tpEmployeeStudi.Akhir = value.Akhir;
            tpEmployeeStudi.Status = value.Status;

            if (value.File != null && value.File.Length > 0)
            {
                Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));
                string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()), value.EmployeeId);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                IFormFile file = value.File;
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(directoryPath, fileName);

                // Save file
                using (var stream = new FileStream(path: filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    //fileName = filePath;
                }

                tpEmployeeStudi.FileName = fileName;
                tpEmployeeStudi.Type = value.File.ContentType;
                tpEmployeeStudi.Size = file.Length;
            }

            List<Dictionary<string, dynamic>> create = employeeDbModel.CreateGetTwo(tpEmployeeStudi);

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
                Data.Add("message", "Data berhasil dibuat.");
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

        [HttpPost("{id}/create_getthree")]
        public ActionResult CreateGetThree([FromForm] CreateGetThreeModel value)
        {
            TpEmployeeLicense tpEmployeeLicense = new TpEmployeeLicense();

            var licenseId = BaseModel
                .GenerateId(
                tableName: "tp_employee_license",
                primaryKey: "license_id",
                str: "LCS",
                trailing: 3,
                lastKey: "NONE");
            Console.WriteLine(licenseId);

            tpEmployeeLicense.LicenseId = licenseId;
            tpEmployeeLicense.EmployeeId = value.employeeId;
            tpEmployeeLicense.TypeInsId = value.typeInsId;
            tpEmployeeLicense.LicenseName = value.licenseName;
            tpEmployeeLicense.Periode = value.periode;
            tpEmployeeLicense.Status = value.status;

            if (value.file != null && value.file.Length > 0)
            {
                Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));
                string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()), value.employeeId);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                IFormFile file = value.file;
                string fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(directoryPath, fileName);

                // Save file
                using (var stream = new FileStream(path: filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    //fileName = filePath;
                }

                tpEmployeeLicense.FileName = fileName;
                tpEmployeeLicense.Type = value.file.ContentType;
                tpEmployeeLicense.Size = file.Length;
            }

            List<Dictionary<string, dynamic>> create = employeeDbModel.CreateGetThree(tpEmployeeLicense);

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
                Data.Add("message", "Data berhasil dibuat.");
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

        [HttpPost("{id}/create_getfour")]
        public ActionResult CreateGetFour([FromBody] CreateGetFourModel value)
        {
            TpEmployeeRelation tpEmployeeRelation = new TpEmployeeRelation();

            var relationId = BaseModel
                .GenerateId(
                tableName: "tp_employee_relation",
                primaryKey: "relation_id",
                str: "RLT",
                trailing: 3,
                lastKey: "NONE");
            Console.WriteLine(relationId);

            tpEmployeeRelation.RelationId = relationId;
            tpEmployeeRelation.EmployeeId = value.EmployeeId;
            tpEmployeeRelation.HubunganId = value.HubunganId;
            tpEmployeeRelation.RelationName = value.RelationName;
            tpEmployeeRelation.RelationAlamat = value.RelationAlamat;
            tpEmployeeRelation.NoTelp = value.NoTelp;
            tpEmployeeRelation.Status = 1;

            List<Dictionary<string, dynamic>> create = employeeDbModel.CreateGetFour(tpEmployeeRelation);

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
                Data.Add("message", "Data berhasil dibuat.");
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

        [HttpPost("{id}/getEdit")]
        public IActionResult getEdit([FromBody] getModel value)
        {
            try
            {
                var getEdit = employeeDbModel.getEdits(value).FirstOrDefault();
                Console.WriteLine(getEdit);

                if (getEdit != null)
                {
                    var myDataSplitEmp = getEdit.EmployeeId + "|" + getEdit.EmployeeFirstName + "|" + getEdit.EmployeeLastName + "|" +
                          getEdit.DepartmentId + "|" + getEdit.TitleId + "|" + getEdit.LevelId + "|" + getEdit.EmployeeStatusId + "|" +
                          getEdit.JointDate + "|" + getEdit.EndOfContract + "|" + getEdit.PermanentDate + "|" + getEdit.TempatLahir + "|" +
                          getEdit.TanggalLahir + "|" + getEdit.Alamat + "|" + getEdit.AlamatTinggal + "|" + getEdit.ReligiId + "|" +
                          getEdit.GenderId + "|" + getEdit.MarriedId + "|" + getEdit.Status;
                    Console.WriteLine(myDataSplitEmp);

                    var listDocument = employeeDbModel.listDocuments(value.EmployeeId);
                    Console.WriteLine(listDocument);

                    Responses.Add("code", ErrorCode.Ok);
                    Responses.Add("message", ErrorMessege.Ok);
                    ListResponse.Add(Responses);

                    Detail.Add("mySplitEmp", myDataSplitEmp);
                    Detail.Add("listDocument", listDocument);
                    ListData.Add(Detail);

                    Data.Add("response", ListResponse);
                    Data.Add("message", "Data berhasil");
                    Data.Add("data", ListData);
                    return Ok(Data);
                }
                else
                {
                    Responses.Add("code", ErrorCode.Ok);
                    Responses.Add("message", ErrorMessege.Ok);
                    ListResponse.Add(Responses);

                    Data.Add("response", ListResponse);
                    Data.Add("message", "Data Kosong");
                    Data.Add("data", ListData);
                    return Ok(Data);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, HttpContext.Request.Path);
                Responses.Add("code", ErrorCode.BadRequest);
                Responses.Add("message", ErrorMessege.BadRequest);
                ListResponse.Add(Responses);

                return BadRequest(Data);
            }
        }

        [HttpPost("{id}/update")]
        public IActionResult update([FromForm] updateEmployeeId value, string id)
        {
            try
            {
                tmEmployeeAffair.EmployeeFirstName = value.EmployeeFirstName;
                tmEmployeeAffair.EmployeeLastName = value.EmployeeLastName;
                tmEmployeeAffair.DepartmentId = value.DepartmentId;
                tmEmployeeAffair.TitleId = value.TitleId;
                tmEmployeeAffair.LevelId = value.LevelId;
                tmEmployeeAffair.EmployeeStatusId = value.EmployeeStatusId;
                tmEmployeeAffair.JointDate = value.JointDate;
                tmEmployeeAffair.EndOfContract = value.EndOfContract;
                tmEmployeeAffair.PermanentDate = value.PermanentDate;
                tmEmployeeAffair.TempatLahir = value.TempatLahir;
                tmEmployeeAffair.TanggalLahir = value.TanggalLahir;
                tmEmployeeAffair.Alamat = value.Alamat;
                tmEmployeeAffair.AlamatTinggal = value.AlamatTinggal;
                tmEmployeeAffair.ReligiId = value.ReligiId;
                tmEmployeeAffair.GenderId = value.GenderId;
                tmEmployeeAffair.MarriedId = value.MarriedId;
                tmEmployeeAffair.BankId = value.BankId == null ? " " : value.BankId;
                tmEmployeeAffair.NoRekening = value.NoRekening == null ? " " : value.NoRekening;
                tmEmployeeAffair.Email = value.Email == null ? " " : value.Email;

                tmEmployeeAffair.DtEtr = DateTime.Now;
                tmEmployeeAffair.UserEtr = id;
                tmEmployeeAffair.DtUpdate = DateTime.Now;
                tmEmployeeAffair.UserUpdate = id;

                var pegawaiHidFileName = "";
                var arrFileName = pegawaiHidFileName.Split('|');
                var isUploaded = 0;

                TpEmployeeAffairDocument tpEmployeeAffairDocument = new TpEmployeeAffairDocument();
                List<TpEmployeeAffairDocument> listDocumentInsert = new List<TpEmployeeAffairDocument>();

                //Update Document File0
                if (value.File0 != null)
                {

                    var file = value.File0;
                    Console.WriteLine(value.File0.FileName);
                    string[] permittedExtensions = { ".jpg", ".jpeg", ".png" };
                    var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!permittedExtensions.Contains(fileExtension))
                    {
                        Console.WriteLine("masuk = " + fileExtension);
                        return BadRequest(new { message = "Pas photo must be jpg format file", success = 0 });
                    }

                    var Id = BaseModel.GenerateId(
                               tableName: "tp_employee_affair_document",
                               primaryKey: "id",
                               str: "ID",
                               trailing: 5,
                               lastKey: "NONE");

                    Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));
                    string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    //var fileName = Path.GetFileName(file.FileName);
                    //var fileName = $"{value.EmployeeId}{documentUpdate.DocumentId}{Path.GetExtension(file.FileName)}";
                    var fileName = $"{value.EmployeeId}{Id}{Path.GetExtension(file.FileName)}";

                    var documentUpdate = databaseContext.TpEmployeeAffairDocuments.Where(x => x.EmployeeId == value.EmployeeId && x.DocumentId == "1").FirstOrDefault();
                    if (documentUpdate != null)
                    {
                        fileName = $"{documentUpdate.EmployeeId}{documentUpdate.Id}{Path.GetExtension(file.FileName)}";
                        Console.WriteLine($"pas photo {fileName}");
                        Console.WriteLine($"Mulai Update tp_employee_document_affair dengan id {documentUpdate.Id}, EmployeeId {documentUpdate.EmployeeId}, DocumentId {documentUpdate.DocumentId}");
                        documentUpdate.DocumentId = "1";
                        documentUpdate.Type = file.ContentType;
                        documentUpdate.FileName = fileName;
                        documentUpdate.Size = file.Length;
                        documentUpdate.FcDocument = value.FcDocument0;
                        documentUpdate.DocumentStatus = 1;

                        documentUpdate.IsDeleted = 0;
                        documentUpdate.SubmittedBy = id;
                        documentUpdate.Submitted = DateTime.Now;

                        databaseContext.TpEmployeeAffairDocuments.Update(documentUpdate);
                        isUploaded = 1;
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Berhasil Update tp_employee_document_affair dengan id {documentUpdate.Id}, EmployeeId {documentUpdate.EmployeeId}, DocumentId {documentUpdate.DocumentId}");
                    }
                    else
                    {
                        TpEmployeeAffairDocument tpEmployeeDocumentAffair = new TpEmployeeAffairDocument
                        {
                            EmployeeId = value.EmployeeId,
                            ClaimTypeId = "CT000",
                            FcDocument = value.FcDocument0,
                            FileName = fileName,
                            DocumentStatus = 1,
                            IsDeleted = 0,
                            Size = file.Length,
                            Submitted = DateTime.Now,
                            SubmittedBy = id,
                            Type = null,
                            DocumentId = "1",
                            Id = Id
                        };

                        databaseContext.Add(tpEmployeeDocumentAffair);
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Selesai Insert File0 tp_employee_document_affair dengan id {tpEmployeeDocumentAffair.Id}, EmployeeId {tpEmployeeDocumentAffair.EmployeeId}, DocumentId {tpEmployeeDocumentAffair.DocumentId}");
                    }

                    var filePath = Path.Combine(directoryPath, fileName);
                    //proses penyimpanan file
                    using (var stream = new FileStream(path: filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                //Update Document File1
                if (value.File1 != null)
                {
                    var file = value.File1;

                    var Id1 = BaseModel.GenerateId(
                                   tableName: "tp_employee_affair_document",
                                   primaryKey: "id",
                                   str: "ID",
                                   trailing: 5,
                                   lastKey: "NONE");

                    Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));
                    string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    var fileName = $"{value.EmployeeId}{Id1}{Path.GetExtension(file.FileName)}";


                    var documentUpdate = databaseContext.TpEmployeeAffairDocuments.Where(x => x.EmployeeId == value.EmployeeId && x.DocumentId == "2").FirstOrDefault();
                    if (documentUpdate != null)
                    {
                        fileName = $"{documentUpdate.EmployeeId}{documentUpdate.Id}{Path.GetExtension(file.FileName)}";
                        Console.WriteLine($"KTP {fileName}");
                        documentUpdate.DocumentId = "2";
                        documentUpdate.Type = file.ContentType;
                        documentUpdate.FileName = fileName;
                        documentUpdate.Size = file.Length;
                        documentUpdate.FcDocument = value.FcDocument1;
                        documentUpdate.DocumentStatus = 1;

                        documentUpdate.IsDeleted = 0;
                        documentUpdate.SubmittedBy = id;
                        documentUpdate.Submitted = DateTime.Now;

                        databaseContext.TpEmployeeAffairDocuments.Update(documentUpdate);
                        isUploaded = 1;
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Berhasil Update File1 dengan id {documentUpdate.Id}, EmployeeId {documentUpdate.EmployeeId}, DocumentId {documentUpdate.DocumentId}");
                    }
                    else
                    {
                        TpEmployeeAffairDocument tpEmployeeDocumentAffair = new TpEmployeeAffairDocument
                        {
                            EmployeeId = value.EmployeeId,
                            ClaimTypeId = "CT000",
                            FcDocument = value.FcDocument1,
                            FileName = fileName,
                            DocumentStatus = 1,
                            IsDeleted = 0,
                            Size = file.Length,
                            Submitted = DateTime.Now,
                            SubmittedBy = id,
                            Type = null,
                            DocumentId = "2",
                            Id = Id1
                        };

                        databaseContext.Add(tpEmployeeDocumentAffair);
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Selesai Insert File1 dengan id {tpEmployeeDocumentAffair.Id}, EmployeeId {tpEmployeeDocumentAffair.EmployeeId}, DocumentId {tpEmployeeDocumentAffair.DocumentId}");
                    }

                    var filePath = Path.Combine(directoryPath, fileName);
                    //proses penyimpanan file
                    using (var stream = new FileStream(path: filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                //Update Document File2
                if (value.File2 != null)
                {
                    var file = value.File2;

                    var Id2 = BaseModel.GenerateId(
                                   tableName: "tp_employee_affair_document",
                                   primaryKey: "id",
                                   str: "ID",
                                   trailing: 5,
                                   lastKey: "NONE");

                    Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));
                    string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    var fileName = $"{value.EmployeeId}{Id2}{Path.GetExtension(file.FileName)}";


                    var documentUpdate = databaseContext.TpEmployeeAffairDocuments.Where(x => x.EmployeeId == value.EmployeeId && x.DocumentId == "3").FirstOrDefault();
                    if (documentUpdate != null)
                    {
                        fileName = $"{documentUpdate.EmployeeId}{documentUpdate.Id}{Path.GetExtension(file.FileName)}";
                        Console.WriteLine($"NPWP {fileName}");
                        Console.WriteLine($"Mulai Update File2 dengan id {documentUpdate.Id}, EmployeeId {documentUpdate.EmployeeId}, DocumentId {documentUpdate.DocumentId}");
                        documentUpdate.DocumentId = "3";
                        documentUpdate.Type = file.ContentType;
                        documentUpdate.FileName = fileName;
                        documentUpdate.Size = file.Length;
                        documentUpdate.FcDocument = value.FcDocument2;
                        documentUpdate.DocumentStatus = 1;

                        documentUpdate.IsDeleted = 0;
                        documentUpdate.SubmittedBy = id;
                        documentUpdate.Submitted = DateTime.Now;

                        databaseContext.TpEmployeeAffairDocuments.Update(documentUpdate);
                        isUploaded = 1;
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Berhasil Update tp_employee_document_affair dengan id {documentUpdate.Id}, EmployeeId {documentUpdate.EmployeeId}, DocumentId {documentUpdate.DocumentId}");
                    }
                    else
                    {
                        TpEmployeeAffairDocument tpEmployeeDocumentAffair = new TpEmployeeAffairDocument
                        {
                            EmployeeId = value.EmployeeId,
                            ClaimTypeId = "CT000",
                            FcDocument = value.FcDocument2,
                            FileName = fileName,
                            DocumentStatus = 1,
                            IsDeleted = 0,
                            Size = file.Length,
                            Submitted = DateTime.Now,
                            SubmittedBy = id,
                            Type = null,
                            DocumentId = "3",
                            Id = Id2
                        };

                        databaseContext.Add(tpEmployeeDocumentAffair);
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Selesai Insert tp_employee_document_affair dengan id {tpEmployeeDocumentAffair.Id}, EmployeeId {tpEmployeeDocumentAffair.EmployeeId}, DocumentId {tpEmployeeDocumentAffair.DocumentId}");
                    }
                    var filePath = Path.Combine(directoryPath, fileName);
                    //proses penyimpanan file
                    using (var stream = new FileStream(path: filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                //Update Document File3
                if (value.File3 != null)
                {
                    var file = value.File3;

                    var Id3 = BaseModel.GenerateId(
                                   tableName: "tp_employee_affair_document",
                                   primaryKey: "id",
                                   str: "ID",
                                   trailing: 5,
                                   lastKey: "NONE");

                    Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));
                    string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    //var fileName = Path.GetFileName(file.FileName);
                    var fileName = $"{value.EmployeeId}{Id3}{Path.GetExtension(file.FileName)}";


                    var documentUpdate = databaseContext.TpEmployeeAffairDocuments.Where(x => x.EmployeeId == value.EmployeeId && x.DocumentId == "4").FirstOrDefault();
                    if (documentUpdate != null)
                    {
                        fileName = $"{documentUpdate.EmployeeId}{documentUpdate.Id}{Path.GetExtension(file.FileName)}";
                        Console.WriteLine($"Passport {fileName}");
                        documentUpdate.DocumentId = "4";
                        documentUpdate.Type = file.ContentType;
                        documentUpdate.FileName = fileName;
                        documentUpdate.Size = file.Length;
                        documentUpdate.FcDocument = value.FcDocument3;
                        documentUpdate.DocumentStatus = 1;

                        documentUpdate.IsDeleted = 0;
                        documentUpdate.SubmittedBy = id;
                        documentUpdate.Submitted = DateTime.Now;

                        databaseContext.TpEmployeeAffairDocuments.Update(documentUpdate);
                        isUploaded = 1;
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Berhasil Update tp_employee_document_affair dengan id {documentUpdate.Id}, EmployeeId {documentUpdate.EmployeeId}, DocumentId {documentUpdate.DocumentId}");
                    }
                    else
                    {
                        TpEmployeeAffairDocument tpEmployeeDocumentAffair = new TpEmployeeAffairDocument
                        {
                            EmployeeId = value.EmployeeId,
                            ClaimTypeId = "CT000",
                            FcDocument = value.FcDocument3,
                            FileName = fileName,
                            DocumentStatus = 1,
                            IsDeleted = 0,
                            Size = file.Length,
                            Submitted = DateTime.Now,
                            SubmittedBy = id,
                            Type = null,
                            DocumentId = "4",
                            Id = Id3
                        };

                        databaseContext.Add(tpEmployeeDocumentAffair);
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Selesai Insert tp_employee_document_affair dengan id {tpEmployeeDocumentAffair.Id}, EmployeeId {tpEmployeeDocumentAffair.EmployeeId}, DocumentId {tpEmployeeDocumentAffair.DocumentId}");
                    }

                    var filePath = Path.Combine(directoryPath, fileName);
                    //proses penyimpanan file
                    using (var stream = new FileStream(path: filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                //Update Document File4
                if (value.File4 != null)
                {
                    var file = value.File4;

                    var Id4 = BaseModel.GenerateId(
                                   tableName: "tp_employee_affair_document",
                                   primaryKey: "id",
                                   str: "ID",
                                   trailing: 5,
                                   lastKey: "NONE");

                    Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));
                    string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    //var fileName = Path.GetFileName(file.FileName);
                    var fileName = $"{value.EmployeeId}{Id4}{Path.GetExtension(file.FileName)}";

                    var documentUpdate = databaseContext.TpEmployeeAffairDocuments.Where(x => x.EmployeeId == value.EmployeeId && x.DocumentId == "5").FirstOrDefault();
                    if (documentUpdate != null)
                    {
                        fileName = $"{documentUpdate.EmployeeId}{documentUpdate.Id}{Path.GetExtension(file.FileName)}";
                        Console.WriteLine($"KK {fileName}");
                        documentUpdate.DocumentId = "5";
                        documentUpdate.Type = file.ContentType;
                        documentUpdate.FileName = fileName;
                        documentUpdate.Size = file.Length;
                        documentUpdate.FcDocument = value.FcDocument4;
                        documentUpdate.DocumentStatus = 1;

                        documentUpdate.IsDeleted = 0;
                        documentUpdate.SubmittedBy = id;
                        documentUpdate.Submitted = DateTime.Now;

                        databaseContext.TpEmployeeAffairDocuments.Update(documentUpdate);
                        isUploaded = 1;
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Berhasil Update tp_employee_document_affair dengan id {documentUpdate.Id}, EmployeeId {documentUpdate.EmployeeId}, DocumentId {documentUpdate.DocumentId}");
                    }
                    else
                    {
                        TpEmployeeAffairDocument tpEmployeeDocumentAffair = new TpEmployeeAffairDocument
                        {
                            EmployeeId = value.EmployeeId,
                            ClaimTypeId = "CT000",
                            FcDocument = value.FcDocument4,
                            FileName = fileName,
                            DocumentStatus = 1,
                            IsDeleted = 0,
                            Size = file.Length,
                            Submitted = DateTime.Now,
                            SubmittedBy = id,
                            Type = null,
                            DocumentId = "5",
                            Id = Id4
                        };

                        databaseContext.Add(tpEmployeeDocumentAffair);
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Selesai Insert tp_employee_document_affair dengan id {tpEmployeeDocumentAffair.Id}, EmployeeId {tpEmployeeDocumentAffair.EmployeeId}, DocumentId {tpEmployeeDocumentAffair.DocumentId}");
                    }

                    var filePath = Path.Combine(directoryPath, fileName);
                    //proses penyimpanan file
                    using (var stream = new FileStream(path: filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                //Update Document File5
                if (value.File5 != null)
                {
                    var file = value.File5;

                    var Id5 = BaseModel.GenerateId(
                                   tableName: "tp_employee_affair_document",
                                   primaryKey: "id",
                                   str: "ID",
                                   trailing: 5,
                                   lastKey: "NONE");

                    Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));
                    string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    //var fileName = Path.GetFileName(file.FileName);
                    var fileName = $"{value.EmployeeId}{Id5}{Path.GetExtension(file.FileName)}";

                    var documentUpdate = databaseContext.TpEmployeeAffairDocuments.Where(x => x.EmployeeId == value.EmployeeId && x.DocumentId == "6").FirstOrDefault();
                    if (documentUpdate != null)
                    {
                        fileName = $"{documentUpdate.EmployeeId}{documentUpdate.Id}{Path.GetExtension(file.FileName)}";
                        Console.WriteLine($"CV {fileName}");
                        documentUpdate.DocumentId = "6";
                        documentUpdate.Type = file.ContentType;
                        documentUpdate.FileName = fileName;
                        documentUpdate.Size = file.Length;
                        documentUpdate.FcDocument = value.FcDocument5;
                        documentUpdate.DocumentStatus = 1;

                        documentUpdate.IsDeleted = 0;
                        documentUpdate.SubmittedBy = id;
                        documentUpdate.Submitted = DateTime.Now;

                        databaseContext.TpEmployeeAffairDocuments.Update(documentUpdate);
                        isUploaded = 1;
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Berhasil Update tp_employee_document_affair dengan id {documentUpdate.Id}, EmployeeId {documentUpdate.EmployeeId}, DocumentId {documentUpdate.DocumentId}");
                    }
                    else
                    {
                        TpEmployeeAffairDocument tpEmployeeDocumentAffair = new TpEmployeeAffairDocument
                        {
                            EmployeeId = value.EmployeeId,
                            ClaimTypeId = "CT000",
                            FcDocument = value.FcDocument5,
                            FileName = fileName,
                            DocumentStatus = 1,
                            IsDeleted = 0,
                            Size = file.Length,
                            Submitted = DateTime.Now,
                            SubmittedBy = id,
                            Type = null,
                            DocumentId = "6",
                            Id = Id5
                        };

                        databaseContext.Add(tpEmployeeDocumentAffair);
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Selesai Insert tp_employee_document_affair dengan id {tpEmployeeDocumentAffair.Id}, EmployeeId {tpEmployeeDocumentAffair.EmployeeId}, DocumentId {tpEmployeeDocumentAffair.DocumentId}");
                    }

                    var filePath = Path.Combine(directoryPath, fileName);
                    //proses penyimpanan file
                    using (var stream = new FileStream(path: filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                //Update Document File6
                if (value.File6 != null)
                {
                    var file = value.File6;

                    var Id6 = BaseModel.GenerateId(
                                   tableName: "tp_employee_affair_document",
                                   primaryKey: "id",
                                   str: "ID",
                                   trailing: 5,
                                   lastKey: "NONE");

                    Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));
                    string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    //var fileName = Path.GetFileName(file.FileName);
                    var fileName = $"{value.EmployeeId}{Id6}{Path.GetExtension(file.FileName)}";

                    var documentUpdate = databaseContext.TpEmployeeAffairDocuments.Where(x => x.EmployeeId == value.EmployeeId && x.DocumentId == "7").FirstOrDefault();
                    if (documentUpdate != null)
                    {
                        fileName = $"{documentUpdate.EmployeeId}{documentUpdate.Id}{Path.GetExtension(file.FileName)}";
                        Console.WriteLine($"BukuNikah {fileName}");
                        documentUpdate.DocumentId = "7";
                        documentUpdate.Type = file.ContentType;
                        documentUpdate.FileName = fileName;
                        documentUpdate.Size = file.Length;
                        documentUpdate.FcDocument = value.FcDocument6;
                        documentUpdate.DocumentStatus = 1;

                        documentUpdate.IsDeleted = 0;
                        documentUpdate.SubmittedBy = id;
                        documentUpdate.Submitted = DateTime.Now;

                        databaseContext.TpEmployeeAffairDocuments.Update(documentUpdate);
                        isUploaded = 1;
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Berhasil Update tp_employee_document_affair dengan id {documentUpdate.Id}, EmployeeId {documentUpdate.EmployeeId}, DocumentId {documentUpdate.DocumentId}");
                    }
                    else
                    {
                        TpEmployeeAffairDocument tpEmployeeDocumentAffair = new TpEmployeeAffairDocument
                        {
                            EmployeeId = value.EmployeeId,
                            ClaimTypeId = "CT000",
                            FcDocument = value.FcDocument6,
                            FileName = fileName,
                            DocumentStatus = 1,
                            IsDeleted = 0,
                            Size = file.Length,
                            Submitted = DateTime.Now,
                            SubmittedBy = id,
                            Type = null,
                            DocumentId = "7",
                            Id = Id6
                        };

                        databaseContext.Add(tpEmployeeDocumentAffair);
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Selesai Insert tp_employee_document_affair dengan id {tpEmployeeDocumentAffair.Id}, EmployeeId {tpEmployeeDocumentAffair.EmployeeId}, DocumentId {tpEmployeeDocumentAffair.DocumentId}");
                    }

                    var filePath = Path.Combine(directoryPath, fileName);
                    //proses penyimpanan file
                    using (var stream = new FileStream(path: filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                //Update Document File7
                if (value.File7 != null)
                {
                    var file = value.File7;

                    var Id7 = BaseModel.GenerateId(
                                   tableName: "tp_employee_affair_document",
                                   primaryKey: "id",
                                   str: "ID",
                                   trailing: 5,
                                   lastKey: "NONE");

                    Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));
                    string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    //var fileName = Path.GetFileName(file.FileName);
                    var fileName = $"{value.EmployeeId}{Id7}{Path.GetExtension(file.FileName)}";

                    var documentUpdate = databaseContext.TpEmployeeAffairDocuments.Where(x => x.EmployeeId == value.EmployeeId && x.DocumentId == "8").FirstOrDefault();
                    if (documentUpdate != null)
                    {
                        fileName = $"{documentUpdate.EmployeeId}{documentUpdate.Id}{Path.GetExtension(file.FileName)}";
                        Console.WriteLine($"SuratKeputusan {fileName}");
                        documentUpdate.DocumentId = "8";
                        documentUpdate.Type = file.ContentType;
                        documentUpdate.FileName = fileName;
                        documentUpdate.Size = file.Length;
                        documentUpdate.FcDocument = value.FcDocument7;
                        documentUpdate.DocumentStatus = 1;

                        documentUpdate.IsDeleted = 0;
                        documentUpdate.SubmittedBy = id;
                        documentUpdate.Submitted = DateTime.Now;

                        databaseContext.TpEmployeeAffairDocuments.Update(documentUpdate);
                        isUploaded = 1;
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Berhasil Update tp_employee_document_affair dengan id {documentUpdate.Id}, EmployeeId {documentUpdate.EmployeeId}, DocumentId {documentUpdate.DocumentId}");
                    }
                    else
                    {
                        TpEmployeeAffairDocument tpEmployeeDocumentAffair = new TpEmployeeAffairDocument
                        {
                            EmployeeId = value.EmployeeId,
                            ClaimTypeId = "CT000",
                            FcDocument = value.FcDocument7,
                            FileName = fileName,
                            DocumentStatus = 1,
                            IsDeleted = 0,
                            Size = file.Length,
                            Submitted = DateTime.Now,
                            SubmittedBy = id,
                            Type = null,
                            DocumentId = "8",
                            Id = Id7
                        };

                        databaseContext.Add(tpEmployeeDocumentAffair);
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Selesai Insert tp_employee_document_affair dengan id {tpEmployeeDocumentAffair.Id}, EmployeeId {tpEmployeeDocumentAffair.EmployeeId}, DocumentId {tpEmployeeDocumentAffair.DocumentId}");
                    }

                    var filePath = Path.Combine(directoryPath, fileName);
                    //proses penyimpanan file
                    using (var stream = new FileStream(path: filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                //Update Document File8
                if (value.File8 != null)
                {
                    var file = value.File8;

                    var Id8 = BaseModel.GenerateId(
                                   tableName: "tp_employee_affair_document",
                                   primaryKey: "id",
                                   str: "ID",
                                   trailing: 5,
                                   lastKey: "NONE");

                    Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));
                    string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    //var fileName = Path.GetFileName(file.FileName);
                    var fileName = $"{value.EmployeeId}{Id8}{Path.GetExtension(file.FileName)}";

                    var documentUpdate = databaseContext.TpEmployeeAffairDocuments.Where(x => x.EmployeeId == value.EmployeeId && x.DocumentId == "9").FirstOrDefault();
                    if (documentUpdate != null)
                    {
                        fileName = $"{documentUpdate.EmployeeId}{documentUpdate.Id}{Path.GetExtension(file.FileName)}";
                        Console.WriteLine($"BukuRekening {fileName}");
                        documentUpdate.DocumentId = "9";
                        documentUpdate.Type = file.ContentType;
                        documentUpdate.FileName = fileName;
                        documentUpdate.Size = file.Length;
                        documentUpdate.FcDocument = value.FcDocument8;
                        documentUpdate.DocumentStatus = 1;

                        documentUpdate.IsDeleted = 0;
                        documentUpdate.SubmittedBy = id;
                        documentUpdate.Submitted = DateTime.Now;

                        databaseContext.TpEmployeeAffairDocuments.Update(documentUpdate);
                        isUploaded = 1;
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Berhasil Update tp_employee_document_affair dengan id {documentUpdate.Id}, EmployeeId {documentUpdate.EmployeeId}, DocumentId {documentUpdate.DocumentId}");
                    }
                    else
                    {
                        TpEmployeeAffairDocument tpEmployeeDocumentAffair = new TpEmployeeAffairDocument
                        {
                            EmployeeId = value.EmployeeId,
                            ClaimTypeId = "CT000",
                            FcDocument = value.FcDocument8,
                            FileName = fileName,
                            DocumentStatus = 1,
                            IsDeleted = 0,
                            Size = file.Length,
                            Submitted = DateTime.Now,
                            SubmittedBy = id,
                            Type = null,
                            DocumentId = "9",
                            Id = Id8
                        };

                        databaseContext.Add(tpEmployeeDocumentAffair);
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Selesai Insert tp_employee_document_affair dengan id {tpEmployeeDocumentAffair.Id}, EmployeeId {tpEmployeeDocumentAffair.EmployeeId}, DocumentId {tpEmployeeDocumentAffair.DocumentId}");
                    }

                    var filePath = Path.Combine(directoryPath, fileName);
                    //proses penyimpanan file
                    using (var stream = new FileStream(path: filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                //Update Document File9
                if (value.File9 != null)
                {
                    var file = value.File9;

                    var Id9 = BaseModel.GenerateId(
                                   tableName: "tp_employee_affair_document",
                                   primaryKey: "id",
                                   str: "ID",
                                   trailing: 5,
                                   lastKey: "NONE");

                    Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));
                    string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    //var fileName = Path.GetFileName(file.FileName);
                    var fileName = $"{value.EmployeeId}{Id9}{Path.GetExtension(file.FileName)}";

                    var documentUpdate = databaseContext.TpEmployeeAffairDocuments.Where(x => x.EmployeeId == value.EmployeeId && x.DocumentId == "10").FirstOrDefault();
                    if (documentUpdate != null)
                    {
                        fileName = $"{documentUpdate.EmployeeId}{documentUpdate.Id}{Path.GetExtension(file.FileName)}";
                        Console.WriteLine($"BPJSTK {fileName}");
                        documentUpdate.DocumentId = "10";
                        documentUpdate.Type = file.ContentType;
                        documentUpdate.FileName = fileName;
                        documentUpdate.Size = file.Length;
                        documentUpdate.FcDocument = value.FcDocument9;
                        documentUpdate.DocumentStatus = 1;

                        documentUpdate.IsDeleted = 0;
                        documentUpdate.SubmittedBy = id;
                        documentUpdate.Submitted = DateTime.Now;

                        databaseContext.TpEmployeeAffairDocuments.Update(documentUpdate);
                        isUploaded = 1;
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Berhasil Update tp_employee_document_affair dengan id {documentUpdate.Id}, EmployeeId {documentUpdate.EmployeeId}, DocumentId {documentUpdate.DocumentId}");
                    }
                    else
                    {
                        TpEmployeeAffairDocument tpEmployeeDocumentAffair = new TpEmployeeAffairDocument
                        {
                            EmployeeId = value.EmployeeId,
                            ClaimTypeId = "CT000",
                            FcDocument = value.FcDocument9,
                            FileName = fileName,
                            DocumentStatus = 1,
                            IsDeleted = 0,
                            Size = file.Length,
                            Submitted = DateTime.Now,
                            SubmittedBy = id,
                            Type = null,
                            DocumentId = "10",
                            Id = Id9
                        };

                        databaseContext.Add(tpEmployeeDocumentAffair);
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Selesai Insert tp_employee_document_affair dengan id {tpEmployeeDocumentAffair.Id}, EmployeeId {tpEmployeeDocumentAffair.EmployeeId}, DocumentId {tpEmployeeDocumentAffair.DocumentId}");
                    }

                    var filePath = Path.Combine(directoryPath, fileName);
                    //proses penyimpanan file
                    using (var stream = new FileStream(path: filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                //Update Document File10
                if (value.File10 != null)
                {
                    var file = value.File10;

                    var Id10 = BaseModel.GenerateId(
                                   tableName: "tp_employee_affair_document",
                                   primaryKey: "id",
                                   str: "ID",
                                   trailing: 5,
                                   lastKey: "NONE");

                    Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));
                    string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    //var fileName = Path.GetFileName(file.FileName);
                    var fileName = $"{value.EmployeeId}{Id10}{Path.GetExtension(file.FileName)}";

                    var documentUpdate = databaseContext.TpEmployeeAffairDocuments.Where(x => x.EmployeeId == value.EmployeeId && x.DocumentId == "11").FirstOrDefault();
                    if (documentUpdate != null)
                    {
                        fileName = $"{documentUpdate.EmployeeId}{documentUpdate.Id}{Path.GetExtension(file.FileName)}";
                        Console.WriteLine($"BPJSkes {fileName}");
                        documentUpdate.DocumentId = "11";
                        documentUpdate.Type = file.ContentType;
                        documentUpdate.FileName = fileName;
                        documentUpdate.Size = file.Length;
                        documentUpdate.FcDocument = value.FcDocument10;
                        documentUpdate.DocumentStatus = 1;

                        documentUpdate.IsDeleted = 0;
                        documentUpdate.SubmittedBy = id;
                        documentUpdate.Submitted = DateTime.Now;

                        databaseContext.TpEmployeeAffairDocuments.Update(documentUpdate);
                        isUploaded = 1;
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Berhasil Update tp_employee_document_affair dengan id {documentUpdate.Id}, EmployeeId {documentUpdate.EmployeeId}, DocumentId {documentUpdate.DocumentId}");
                    }
                    else
                    {
                        TpEmployeeAffairDocument tpEmployeeDocumentAffair = new TpEmployeeAffairDocument
                        {
                            EmployeeId = value.EmployeeId,
                            ClaimTypeId = "CT000",
                            FcDocument = value.FcDocument10,
                            FileName = fileName,
                            DocumentStatus = 1,
                            IsDeleted = 0,
                            Size = file.Length,
                            Submitted = DateTime.Now,
                            SubmittedBy = id,
                            Type = null,
                            DocumentId = "11",
                            Id = Id10
                        };

                        databaseContext.Add(tpEmployeeDocumentAffair);
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Selesai Insert tp_employee_document_affair dengan id {tpEmployeeDocumentAffair.Id}, EmployeeId {tpEmployeeDocumentAffair.EmployeeId}, DocumentId {tpEmployeeDocumentAffair.DocumentId}");
                    }

                    var filePath = Path.Combine(directoryPath, fileName);
                    //proses penyimpanan file
                    using (var stream = new FileStream(path: filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                //Update Document File11
                if (value.File11 != null)
                {
                    var file = value.File11;

                    var Id11 = BaseModel.GenerateId(
                                   tableName: "tp_employee_affair_document",
                                   primaryKey: "id",
                                   str: "ID",
                                   trailing: 5,
                                   lastKey: "NONE");

                    Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));
                    string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    //var fileName = Path.GetFileName(file.FileName);
                    var fileName = $"{value.EmployeeId}{Id11}{Path.GetExtension(file.FileName)}";

                    var documentUpdate = databaseContext.TpEmployeeAffairDocuments.Where(x => x.EmployeeId == value.EmployeeId && x.DocumentId == "12").FirstOrDefault();
                    if (documentUpdate != null)
                    {
                        fileName = $"{documentUpdate.EmployeeId}{documentUpdate.Id}{Path.GetExtension(file.FileName)}";
                        Console.WriteLine($"IjazahTerakhir {fileName}");
                        documentUpdate.DocumentId = "12";
                        documentUpdate.Type = file.ContentType;
                        documentUpdate.FileName = fileName;
                        documentUpdate.Size = file.Length;
                        documentUpdate.FcDocument = value.FcDocument11;
                        documentUpdate.DocumentStatus = 1;

                        documentUpdate.IsDeleted = 0;
                        documentUpdate.SubmittedBy = id;
                        documentUpdate.Submitted = DateTime.Now;

                        databaseContext.TpEmployeeAffairDocuments.Update(documentUpdate);
                        isUploaded = 1;
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Berhasil Update tp_employee_document_affair dengan id {documentUpdate.Id}, EmployeeId {documentUpdate.EmployeeId}, DocumentId {documentUpdate.DocumentId}");
                    }
                    else
                    {
                        TpEmployeeAffairDocument tpEmployeeDocumentAffair = new TpEmployeeAffairDocument
                        {
                            EmployeeId = value.EmployeeId,
                            ClaimTypeId = "CT000",
                            FcDocument = value.FcDocument11,
                            FileName = fileName,
                            DocumentStatus = 1,
                            IsDeleted = 0,
                            Size = file.Length,
                            Submitted = DateTime.Now,
                            SubmittedBy = id,
                            Type = null,
                            DocumentId = "12",
                            Id = Id11
                        };

                        databaseContext.Add(tpEmployeeDocumentAffair);
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Selesai Insert tp_employee_document_affair dengan id {tpEmployeeDocumentAffair.Id}, EmployeeId {tpEmployeeDocumentAffair.EmployeeId}, DocumentId {tpEmployeeDocumentAffair.DocumentId}");
                    }

                    var filePath = Path.Combine(directoryPath, fileName);
                    //proses penyimpanan file
                    using (var stream = new FileStream(path: filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                //Update Document File12
                if (value.File12 != null)
                {
                    var file = value.File12;

                    var Id12 = BaseModel.GenerateId(
                                   tableName: "tp_employee_affair_document",
                                   primaryKey: "id",
                                   str: "ID",
                                   trailing: 5,
                                   lastKey: "NONE");

                    Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));
                    string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    //var fileName = Path.GetFileName(file.FileName);
                    var fileName = $"{value.EmployeeId}{Id12}{Path.GetExtension(file.FileName)}";

                    var documentUpdate = databaseContext.TpEmployeeAffairDocuments.Where(x => x.EmployeeId == value.EmployeeId && x.DocumentId == "13").FirstOrDefault();
                    if (documentUpdate != null)
                    {
                        fileName = $"{documentUpdate.EmployeeId}{documentUpdate.Id}{Path.GetExtension(file.FileName)}";
                        Console.WriteLine($"BukuTabungan {fileName}");
                        documentUpdate.DocumentId = "13";
                        documentUpdate.Type = file.ContentType;
                        documentUpdate.FileName = fileName;
                        documentUpdate.Size = file.Length;
                        documentUpdate.FcDocument = value.FcDocument12;
                        documentUpdate.DocumentStatus = 1;

                        documentUpdate.IsDeleted = 0;
                        documentUpdate.SubmittedBy = id;
                        documentUpdate.Submitted = DateTime.Now;

                        databaseContext.TpEmployeeAffairDocuments.Update(documentUpdate);
                        isUploaded = 1;
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Berhasil Update tp_employee_document_affair dengan id {documentUpdate.Id}, EmployeeId {documentUpdate.EmployeeId}, DocumentId {documentUpdate.DocumentId}");
                    }
                    else
                    {
                        TpEmployeeAffairDocument tpEmployeeDocumentAffair = new TpEmployeeAffairDocument
                        {
                            EmployeeId = value.EmployeeId,
                            ClaimTypeId = "CT000",
                            FcDocument = value.FcDocument12,
                            FileName = fileName,
                            DocumentStatus = 1,
                            IsDeleted = 0,
                            Size = file.Length,
                            Submitted = DateTime.Now,
                            SubmittedBy = id,
                            Type = null,
                            DocumentId = "13",
                            Id = Id12
                        };

                        databaseContext.Add(tpEmployeeDocumentAffair);
                        databaseContext.SaveChanges();
                        Console.WriteLine($"Selesai Insert tp_employee_document_affair dengan id {tpEmployeeDocumentAffair.Id}, EmployeeId {tpEmployeeDocumentAffair.EmployeeId}, DocumentId {tpEmployeeDocumentAffair.DocumentId}");
                    }

                    var filePath = Path.Combine(directoryPath, fileName);
                    //proses penyimpanan file
                    using (var stream = new FileStream(path: filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                if (isUploaded == 1)
                {
                    databaseContext.TpEmployeeAffairDocuments.UpdateRange(listDocumentInsert);

                    databaseContext.SaveChanges();
                }

                var updateAffair = databaseContext.TmEmployeeAffairs.FirstOrDefault(e => e.EmployeeId == value.EmployeeId);
                if (updateAffair != null)
                {
                    updateAffair.EmployeeFirstName = value.EmployeeFirstName;
                    updateAffair.EmployeeLastName = value.EmployeeLastName;
                    updateAffair.DepartmentId = value.DepartmentId;
                    updateAffair.TitleId = value.TitleId;
                    updateAffair.LevelId = value.LevelId;
                    updateAffair.EmployeeStatusId = value.EmployeeStatusId;
                    updateAffair.JointDate = value.JointDate;
                    updateAffair.EndOfContract = value.EndOfContract;
                    updateAffair.PermanentDate = value.PermanentDate;
                    updateAffair.TempatLahir = value.TempatLahir;
                    updateAffair.TanggalLahir = value.TanggalLahir;
                    updateAffair.Alamat = value.Alamat;
                    updateAffair.AlamatTinggal = value.AlamatTinggal;
                    updateAffair.ReligiId = value.ReligiId;
                    updateAffair.GenderId = value.GenderId;
                    updateAffair.MarriedId = value.MarriedId;
                    updateAffair.BankId = value.BankId;
                    updateAffair.NoRekening = value.NoRekening;
                    updateAffair.Email = value.Email;

                    updateAffair.DtEtr = DateTime.Now;
                    updateAffair.UserEtr = id;
                    updateAffair.DtUpdate = DateTime.Now;
                    updateAffair.UserUpdate = id;

                    databaseContext.SaveChanges();
                }

                var updateTmUser = databaseContext.TmUsers.FirstOrDefault(e => e.EmployeeId == value.EmployeeId);
                if (updateTmUser != null)
                {
                    updateTmUser.Email = value.Email == null ? "-" : value.Email;
                    databaseContext.SaveChanges();
                }

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException == null ? ex : ex.InnerException);
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

        [HttpPost("{id}/update_getone")]
        public IActionResult UpdateGetOne([FromBody] updateGetOne value)
        {
            List<Dictionary<string, dynamic>> update = employeeDbModel.UpdateGetOne(value);

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

        [HttpPost("{id}/update_gettwo")]
        public IActionResult UpdateGetTwo([FromForm] updateGetTwo value)
        {
            TpEmployeeStudi tpEmployeeStudi = new TpEmployeeStudi();
            if (value.File != null && value.File.Length > 0)
            {
                Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));
                string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()), value.EmployeeId);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                IFormFile file = value.File;
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(directoryPath, fileName);

                using (var stream = new FileStream(path: filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    //fileName = filePath;
                }

                Console.Write($"File '{fileName}' berhasil disimpan dilokasi: {filePath}");
                tpEmployeeStudi.FileName = fileName;
                tpEmployeeStudi.Type = value.File.ContentType;
                tpEmployeeStudi.Size = file.Length;
            }

            List<Dictionary<string, dynamic>> update = employeeDbModel.UpdateGetTwo(value);

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

        [HttpPost("{id}/update_getthree")]
        public IActionResult UpdateGetThree([FromForm] updateGetThree value)
        {
            TpEmployeeLicense tpEmployeeLicense = new TpEmployeeLicense();
            if (value.File != null && value.File.Length > 0)
            {
                Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()));
                string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.PegawaiNext, isProduction: _env.IsProduction()), value.EmployeeId);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                IFormFile file = value.File;
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(directoryPath, fileName);

                using (var stream = new FileStream(path: filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    //fileName = filePath;
                }

                tpEmployeeLicense.FileName = fileName;
                tpEmployeeLicense.Type = value.File.ContentType;
                tpEmployeeLicense.Size = file.Length;
            }

            List<Dictionary<string, dynamic>> update = employeeDbModel.UpdateGetThree(value);

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

        [HttpPost("{id}/update_getfour")]
        public IActionResult UpdateGetFour([FromBody] updateGetFour value)
        {
            List<Dictionary<string, dynamic>> update = employeeDbModel.UpdateGetFour(value);

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
        public IActionResult DeletePegawaiNext([FromBody] getModel value)
        {
            var getDelete = employeeDbModel.nextFindById(value!.EmployeeId!);
            if (getDelete == null)
            {
                Responses.Add("code", ErrorCode.NotFound);
                Responses.Add("message", ErrorMessege.NotFound);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", "Data tidak ditemukan");
                return NotFound(Data);
            }

            var statusUpdate = databaseContext.TmEmployeeAffairs.FirstOrDefault(x => x.EmployeeId == value.EmployeeId);
            if (statusUpdate != null)
            {
                statusUpdate.Status = 0;
                databaseContext.SaveChanges();
            }

            Responses.Add("code", ErrorCode.Ok);
            Responses.Add("message", ErrorMessege.Ok);
            ListResponse.Add(Responses);

            Detail.Add("datas", value.EmployeeId);
            ListData.Add(Detail);

            Data.Add("response", ListResponse);
            Data.Add("data", ListData);
            return Ok(Data);
        }

        [HttpPost("{id}/deletegetone")]
        public IActionResult DeleteGetOne([FromBody] deleteGetOne value)
        {
            var getOneDelete = employeeDbModel.getOneFindById(value!.FamilyId!);
            if (getOneDelete == null)
            {
                Responses.Add("code", ErrorCode.NotFound);
                Responses.Add("message", ErrorMessege.NotFound);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", "Data tidak ditemukan");
                return NotFound(Data);
            }

            var deleteGetOne = employeeDbModel.getOneDelete(value);
            if (!deleteGetOne)
            {
                Responses.Add("code", ErrorCode.BadRequest);
                Responses.Add("message", ErrorMessege.BadRequest);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", "Gagal hapus!");
                return BadRequest(Data);
            }
            Responses.Clear();
            ListResponse.Clear();
            ListData.Clear();
            Detail.Clear();
            Data.Clear();

            Responses.Add("code", ErrorCode.Ok);
            Responses.Add("message", ErrorMessege.Ok);
            ListResponse.Add(Responses);

            Detail.Add("datas", value.FamilyId);
            ListData.Add(Detail);

            Data.Add("response", ListResponse);
            Data.Add("data", ListData);
            Data.Add("message", "Berhasil Hapus!");
            return Ok(Data);
        }

        [HttpPost("{id}/deletegettwo")]
        public IActionResult DeleteGetTwo([FromBody] deleteGetTwo value)
        {
            var getTwoDelete = employeeDbModel.getTwoFindById(value!.PendidikanId!);
            if (getTwoDelete == null)
            {
                Responses.Add("code", ErrorCode.NotFound);
                Responses.Add("message", ErrorMessege.NotFound);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", "Data tidak ditemukan");
                return NotFound(Data);
            }

            var deleteGetTwo = employeeDbModel.getTwoDelete(value);
            if (!deleteGetTwo)
            {
                Responses.Add("code", ErrorCode.BadRequest);
                Responses.Add("message", ErrorMessege.BadRequest);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", "Gagal hapus!");
                return BadRequest(Data);
            }
            Responses.Clear();
            ListResponse.Clear();
            ListData.Clear();
            Detail.Clear();
            Data.Clear();

            Responses.Add("code", ErrorCode.Ok);
            Responses.Add("message", ErrorMessege.Ok);
            ListResponse.Add(Responses);

            Detail.Add("datas", value.PendidikanId);
            ListData.Add(Detail);

            Data.Add("response", ListResponse);
            Data.Add("data", ListData);
            Data.Add("message", "Berhasil Hapus!");
            return Ok(Data);
        }

        [HttpPost("{id}/deletegetthree")]
        public IActionResult DeleteGetThree([FromBody] deleteGetThree value)
        {
            var getThreeDelete = employeeDbModel.getThreeFindById(value!.LicenseId!);
            if (getThreeDelete == null)
            {
                Responses.Add("code", ErrorCode.NotFound);
                Responses.Add("message", ErrorMessege.NotFound);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", "Data tidak ditemukan");
                return NotFound(Data);
            }

            var deleteGetThree = employeeDbModel.getThreeDelete(value);
            if (!deleteGetThree)
            {
                Responses.Add("code", ErrorCode.BadRequest);
                Responses.Add("message", ErrorMessege.BadRequest);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", "Gagal hapus!");
                return BadRequest(Data);
            }
            Responses.Clear();
            ListResponse.Clear();
            ListData.Clear();
            Detail.Clear();
            Data.Clear();

            Responses.Add("code", ErrorCode.Ok);
            Responses.Add("message", ErrorMessege.Ok);
            ListResponse.Add(Responses);

            Detail.Add("datas", value.LicenseId);
            ListData.Add(Detail);

            Data.Add("response", ListResponse);
            Data.Add("data", ListData);
            Data.Add("message", "Berhasil Hapus!");
            return Ok(Data);
        }

        [HttpPost("{id}/deletegetfour")]
        public IActionResult DeleteGetFour([FromBody] deleteGetFour value)
        {
            var getFourDelete = employeeDbModel.getFourFindById(value!.RelationId!);
            if (getFourDelete == null)
            {
                Responses.Add("code", ErrorCode.NotFound);
                Responses.Add("message", ErrorMessege.NotFound);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", "Data tidak ditemukan");
                return NotFound(Data);
            }

            var deleteGetFour = employeeDbModel.getFourDelete(value);
            if (!deleteGetFour)
            {
                Responses.Add("code", ErrorCode.BadRequest);
                Responses.Add("message", ErrorMessege.BadRequest);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("data", ListData);
                Data.Add("message", "Gagal hapus!");
                return BadRequest(Data);
            }
            Responses.Clear();
            ListResponse.Clear();
            ListData.Clear();
            Detail.Clear();
            Data.Clear();

            Responses.Add("code", ErrorCode.Ok);
            Responses.Add("message", ErrorMessege.Ok);
            ListResponse.Add(Responses);

            Detail.Add("datas", value.RelationId);
            ListData.Add(Detail);

            Data.Add("response", ListResponse);
            Data.Add("data", ListData);
            Data.Add("message", "Berhasil Hapus!");
            return Ok(Data);
        }


    }
}
