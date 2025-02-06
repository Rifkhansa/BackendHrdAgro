using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using BackendHrdAgro.Models.Master;

namespace BackendHrdAgro.Controllers.Master
{
    [Route("api/menu")]
    [ApiController]
    public class Menu : ControllerBase
    {
        private readonly ILogger<Menu> _logger;
        private readonly IWebHostEnvironment _env;
        public Menu(ILogger<Menu> logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }

        BaseModel baseModel = new BaseModel();
        MenuDB menuDB = new MenuDB();
        ErrorCodes errorCodes = new ErrorCodes();
        ErrorMessege errorMessege = new ErrorMessege();
        Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> detail = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> resp = new Dictionary<string, dynamic>();
        List<dynamic> listResp = new List<dynamic>();
        List<dynamic> listData = new List<dynamic>();

        [HttpGet]
        public ActionResult index()
        {
            try
            {
                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                detail.Add("menus", menuDB.MenuFindAll());
                detail.Add("parents", menuDB.MenuFindParent());

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

        [HttpPost("{id}/create")]
        public ActionResult post([FromBody]TmMenu value, string id)
        {
            try
            {
                value.Id = BaseModel.GenerateId(tableName:"tm_menu", str:"MNU-", primaryKey: "Id", trailing: 4, lastKey: "NONE", date: DateTime.Now.ToString("yyMM"));
                value.CreatedBy = id;
                value.CreatedAt = DateTime.Now;

                menuDB.menuCreate(value);

                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                detail.Add("datas", value);

                listData.Add(detail);

                data.Add("response", listResp);
                data.Add("message", "Berhasil membuat menu");
                data.Add("data", listData);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: HttpContext.Request.Path, exception: ex);
                resp.Add("code", errorCodes.BadRequest);
                resp.Add("message", errorMessege.BadRequest);
                listResp.Add(resp);

                detail.Add("datas", value);

                listData.Add(detail);

                data.Add("response", listResp);
                data.Add("message", ex.Message);
                data.Add("data", listData);
                return BadRequest(data);
            }
        }

        [HttpPost("{id}/update")]
        public ActionResult update([FromBody]TmMenu value, string id)
        {
            try
            {
                value.UpdatedBy = id;
                value.UpdatedAt = DateTime.Now;

                menuDB.menuUpdate(value);

                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                detail.Add("datas", value);

                listData.Add(detail);

                data.Add("response", listResp);
                data.Add("message", "Berhasil mengubah menu");
                data.Add("data", listData);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: HttpContext.Request.Path, exception: ex);
                resp.Add("code", errorCodes.BadRequest);
                resp.Add("message", errorMessege.BadRequest);
                listResp.Add(resp);

                detail.Add("datas", value);

                listData.Add(detail);

                data.Add("response", listResp);
                data.Add("message", ex.Message);
                data.Add("data", listData);
                return BadRequest(data);
            }
        }

        [HttpPost("{id}/delete")]
        public ActionResult delete([FromBody] DeleteMenuModel value, string id)
        {
            try
            {
                value.DeletedAt = DateTime.Now;
                value.DeletedBy = id;

                menuDB.menuDelete(value);

                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                detail.Add("datas", value);

                listData.Add(detail);

                data.Add("response", listResp);
                data.Add("message", "Berhasil mengubah menu");
                data.Add("data", listData);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: HttpContext.Request.Path, exception: ex);
                resp.Add("code", errorCodes.BadRequest);
                resp.Add("message", errorMessege.BadRequest);
                listResp.Add(resp);

                detail.Add("datas", value);

                listData.Add(detail);

                data.Add("response", listResp);
                data.Add("message", ex.Message);
                data.Add("data", listData);
                return BadRequest(data);
            }
        }
    }
}
