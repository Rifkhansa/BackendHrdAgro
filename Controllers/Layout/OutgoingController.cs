using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Layout;
using BackendHrdAgro.Models.Master;
using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Org.BouncyCastle.Asn1;

namespace BackendHrdAgro.Controllers.Layout
{
    [Route("api/outgoingletter")]
    [ApiController]
    public class OutgoingController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<OutgoingController> _logger;

        public OutgoingController(IWebHostEnvironment env, ILogger<OutgoingController> logger)
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
        DatabaseContext dbContext = new DatabaseContext();
        OutgoingLetter outgoingLetterDb = new OutgoingLetter();
        TpOutgoingLetter tpOutgoingLetter = new TpOutgoingLetter();

        [HttpPost("{id}")]
        public IActionResult Index(string id = "USR-201710052")
        {
            var kriteria = "";
            string[] arrayTitle = { "DS002", "DS003" };

            try
            {
                var findSessionData = userDB.FindSessionDataUser(id);
                string employeeId = findSessionData[0].EmployeeId;
                string departmentId = findSessionData[0].DepartmentId;

                if (departmentId == "DP006" || employeeId == "0808003") // untuk hrd
                {
                    kriteria = "";
                }
                else
                {
                    kriteria = $"and x.department_id='{departmentId}'";
                }

                var groups = outgoingLetterDb.group();
                var letterForms = outgoingLetterDb.letterForms(departmentId);
                var letterTypes = outgoingLetterDb.letterTypes();
                var titles = outgoingLetterDb.titleOutgoings();
                var cob = outgoingLetterDb.cobs();
                var securities = outgoingLetterDb.security();
                var senders = outgoingLetterDb.senders();

                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("groups", groups);
                Detail.Add("letterForms", letterForms);
                Detail.Add("letterTypes", letterTypes);
                Detail.Add("titles", titles);
                Detail.Add("cob", cob);
                Detail.Add("securities", securities);
                Detail.Add("senders", senders);

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

        [HttpPost("{id}/create")]
        public ActionResult Create([FromBody] CreateOutgoing value)
        {
            try
            {
                var letterDate = value.LetterDate.ToString("yyyy-MM-dd");
                var LetterId = BaseModel
                    .GenerateId(
                    tableName: "tp_outgoing_letter",
                    primaryKey: "letter_id",
                    str: "LET" + DateTime.Now.ToString("yyyyMM"),
                    trailing: 6,
                    lastKey: "NONE");
                Console.WriteLine(LetterId + "Letter Id");

                var f_letterFormId = string.Empty;
                var typeLetterForm = string.Empty;

                var letterFormId = "select mapping_id\r\n  from tm_letter_form a\r\n  " +
                    $"WHERE a.letter_form_id = '{value.LetterFormId}'";

                //letterForm
                var myLetterForm = dbContext.MappingIds.FromSqlRaw(letterFormId).FirstOrDefault();
                if (myLetterForm != null)
                {
                    var mappingId = myLetterForm.LetterMappingId;
                    if (!string.IsNullOrEmpty(mappingId))
                    {
                        f_letterFormId = mappingId + "/";
                        typeLetterForm = mappingId;
                    }
                }

                if (string.IsNullOrEmpty(typeLetterForm))
                {
                    typeLetterForm = string.Empty;
                }

                string letterDateYear = letterDate.Substring(0, 4);
                var letterNumber = BaseModel
                    .GenerateLetterNumber(
                    "",
                    additionalCriteria: letterDateYear,
                    typeLetterForm: typeLetterForm
                    );

                Console.WriteLine(letterNumber + "Letter Numbers");
                tpOutgoingLetter.LetterId = LetterId;
                tpOutgoingLetter.LetterDate = value.LetterDate;

                var myYear = letterDate.Substring(0, 4);
                var myMonth = letterDate.Substring(5, 2);
                var arrayBulan = new Dictionary<int, string>
                {
                    { 1, "I" },
                    { 2, "II" },
                    { 3, "III" },
                    { 4, "IV" },
                    { 5, "V" },
                    { 6, "VI" },
                    { 7, "VII" },
                    { 8, "VIII" },
                    { 9, "IX" },
                    { 10, "X" },
                    { 11, "XI" },
                    { 12, "XII" }
                };
                var month = arrayBulan[int.Parse(myMonth)];

                tpOutgoingLetter.LetterFormId = value.LetterFormId;
                tpOutgoingLetter.MappingId = typeLetterForm;
                tpOutgoingLetter.LetterTypeId = value.LetterTypeId;
                var f_letterTypeId = string.Empty;

                var letterTypeId = "select mapping_id\r\n  from tm_letter_type a\r\n " +
                    $"WHERE a.letter_type_id = '{value.LetterTypeId}'";

                //letterType
                var myLetterType = dbContext.MappingIds.FromSqlRaw(letterTypeId).FirstOrDefault();
                if (myLetterType != null)
                {
                    var mappingId = myLetterType.LetterMappingId;
                    if (!string.IsNullOrEmpty(mappingId))
                    {
                        f_letterTypeId = mappingId + "/";
                    }
                }

                var f_deptCode = string.Empty;
                var f_deptUnit = string.Empty;
                var query = "select dept_code\r\n from tm_department a\r\n \t " +
                    $"WHERE a.department_id = '{value.DepartmentId}'";

                var myQuery = dbContext.DepartmentOutgoings.FromSqlRaw(query).FirstOrDefault();
                Console.WriteLine(myQuery);
                if (myQuery != null)
                {
                    f_deptCode = myQuery.DeptCode;
                    if (value.TitleId == "DS001")
                    {
                        f_deptUnit = "/BOD";
                    }
                    else if (value.TitleId == "DS002")
                    {
                        f_deptUnit = "/M";
                    }
                }

                tpOutgoingLetter.SignatureBy = value.TitleId;
                tpOutgoingLetter.Destination = value.Destination;
                tpOutgoingLetter.Remark = value.Remark;
                tpOutgoingLetter.Tender = "";
                tpOutgoingLetter.CobId = value.CobId;
                tpOutgoingLetter.SecurityId = value.SecurityId;
                tpOutgoingLetter.LetterNumber = letterNumber;

                var letterFormatNumber = letterNumber + "/" + f_letterTypeId + f_letterFormId + "AGRO/" + month + "/" + myYear + f_deptUnit;
                Console.WriteLine(letterFormatNumber);
                tpOutgoingLetter.LetterFormatNumber = letterFormatNumber;
                tpOutgoingLetter.UserSender = value.SenderId;
                tpOutgoingLetter.ReceiptDt = value.ReceiptDt;
                tpOutgoingLetter.ReceiptBy = value.ReceiptBy;

                tpOutgoingLetter.DepartmentId = value.DepartmentId;
                tpOutgoingLetter.Status = 1;
                tpOutgoingLetter.IsAccepted = 0;
                tpOutgoingLetter.DtEtr = DateTime.Now;
                tpOutgoingLetter.UserEtr = value.EmployeeId;
                tpOutgoingLetter.DtUpdate = DateTime.Now;
                tpOutgoingLetter.UserUpdate = value.EmployeeId;
                tpOutgoingLetter.DocumentStatus = 0;

                dbContext.TpOutgoingLetters.Add(tpOutgoingLetter);
                dbContext.SaveChanges();

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

        [HttpPost("{id}/update")]
        public async Task<IActionResult> Update([FromForm] UpdateOutgoing value, string id)
        {
            try
            {
                using DatabaseContext context = new DatabaseContext();

                // cek apakah OutgoingLetter yang akan diupdate ada di dalam database
                var outgoingletter = await context.TpOutgoingLetters.Where(x => x.LetterId.Equals(value.LetterId)).FirstOrDefaultAsync() ?? throw new Exception("Data Letter Outgoing tidak ada didalam database"); // error message jika data request tidak ada didalam database

                // get mapping dari tm_letter_form sesuai dengan request letterFormId yang dikirim
                var letterFormMappingId = await context.TmLetterForms.Where(x => x.LetterFormId.Equals(value.LetterFormId)).Select(x => x.LetterMapping).FirstOrDefaultAsync() ?? throw new Exception("Invalid mapping letter form");

                // get mapping dari tm_letter_type 
                var letterTypeMappingId = await context.TmLetterTypes.Where(x => x.LetterTypeId.Equals(value.LetterTypeId)).Select(x => x.MappingId).FirstOrDefaultAsync() ?? throw new Exception("Invalid mapping letter type");

                // get user department from created OutgoingLetter
                var departmentCode = await context.TmDepartments.Where(x => x.DepartmentId.Equals(value.DepartmentId)).Select(x => x.DeptCode).FirstOrDefaultAsync() ?? throw new Exception("Department id not found");

                var bod = await context.TmTitles.Where(x => x.TitleId.Equals("DS001")).FirstOrDefaultAsync() ?? throw new Exception("");
                var manager = await context.TmTitles.Where(x => x.TitleId.Equals("DS002")).FirstOrDefaultAsync() ?? throw new Exception("");

                string signaturedBy = string.Empty;
                // cek apakah outgoing letter di tandatangin oleh direktur
                if (bod.TitleId.Equals(value.TitleId))
                {
                    signaturedBy = bod.TitleName;
                }
                // cek apakah outgoing letter di tandatangin oleh manager
                else if (manager.TitleId.Equals(value.TitleId))
                {
                    signaturedBy = $"{departmentCode}M";
                }
                else
                {
                    signaturedBy = departmentCode;
                }

                string letterMonthInRoman = BaseModel.StaticIntToRoman(value.LetterDate.Month);
                string letterFormatNumber = $"{outgoingletter.LetterNumber}/{letterTypeMappingId}/{letterFormMappingId}AGRO/{letterMonthInRoman}/{value.LetterDate.Year}/{signaturedBy}";

                //
                var filename = string.Empty;
                long size = 0;
                var fileType = string.Empty;

                if (value.File != null)
                {
                    var fileTypes = new[] { "image/gif", "image/bmp", "image/jpeg", "image/pjpeg", "image/png", "application/pdf" };
                    if (!fileTypes.Contains(value.File.ContentType))
                    {
                        var msg = "File surat harus format PDF atau Image";
                        Data.Add("response", ListResponse);
                        Data.Add("data", ListData);
                        Data.Add("message", msg);
                        return BadRequest(Data);
                    }

                    Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.OutgoingLetter, isProduction: _env.IsProduction()));
                    string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.OutgoingLetter, isProduction: _env.IsProduction()), value.LetterId);

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    IFormFile file = value.File;
                    string fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(directoryPath, fileName);

                    // Save file
                    using (var stream = new FileStream(path: filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        fileName = filePath;
                    }

                    filename = value.File.FileName;
                    fileType = value.File.ContentType;
                    size = value.File.Length;
                }

                outgoingletter.LetterDate = value.LetterDate;
                outgoingletter.LetterFormId = value.LetterFormId;
                outgoingletter.LetterTypeId = value.LetterTypeId;
                outgoingletter.SignatureBy = value.TitleId;
                outgoingletter.Destination = value.Destination;
                outgoingletter.Remark = value.Remark;
                outgoingletter.CobId = value.CobId;
                outgoingletter.SecurityId = value.SecurityId;

                outgoingletter.LetterFormatNumber = letterFormatNumber;
                outgoingletter.UserSender = value.SenderId;
                outgoingletter.ReceiptDt = value.ReceiptDt;
                outgoingletter.ReceiptBy = value.ReceiptBy;
                outgoingletter.DtUpdate = DateTime.Now;
                outgoingletter.UserUpdate = id;
                outgoingletter.FileName = filename;
                outgoingletter.Size = size;
                outgoingletter.DocumentStatus = string.IsNullOrEmpty(filename) ? (sbyte)0 : (sbyte)1;

                context.TpOutgoingLetters.Update(outgoingletter);
                await context.SaveChangesAsync();

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
                Console.WriteLine(ex);
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

        [HttpPost("{id}/doSent")]
        public IActionResult doSent([FromForm] doSent value)
        {
            try
            {
                tpOutgoingLetter.UserSender = value.SenderId;
                tpOutgoingLetter.ReceiptDt = value.ReceiptDt;
                tpOutgoingLetter.ReceiptBy = value.ReceiptBy;
                tpOutgoingLetter.Status = 5;

                if (value.File != null && value.File.Length > 0)
                {
                    Directory.CreateDirectory(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.OutgoingLetter, isProduction: _env.IsProduction()));
                    string directoryPath = Path.Combine(DocumentDropper.SetDirectory(directory: ModuleDirectory.Closing, action: HRDModuleAction.OutgoingLetter, isProduction: _env.IsProduction()), value.LetterId);

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    IFormFile file = value.File;
                    string fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(directoryPath, fileName);

                    // Save file
                    using (var stream = new FileStream(path: filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        fileName = filePath;
                    }

                    tpOutgoingLetter.FileName = fileName;
                    tpOutgoingLetter.Type = value.File.ContentType;
                    tpOutgoingLetter.Size = 0;
                    tpOutgoingLetter.DocumentStatus = 1;
                }

                var doSent = dbContext.TpOutgoingLetters.FirstOrDefault(x => x.LetterId == value.LetterId);
                if (doSent != null)
                {
                    doSent.UserSender = value.SenderId;
                    doSent.ReceiptDt = value.ReceiptDt;
                    doSent.ReceiptBy = value.ReceiptBy;
                    doSent.Status = 5;

                    dbContext.SaveChanges();
                }

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

        [HttpPost("{id}/doDelete")]
        public IActionResult doDelete([FromBody] doDelete value, string id = "USR-201710052")
        {
            try
            {
                var findSessionData = userDB.FindSessionDataUser(id);
                string userId = findSessionData[0].UserId;

                tpOutgoingLetter.Status = 0;
                tpOutgoingLetter.DtUpdate = DateTime.Now;
                tpOutgoingLetter.UserUpdate = userId;

                var doDelete = dbContext.TpOutgoingLetters.FirstOrDefault(x => x.LetterId == value.letterId);
                if (doDelete != null)
                {
                    doDelete.Status = 0;
                    doDelete.DtUpdate = DateTime.Now;
                    doDelete.UserUpdate = userId;

                    dbContext.SaveChanges();
                }

                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("message", "Data berhasil.");
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

        [HttpPost("{id}/deletedocument")]
        public IActionResult DeleteDocument([FromBody] deleteDocumentOut value)
        {
            try
            {
                var deleteDocumentOut = dbContext.TpOutgoingLetters.FirstOrDefault(e => e.LetterId == value.Id);
                if (deleteDocumentOut != null)
                {
                    deleteDocumentOut.DocumentStatus = 0;
                    deleteDocumentOut.FileName = "";
                    deleteDocumentOut.Size = 0;
                    deleteDocumentOut.Type = "";

                    dbContext.SaveChanges();
                }

                var filePath = Path.Combine($"assets/images/letter/", value.FileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("message", "File berhasil dihapus.");
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

    }
}
