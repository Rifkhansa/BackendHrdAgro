using BackendHrdAgro.Models.Master;
using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Mvc;
using NPOI.OpenXmlFormats.Spreadsheet;
using BackendHrdAgro.Models.Database.MySql;
using Newtonsoft.Json;

namespace BackendHrdAgro.Controllers.Master
{
    [Route("api/master/user")]
    [ApiController]
    public class User : ControllerBase
    {
        private readonly ILogger<User> _logger;
        private readonly IWebHostEnvironment _env;
        public User(ILogger<User> logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }

        BaseModel baseModel = new BaseModel();
        UsersDB usersDB = new UsersDB();
        MenuDB menuDB = new MenuDB();
        RolesDB roleDB = new RolesDB();
        ErrorCodes errorCodes = new ErrorCodes();
        ErrorMessege errorMessege = new ErrorMessege();
        Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> detail = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> resp = new Dictionary<string, dynamic>();
        List<dynamic> listResp = new List<dynamic>();
        List<dynamic> listData = new List<dynamic>();


        [HttpGet("{id}")]
        public ActionResult get(string id)
        {
            try
            {

                using DatabaseContext context = new DatabaseContext();
                var usersrole = (from user in context.TmUsers
                                 join employe in context.TmEmployeeAffairs on user.EmployeeId equals employe.EmployeeId
                                 select new EmployeeRoles
                                 {
                                     EmployeeId = user.EmployeeId,
                                     EmployeeFirstName = employe.EmployeeFirstName,
                                     EmployeeLastName = employe.EmployeeLastName,
                                     EmployeeName = $"{employe.EmployeeFirstName} {employe.EmployeeLastName}"
                                 }).ToList();
                usersrole.ForEach(item =>
                {
                    var roles = (from user in context.TmUsers
                                 join userRole in context.TpUserRoles on user.UserId equals userRole.UserId into userRoleGroup
                                 from userRole in userRoleGroup.DefaultIfEmpty()
                                 join role in context.TmRoles on userRole.RoleId equals role.Id into roleGroup
                                 from role in roleGroup.DefaultIfEmpty()
                                 where user.EmployeeId == item.EmployeeId
                                 select new
                                 {
                                     RoleId = userRole.RoleId,
                                     RoleName = role.Name,
                                     RoleDescription = role.Description
                                 }).ToList();
                    item.RoleId = roles.Select(x => x.RoleId).ToList();
                    item.RoleName = roles.Select(x => x.RoleName).ToList();
                    item.RoleDescription = roles.Select(x => x.RoleDescription).ToList();

                });

                var roles = context.TmRoles.Where(x => !x.DeletedAt.HasValue).Select(x => new
                {
                    RoleId = x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToList();

                context.Dispose();


                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                detail.Add("userRole", usersrole);
                detail.Add("roles", roles);

                listData.Add(detail);

                data.Add("response", listResp);
                data.Add("data", listData);
                data.Add("message", "");
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

        [HttpPost("{id}/update")]
        public IActionResult Update([FromBody] EmployeeRoleUpdate value, string id)
        {
            try
            {
                using DatabaseContext context = new DatabaseContext();
                context.ChangeTracker.AutoDetectChangesEnabled = false;
                var users = context.TmUsers.Where(x => x.EmployeeId.Equals(value.EmployeeId)).FirstOrDefault() ?? throw new Exception("Data karyawan tidak ditemukan", innerException: new Exception($"Data karyawan dengan NIK {value.EmployeeId} tidak ditemukan"));
                var roles = context.TmRoles.Where(x => value.RoleId.Contains(x.Id)).ToList();
                Console.WriteLine(roles.Count);
                if (!roles.Any()) throw new Exception("Pastikan role yang dipilih tersedia di menu master role", innerException: new Exception($"Role tidak ditemukan didalam database atau tidak aktif, {JsonConvert.SerializeObject(value.RoleId)}"));
                bool userHasRole = false;
                if (context.TpUserRoles.Where(x => x.UserId.Equals(users.UserId)).Any())
                {
                    var userRoles = context.TpUserRoles.Where(x => x.UserId.Equals(users.UserId)).ToList();
                    userRoles.ForEach(x =>
                    {
                        context.TpUserRoles.Remove(x);
                    });
                    //context.SaveChanges();
                    userHasRole = true;
                }
                if(!context.TpUserLocIds.Where(x=> x.UserId.Equals(users.UserId)).Any())
                {
                    context.TpUserLocIds.Add(new TpUserLocId
                    {
                        LocId = "3RDP-22080005",
                        UserId = users.UserId,
                        CreatedBy = id,
                        UpdatedBy = id,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                        
                    });
                }

                value.RoleId.ForEach(async role =>
                {
                    context.TpUserRoles.Add(new TpUserRole
                    {
                        UserId = users.UserId,
                        RoleId = role,
                        CreatedAt = DateTime.Now,
                        CreatedBy = id,
                        UpdatedAt = userHasRole ? DateTime.Now : null,
                        UpdatedBy = userHasRole ? id : null
                    });
                    await Task.Delay(TimeSpan.FromSeconds(1));
                });

                context.SaveChanges();

                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                data.Add("response", listResp);
                data.Add("data", listData);
                data.Add("message", $"Berhasil menambahkan role {string.Join(",", roles.Select(x => x.Name).ToList())} ke akun {users.UserName}");
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

    }

#nullable disable
    public class EmployeeRoles
    {
        public string EmployeeId { get; set; }
        public List<string> RoleId { get; set; }
        public List<string> RoleName { get; set; }
        public List<string> RoleDescription { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
    }

#nullable disable
    public class EmployeeRoleUpdate
    {
        public string EmployeeId { get; set; }
        public List<string> RoleId { get; set; }
    }
}