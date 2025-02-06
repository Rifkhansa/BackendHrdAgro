using System.ComponentModel.DataAnnotations;
using BackendHrdAgro.Models.Database.MySql;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace BackendHrdAgro.Models
{
    public class AuthDB
    {
        DatabaseContext context = new DatabaseContext();
        public static bool AuthLogin(AuthLoginModel value)
        {
            using DatabaseContext context = new DatabaseContext();
            if (context.TmUsers.Where(e => e.EmployeeId == value.EmployeeId && e.Password == value.Password).Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsUserExist(string EmployeeId) => new DatabaseContext().TmUsers.Where(x => x.EmployeeId.Equals(EmployeeId) && x.Status.Equals(1)).Any();

        public static bool IsUserHasRole(string user) => new DatabaseContext().TpUserRoles.Where(x=> x.UserId == user).Any();
        public static bool IsRoleHasMenu(string role,string menu) => new DatabaseContext().TpRoleMenus.Where(x=> x.RoleId == role && x.MenuId == menu).Any();

        public static bool IsUserNeedToChangePassword(string employeeId, string password) => employeeId.Equals(password);

        public static LoginResponse? GetUserInformation(string employeeId)
        {
            using DatabaseContext context = new DatabaseContext();
            try
            {
                var query = from a in context.TmUsers
                            join b in context.TmEmployeeAffairs on a.EmployeeId equals b.EmployeeId
                            join c in context.TmDepartments on b.DepartmentId equals c.DepartmentId
                            join d in context.TmTitles on b.TitleId equals d.TitleId
                            join f in context.TmSisaCutis on a.EmployeeId equals f.EmployeeId into sisacutis
                            from f in sisacutis.DefaultIfEmpty()
                            join i in context.TpUserRoles on a.UserId equals i.UserId
                            join j in context.TpUserLocIds on a.UserId equals j.UserId
                            join e in (
                                        from empDoc in context.TpEmployeeAffairDocuments
                                        where empDoc.DocumentId == "1" && empDoc.ClaimTypeId == "CT000"
                                        select new { empDoc.EmployeeId, empDoc.FileName, empDoc.DocumentStatus }
                                      ) on a.EmployeeId equals e.EmployeeId
                            where a.EmployeeId == employeeId
                            select new LoginResponse
                            {
                                UserData = a,
                                EmployeeId = a.EmployeeId,
                                EmployeeFirstName = b.EmployeeFirstName!,
                                EmployeeLastName = b.EmployeeLastName!,
                                DepartmentId = b.DepartmentId,
                                TitleId = b.TitleId,
                                DepartmentName = c.DepartmentName,
                                TitleName = d.TitleName,
                                IsCutAbsenteeDes = d.IsCutAbsentee,
                                IsOvertimeDes = d.IsOvertime,
                                MyFoto = e.FileName,
                                SisaCutiAnnual = f == null ? 0 : f.SisaCutiAnnual,
                                SisaCutiMaternity = f == null ? 0 : f.SisaCutiMaternity,
                                SisaCutiLong = f == null ? 0 : f.SisaCutiLong,
                                GenderId = b.GenderId,
                                DivId = c.DivId,
                                LevelId = b.LevelId,
                                DocumentStatus = e.DocumentStatus,
                                LastChangePasswordNew = a.LastChangePassword,
                                RoleId = i.RoleId,
                                LocId = j.LocId,
                                FileName = e.FileName,
                            };

                var menus = from a in context.TpRoleMenus
                            join b in context.TmMenus on a.MenuId equals b.Id
                            join c in context.TpUserRoles on a.RoleId equals c.RoleId
                            join d in context.TmUsers on c.UserId equals d.UserId
                            where d.EmployeeId == employeeId && b.Parent == null
                            orderby b.Hierarchy
                            select new 
                            {
                                b.Name
                            };

                var result = query.FirstOrDefault() ?? throw new Exception("Cannot retrive user detail");
                result.Menus = menus.Select(x => x.Name).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
    public class AuthLoginModel
    {
        [Required]
        public string EmployeeId { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class AuthEntranceModel
    {
        [Required]
        public string R { get; set; }
        [Required]
        public string M { get; set; }
        [Required]
        public string U { get; set; }
    }

    public class LoginResponse
    {
        public TmUser UserData { get; set; } = null!;
        public string EmployeeId { get; set; } = null!;
        public string EmployeeFirstName { get; set; } = null!;
        public string EmployeeLastName { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public string TitleId { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public string TitleName { get; set; } = null!;
        public int IsCutAbsenteeDes { get; set; }
        public int IsOvertimeDes { get; set; }
        public string MyFoto { get; set; } = null!;
        public double SisaCutiAnnual { get; set; }
        public double SisaCutiMaternity { get; set; }
        public double SisaCutiLong { get; set; }
        public string GenderId { get; set; } = null!;
        public string DivId { get; set; } = null!;
        public string LevelId { get; set; } = null!;
        public string Latitude { get; set; } = null!;
        public string Longitude { get; set; } = null!;
        public byte DocumentStatus { get; set; }
        public DateTime? LastChangePasswordNew { get; set; }
        public string RoleId { get; set; } = null!;
        public string LocId { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public List<string> Menus { get; set; } = null!;
    }
}
