using BackendHrdAgro.Models.Master;
using BackendHrdAgro.Models;
using BackendHrdAgro.Models;
using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Master;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.RegularExpressions;

namespace BackendHrdAgro.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class Dashboard : ControllerBase
    {
        private readonly ILogger<Dashboard> _logger;
        private readonly IWebHostEnvironment _env;
        public Dashboard(ILogger<Dashboard> logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }

        DashboardDB dashboardDB = new DashboardDB();
        BaseModel baseModel = new BaseModel();
        UserDB userDB = new UserDB();
        ErrorCodes errorCodes = new ErrorCodes();
        ErrorMessege errorMessege = new ErrorMessege();
        Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> detail = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> resp = new Dictionary<string, dynamic>();
        List<dynamic> listResp = new List<dynamic>();
        List<dynamic> listData = new List<dynamic>();

        [HttpPost("{id}/show-menu")]
        public async Task<ActionResult> dashboard([FromBody] DashboardShowMenuModel value, string id)
        {

            try
            {
                using DatabaseContext context = new DatabaseContext();
                List<MainMenu> menu = new List<MainMenu>();

                var role = context.TpUserRoles.Where(x => x.UserId.Equals(id)).Select(x => x.RoleId).FirstOrDefault() ?? throw new Exception("Invalaid data");
                var allowedmenu = context.TpRoleMenus.Where(x => x.RoleId.Equals(role)).ToList();
                List<TmMenu?> menus = new List<TmMenu?>();
                allowedmenu.ForEach(e =>
                {
                    menus.Add(context.TmMenus.Where(x => x.Id.Equals(e.MenuId)).FirstOrDefault());
                });
                if (menus == null) throw new Exception("User does not have any access, please contact your administrator");
                var mainmenu = menus.Where(x => x!.DeletedAt == null && x!.DeletedBy == null && (x!.Parent == null || x!.Parent == "")).ToList();
                mainmenu.ForEach(x =>
                {
                    menu.Add(new MainMenu
                    {
                        Name = x.Name,
                        Caption = x.Caption,
                        Icon = x.Icon,
                        Index = x.Hierarchy,
                        Url = x.Url,
                        SubMenu = menus.Where(z => z.DeletedAt == null && z.DeletedBy == null && z.Parent == x.Id).Select(z => new SubMenu
                        {
                            Name = z.Name,
                            Caption = z.Caption,
                            Icon = z.Icon,
                            Index = z.Hierarchy,
                            Url = z.Url
                        }).ToList()
                    });
                });

                StringBuilder sidebarHtml = new StringBuilder();

                sidebarHtml.AppendLine("<nav class='sidebar-nav scroll-sidebar' id='menu-right-mini-1' data-simplebar>");
                sidebarHtml.AppendLine("<ul class='sidebar-menu' id='sidebarnav'>");

                foreach (var category in menu)
                {
                    //Console.WriteLine("Parent " + category.Name);
                    //sidebarHtml.AppendLine("<li class='nav-small-cap'>");
                    //sidebarHtml.AppendLine($"  <span class='hide-menu'>{category.Caption}</span>");
                    //sidebarHtml.AppendLine("</li>");

                    sidebarHtml.AppendLine("<li class='sidebar-item'>");
                    if (category.SubMenu.Count() == 0)
                    {
                        //sidebarHtml.AppendLine($"<a class='sidebar-link' href='../main/index.html' aria-expanded='false'>");
                        sidebarHtml.AppendLine($"<a class='sidebar-link' href='{category.Url}' aria-expanded='false'>");
                    }
                    else
                    {
                        sidebarHtml.AppendLine($"<a class='sidebar-link has-arrow' href='javascript:void(0)' aria-expanded='false'>");
                    }

                    //sidebarHtml.AppendLine($"<iconify-icon icon='solar:atom-line-duotone'></iconify-icon>");
                    //sidebarHtml.AppendLine($"<iconify-icon icon='{category.Icon}'></iconify-icon>");
                    sidebarHtml.AppendLine($"<i class='{category.Icon}' style='font-size:16pt;'></i>");
                    sidebarHtml.AppendLine($"<span class='hide-menu'>{category.Caption}</span>");
                    sidebarHtml.AppendLine("</a>");
                    if (category.SubMenu.Count() != 0)
                    {
                        sidebarHtml.AppendLine("<ul aria-expanded='false' class='collapse first-level'>");
                        foreach (var name in category.SubMenu)
                        {
                            Console.WriteLine("Child " + name.Name);
                            sidebarHtml.AppendLine("<li class='sidebar-item'>");
                            //sidebarHtml.AppendLine($"  <a class='sidebar-link' href='../main/index.html'>");
                            sidebarHtml.AppendLine($"  <a class='sidebar-link' href='{name.Url}'>");
                            sidebarHtml.AppendLine($"<span class='icon-small'></span>{name.Caption}");
                            sidebarHtml.AppendLine("</a>");
                            sidebarHtml.AppendLine("</li>");
                        }
                        sidebarHtml.AppendLine("</ul>");
                        sidebarHtml.AppendLine("</li>");
                    }
                    else
                    {
                        sidebarHtml.AppendLine("</li>");
                    }
                }

                sidebarHtml.AppendLine("</ul>");
                sidebarHtml.AppendLine("</nav>");

                string generatedHtml = sidebarHtml.ToString();

                TmMenu? setMenu = context.TmMenus.Where(x => x.Url == value.extendedUrl || x.Url == "/" + value.extendedUrl).FirstOrDefault();

                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                detail.Add("extendedUrl", value.extendedUrl);
                detail.Add("menuName", setMenu);
                detail.Add("parentName", mainmenu.Where(x => x.Id.Equals(setMenu.Parent)));
                detail.Add("menus", Regex.Replace(generatedHtml.Replace("\\\"", ""), @"\t|\n|\r", ""));

                listData.Add(detail);

                data.Add("response", listResp);
                data.Add("message", "");
                data.Add("data", listData);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: HttpContext.Request.Path, exception: ex);
                return BadRequest(ex.Message);
            }
        }


        //samsul
        [HttpGet("{id}/home")]
        public IActionResult Index(string id = "USR-201710052")
        {
            string kriteria = "";
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

                if (departmentId == "DP006" || employeeId == "0808003")
                { //untuk hrd
                    kriteria = "";

                }
                else
                {
                    kriteria = $"and d.department_id='{departmentId}'";
                }
                resp.Add("code", errorCodes.Ok);
                resp.Add("message", errorMessege.Ok);
                listResp.Add(resp);

                //
                if (AuthorizationFactory.Entrance(id: departmentId) || AuthorizationFactory.Entrance(id: employeeId))
                {
                    if (AuthorizationFactory.Exit(id: titleId))
                    {
                        // detail.Add(key: "Presence", dashboardDB.GetPresence(action: RetriveAction.EmployeeId, id: employeeId));
                        detail.Add(key: "Presence", dashboardDB.GetPresence(titleId, employeeId, departmentId, levelId, divId));
                    }
                    else
                    {
                        //  detail.Add(key: "Presence", dashboardDB.GetPresence(action: RetriveAction.All, id: string.Empty));
                        detail.Add(key: "Presence", dashboardDB.GetPresence(titleId, employeeId, departmentId, levelId, divId));
                       // detail.Add(key: "JumlahPunishment", dashboardDB.GetJumlahPunishment());
                        detail.Add(key: "JumlahApproval", dashboardDB.GetJumlahApproval(departmentId));
                        detail.Add(key: "JumlahTermination", dashboardDB.GetJumlahTermination());
                        detail.Add(key: "JumlahApprovalCuti", dashboardDB.GetJumlahApprovalCuti(departmentId, employeeId));
                       // detail.Add(key: "JumlahApprovalPermission", dashboardDB.GetJumlahApprovalPermission(departmentId));
                        detail.Add(key: "JumlahExtend", dashboardDB.GetExtendAmount());
                    }
                }
                else
                {
                    if (AuthorizationFactory.Entrance(id: titleId) && AuthorizationFactory.Entrance(id: levelId))
                    {
                        Console.WriteLine("if1");
                        // detail.Add(key: "Presence", dashboardDB.GetPresence(action: RetriveAction.DivisionId, id: divId));
                        detail.Add(key: "Presence", dashboardDB.GetPresence(titleId, employeeId, departmentId, levelId, divId));
                       //detail.Add(key: "JumlahPunishment", dashboardDB.GetJumlahPunishment());
                        detail.Add(key: "JumlahApproval", dashboardDB.GetJumlahApproval(departmentId));
                        detail.Add(key: "JumlahTermination", dashboardDB.GetJumlahTermination());
                        detail.Add(key: "JumlahApprovalCuti", dashboardDB.GetJumlahApprovalCuti(departmentId, employeeId));
                        //detail.Add(key: "JumlahApprovalPermission", dashboardDB.GetJumlahApprovalPermission(departmentId));
                        detail.Add(key: "JumlahExtend", dashboardDB.GetExtendAmount());
                    }
                    else if (AuthorizationFactory.Entrance(id: titleId))
                    {
                        Console.WriteLine("if2");

                        //detail.Add(key: "Presence", dashboardDB.GetPresence(action: RetriveAction.DepartementId, id: departmentId));
                        detail.Add(key: "Presence", dashboardDB.GetPresence(titleId, employeeId, departmentId, levelId, divId));
                        //    detail.Add(key: "JumlahPunishment", dashboardDB.GetJumlahPunishment());
                        detail.Add(key: "JumlahApproval", dashboardDB.GetJumlahApproval(departmentId));
                        //detail.Add(key: "JumlahTermination", dashboardDB.GetJumlahTermination());
                        detail.Add(key: "JumlahApprovalCuti", dashboardDB.GetJumlahApprovalCuti(departmentId, employeeId));
                        //detail.Add(key: "JumlahApprovalPermission", dashboardDB.GetJumlahApprovalPermission(departmentId));
                        //   detail.Add(key: "JumlahExtend", dashboardDB.GetExtendAmount());
                        if (departmentId == "DP007")
                        {
                            detail.Add(key: "JumlahRequestIT", dashboardDB.GetJumlahRequestIT(employeeId, departmentId));
                        }
                    }
                    else
                    {
                        //detail.Add(key: "Presence", dashboardDB.GetPresence(action: RetriveAction.EmployeeId, id: employeeId));

                        detail.Add(key: "Presence", dashboardDB.GetPresence(titleId, employeeId, departmentId, levelId, divId));
                        detail.Add(key: "JumlahRequestCuti", dashboardDB.GetJumlahRequestCuti(employeeId));
                        detail.Add(key: "JumlahRequest", dashboardDB.GetJumlahRequest(employeeId));

                        if (departmentId == "DP007")
                        {
                            detail.Add(key: "JumlahRequestIT", dashboardDB.GetJumlahRequestIT(employeeId, departmentId));
                        }

                    }
                }
                //

                detail.Add(key: "JumlahIncomingLetters", dashboardDB.GetIncomingAmount(employeeId));
                detail.Add(key: "JumlahCutiLong", dashboardDB.GetJumlahCutiLong(employeeId));
                detail.Add(key: "JumlahCutiAnnual", dashboardDB.GetJumlahCutiAnnual(employeeId));
                detail.Add(key: "JumlahApprovalCutiDiv", dashboardDB.GetJumlahApprovalCutiDiv(employeeId, departmentId));
                detail.Add(key: "JumlahApprovalDiv", dashboardDB.GetJumlahApprovalDiv(employeeId, departmentId));


                detail.Add(key: "JumlahUltah", dashboardDB.GetJumlahUltah(employeeId));
                detail.Add(key: "PersentasePresent", dashboardDB.GetPresentTotal(employeeId));
                //detail.Add(key: "PersentasePermission", dashboardDB.GetPermissionTotal(employeeId));
                detail.Add(key: "PersentaseAbsent", dashboardDB.GetAbsentTotal(employeeId));



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
    }
}
