using BackendHrdAgro.Models.Master;
using BackendHrdAgro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendHrdAgro.Controllers.Master
{
    [Route("api/role")]
    [ApiController]
    public class Role : ControllerBase
    {
        private readonly ILogger<Role> _logger;
        private readonly IWebHostEnvironment _env;
        public Role(ILogger<Role> logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }

        BaseModel baseModel = new BaseModel();
        MenuDB menuDB = new MenuDB();
        RolesDB roleDB = new RolesDB();
        ErrorCodes errorCodes = new ErrorCodes();
        ErrorMessege errorMessege = new ErrorMessege();
        Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> detail = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> resp = new Dictionary<string, dynamic>();
        List<dynamic> listResp = new List<dynamic>();
        List<dynamic> listData = new List<dynamic>();

        [HttpGet]
        public ActionResult get()
        {
            try
            {
                var findAllMenu = menuDB.MenuFindAll();
                var findRoles = roleDB.RoleFind();
                foreach (var i in findRoles)
                {
                    List<string> menuId = new List<string>();
                    var roleMenuId = roleDB.RoleAllUserRole(i.Id);
                        Console.WriteLine(i.Id);
                    foreach (var j in roleMenuId)
                    {
                        Console.WriteLine(j.MenuId);
                        menuId.Add(j.MenuId);
                    }

                    i.menu_ids = menuId.ToArray();
                }

                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                detail.Add("menus", findAllMenu);
                detail.Add("role", findRoles);
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

        [HttpPost("{id}/create")]
        public ActionResult create([FromBody]RoleCreateModel value, string id)
        {


            TmRole createRoleModel = new TmRole()
            {
                Id = BaseModel.GenerateId(tableName: "tm_role", str: "ROL", primaryKey: "id", trailing: 4, lastKey: "NONE"),
                Name = value.Name,
                Description = value.Description,
                CreatedAt = DateTime.Now,
                CreatedBy = id
            };

            List<Dictionary<string, dynamic>> create = roleDB.CreateRole(createRoleModel); //fauzan menambahkan

            value.Id = createRoleModel.Id;


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


        [HttpPost("{id}/update")]
        public ActionResult update([FromBody] RoleCreateModel value, string id)
        {

            List<Dictionary<string, dynamic>> update = roleDB.UpdateRole(value, id);

            bool myResult = update[0]["result"];
            var myMessage = update[0]["message"];

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

        [HttpPost("{id}/delete")]
        public ActionResult delete([FromBody]RoleIdModel value, string id)
        {

            List<Dictionary<string, dynamic>> delete = roleDB.DeleteRole(value, id);

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

        [HttpPost("{id}/change-permission")]
        public async Task<ActionResult> changePermission([FromBody]RoleChangePermissionRoleModel value, string id)
        {
            var getRoleId = roleDB.RoleFind(value.RoleId) ?? throw new Exception("Invalid data");
            var roleMenuId = roleDB.RoleAllUserRole(value.RoleId);
            List<string> menuIds = new List<string>();
            List<string> insert = new List<string>();
            List<string> delete = new List<string>();

            foreach (var i in roleMenuId)
            {
                menuIds.Add(i.MenuId);
            }
            foreach (var role in value.Permission)
            {
                if (!menuIds.Contains(role))
                {
                    insert.Add(role);
                }
            }

            foreach (var role in menuIds)
            {
                if (!value.Permission.Contains(role))
                {
                    delete.Add(role);
                }
            }

            List<Dictionary<string, dynamic>> changePermission = roleDB.ChangePermission(insert, delete, value.RoleId, id);

            // bool myResult = false;
            // var myMessage = "";
            //foreach (var item in changePermission)
            //{
            //    myResult = item["result"];
            //    myMessage = item["message"];
            //}

            bool myResult = changePermission[0]["result"];
            var myMessage = changePermission[0]["message"];
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
