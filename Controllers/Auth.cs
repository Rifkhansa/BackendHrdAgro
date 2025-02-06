using BackendHrdAgro.Models;
using BackendHrdAgro.Models.Database.MySql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace BackendHrdAgro.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class Auth : Controller
    {
        private readonly ILogger<Auth> _logger;
        private readonly IWebHostEnvironment _env;
        public Auth(ILogger<Auth> logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }

        ErrorCodes errorCodes = new ErrorCodes();
        ErrorMessege errorMessege = new ErrorMessege();
        Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> detail = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> resp = new Dictionary<string, dynamic>();
        List<dynamic> listResp = new List<dynamic>();
        List<dynamic> listData = new List<dynamic>();

        [HttpPost("login")]
        public ActionResult Login([FromBody] AuthLoginModel value)
        {
            value.EmployeeId = Regex.Replace(value.EmployeeId, @"[^a-zA-Z0-9_.]+", string.Empty);
            var unhasehedPassword = value.Password;
            value.Password = BaseModel.HashedPassword(value.Password);
            try
            {
                if (AuthDB.IsUserExist(EmployeeId: value.EmployeeId))
                {
                    if (AuthDB.AuthLogin(value))
                    {
                        resp.Add("code", errorCodes.Ok);
                        resp.Add("message", errorMessege.Ok);
                        listResp.Add(resp);

                        data.Add("response", listResp);
                        data.Add("data", AuthDB.GetUserInformation(employeeId: value.EmployeeId)!);
                        data.Add("changePassword", AuthDB.IsUserNeedToChangePassword(employeeId: value.EmployeeId, password: unhasehedPassword));
                        Console.WriteLine(value.EmployeeId);
                        Console.WriteLine(value.Password);
                        data.Add("message", "");
                        return Ok(data);
                    }
                    else
                    {
                        resp.Add("code", errorCodes.NotFound);
                        resp.Add("message", errorMessege.NotFound);
                        listResp.Add(resp);

                        data.Add("response", listResp);
                        data.Add("data", null);
                        data.Add("message", "Invalid Password");
                        return BadRequest(data);
                    }
                }
                else
                {
                    resp.Add("code", errorCodes.NotFound);
                    resp.Add("message", errorMessege.NotFound);
                    listResp.Add(resp);

                    data.Add("response", listResp);
                    data.Add("data", null);
                    data.Add("message", "Invalid Username");
                    return BadRequest(data);
                }

            }
            catch (Exception ex)
            {
                resp.Clear();
                data.Clear();
                listResp.Clear();

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

        [HttpGet("retrive")]
        public IActionResult Retrive()
        {
            try
            {
                var data = new DatabaseContext().TmEmployeeAffairs.Take(5).ToList();
                return Ok(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("entrance")]
        public IActionResult Entrance([FromQuery] AuthEntranceModel value)
        {
            try
            {
                if (!AuthDB.IsUserHasRole(user: new BaseModel().UrlEncoder(str: value.U))) throw new Exception("User doesn't have access to this menu");
                if (!AuthDB.IsRoleHasMenu(role: new BaseModel().UrlEncoder(str: value.R), menu: new BaseModel().UrlEncoder(str: value.M))) throw new Exception("User doesn't have access to this menu");

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(message: HttpContext.Request.Path, exception: e);
                return BadRequest(e.Message);
            }
        }
    }
}
