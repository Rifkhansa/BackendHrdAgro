using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Master;
using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Mvc;
using BackendHrdAgro.Models.Layout;

namespace BackendHrdAgro.Controllers.Layout
{
    [Route("api/incomingLetter")]
    [ApiController]
    public class IncomingController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<IncomingController> _logger;

        public IncomingController(IWebHostEnvironment env, ILogger<IncomingController> logger)
        {
            _env = env;
            _logger = logger;
        }

        readonly ErrorCodes ErrorCode = new ErrorCodes();
        readonly ErrorMessege ErrorMessege = new ErrorMessege();
        UserDB userDB = new UserDB();
        Dictionary<string, dynamic> Data = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> Responses = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> Detail = new Dictionary<string, dynamic>();
        List<dynamic> ListResponse = new List<dynamic>();
        List<dynamic> ListData = new List<dynamic>();
        DatabaseContext dbContext = new DatabaseContext();
        IncomingLetterModel incomingLetterModel = new IncomingLetterModel();
        TpIncomingLetter tpIncomingLetter = new TpIncomingLetter();

        [HttpGet("{id}")]
        public IActionResult Get()
        {
            try
            {
                var letter = incomingLetterModel.letters();
                var destination = incomingLetterModel.destinations();
                var receiptby = incomingLetterModel.receipts();

                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("letter", letter);
                Detail.Add("destination", destination);
                Detail.Add("receiptby", receiptby);

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
        public ActionResult Create([FromBody] createIncoming value, string id = "USR-201710052")
        {
            var findSessionData = userDB.FindSessionDataUser(id);
            var userId = findSessionData[0].UserId;

            var letterId = BaseModel.GenerateId(
                tableName: "tp_incoming_letter",
                primaryKey: "letter_id",
                str: "LET" + DateTime.Now.ToString("yyyyMM"),
                trailing: 4,
                lastKey: "NONE");
            Console.WriteLine(letterId);
            tpIncomingLetter.LetterId = letterId;
            tpIncomingLetter.LetterDate = value.LetterDate;
            tpIncomingLetter.Sender = value.Sender;
            tpIncomingLetter.Subject = value.Subject;
            tpIncomingLetter.DestinationId = value.DestinationId;
            tpIncomingLetter.ReceiptId = value.ReceiptId;
            tpIncomingLetter.Remark = value.Description;

            tpIncomingLetter.Status = 1;
            tpIncomingLetter.IsAccepted = 0;

            tpIncomingLetter.DtEtr = DateTime.Now;
            tpIncomingLetter.UserEtr = userId;
            tpIncomingLetter.DtUpdate = DateTime.Now;
            tpIncomingLetter.UserUpdate = userId;

            List<Dictionary<string, dynamic>> create = incomingLetterModel.CreateIncomingLetter(tpIncomingLetter);

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

        [HttpPost("{id}/update")]
        public IActionResult Update([FromBody] updateIncoming value, string id = "USR-201710052")
        {
            try
            {
                var findSessionData = userDB.FindSessionDataUser(id);
                var userId = findSessionData[0].UserId;

                var updateIncoming = dbContext.TpIncomingLetters.FirstOrDefault(x => x.LetterId == value.LetterId);
                if (updateIncoming != null)
                {
                    updateIncoming.Sender = value.Sender;
                    updateIncoming.Subject = value.Subject;
                    updateIncoming.DestinationId = value.DestinationId;
                    updateIncoming.ReceiptId = value.ReceiptId;
                    updateIncoming.Remark = value.Description;

                    updateIncoming.DtUpdate = DateTime.Now;
                    updateIncoming.UserUpdate = id;

                    dbContext.SaveChanges();
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

        [HttpPost("{id}/doDelete")]
        public IActionResult doDelete([FromBody] doDeleteIn value, string id)
        {
            try
            {
                var doDelete = dbContext.TpIncomingLetters.FirstOrDefault(x => x.LetterId == value.letterId);
                if (doDelete != null)
                {
                    doDelete.Status = 0;
                    doDelete.Remark = value.remark;
                    doDelete.DtUpdate = DateTime.Now;
                    doDelete.UserUpdate = id;

                    dbContext.SaveChanges();
                }

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

        [HttpPost("{id}/close")]
        public ActionResult close([FromBody] closedIn value)
        {
            try
            {
                var Update = dbContext.TpIncomingLetters.FirstOrDefault(x => x.LetterId == value.LetterId);
                if (Update != null)
                {
                    Update.AcceptedDt = DateTime.Now;
                    Update.IsAccepted = 1;

                    dbContext.SaveChanges();
                }

                Responses.Add("code", ErrorCode.Ok);
                Responses.Add("message", ErrorMessege.Ok);
                ListResponse.Add(Responses);

                Detail.Add("datas", value);
                ListData.Add(Detail);

                Data.Add("response", ListResponse);
                Data.Add("message", "");
                Data.Add("data", ListData);

                return Ok(Data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

    }
}
