using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using BackendHrdAgro.Models.Database.MySql;

namespace BackendHrdAgro.Models.Master
{
    public class UsersDB
    {
        public Dictionary<string, List<string>> userRole(string userId)
        {
            using DatabaseContext context = new DatabaseContext();
            var data = context.TpUserRoles.Where(x => x.UserId == userId).Select(x => x.RoleId).ToList();
            if (data.Count < 1) return null;
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var role in data)
            {
                result.Add(role, roleMenu(role));
            }
            return result;
        }

        public List<string> roleMenu(string roleId)
        {
            using DatabaseContext context = new DatabaseContext();
            var data = (from role in context.TpRoleMenus
                        join menu in context.TmMenus on role.MenuId equals menu.Id
                        where role.RoleId == roleId
                        select menu.Name).ToList();
            if (data.Count < 1) return null;
            var result = new List<string>();
            return data;
        }
        // Rifdah
       /* public List<emailDb> emailDbs(lostPass value)
        {
            var reset = new DatabaseContext().EmailDbs.FromSqlRaw($"select email as email_database from tm_users where employee_id = {value.employeeId}").ToList();
            return reset;
        }*/
        public List<employeeDepart> employeeDeparts(IndexChangePassword value)
        {
            var query = new DatabaseContext().EmployeeDeparts.FromSqlRaw("select employee_first_name,employee_last_name,department_name\r\n      from tm_employee_affair a\r\n      " +
                $"inner join tm_department b on a.department_id=b.department_id\r\n      where employee_id = {value.employeeId}").ToList();
            return query;
        }
        public List<password> password(ChangePass value)
        {
            var qPassword = new DatabaseContext().Passwords.FromSqlRaw($"select password  from tm_users a where employee_id = '{value.EmployeeId}'").ToList();
            return qPassword;
        }
        public List<TotalChangePass> TotalChange(ChangePass value)
        {
            var queryTotal = new DatabaseContext().TotalChangePasses.FromSqlRaw($"select count(employee_id) as jumlah_change_password from tp_history_change_password where employee_id = {value.EmployeeId}").ToList();
            return queryTotal;
        }
        public List<historyId> historyIds(ChangePass value)
        {
            var id = new DatabaseContext().HistoryIds.FromSqlRaw("select id_history_password as id_terlama from tp_history_change_password " +
                $"where employee_id = {value.EmployeeId} order by date_change_password asc limit 1").ToList();
            return id;
        }

        //End
    }

    public class UserCreateModel
    {
        public string? id { get; set; }
        [Required]
        public string fullName { get; set; }
        [Required]
        public string name { get; set; }
        public string? email { get; set; }
        public string? groupId { get; set; }
        [Required]
        public string? locId { get; set; }
        [Required]
        public int isLocked { get; set; }
        [Required]
        public string? RoleId { get; set; }
    }
    public class UserDeleteModel
    {
        [Required]
        public string Id { get; set; }
    }
    public class ChangePasswordMasterUser
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }

    // Rifdah
    public class lostPass
    {
        [Required]
        public string employeeId { get; set; }
    }
    public class IndexChangePassword
    {
        [Required]
        public string employeeId { get; set; }
    }
    public class ChangePass
    {
        public string EmployeeId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfrimPassword { get; set; } 
    }

 
    

    [Keyless]
    public class emailDb
    {
        [Column("email_database")]
        public string EmailDatabase { get; set; }
    }
    [Keyless]
    public class employeeDepart
    {
        [Column("employee_first_name")]
        public string EmployeeFirstName { get; set; }

        [Column("employee_last_name")]
        public string EmployeeLastName { get; set; }

        [Column("department_name")]
        public string DepartmentName { get; set; }
    }
    [Keyless]
    public class TotalChangePass
    {
        [Column("jumlah_change_password")]
        public int JumlahChangePassword { get; set; }
    }
    [Keyless]
    public class historyId
    {
        [Column("id_terlama")]
        public int HistoryId { get; set; }
    }
    [Keyless]
    public class password
    {
        [Column("password")]
        public string Password { get; set; }
    }

    //End


    [Keyless]
    public partial class UserFindAll
    {
        [Required]
        [Column("id")]
        [StringLength(21)]
        [Unicode(false)]
        public string Id { get; set; }

        [Column("full_name")]
        [StringLength(50)]
        [Unicode(false)]
        public string FullName { get; set; }

        [Required]
        [Column("name")]
        [StringLength(21)]
        [Unicode(false)]
        public string Name { get; set; }

        [Column("email")]
        [StringLength(50)]
        [Unicode(false)]
        public string Email { get; set; }

        [Column("group_name")]
        [StringLength(50)]
        [Unicode(false)]
        public string GroupName { get; set; }

        [Required]
        [Column("loc_id")]
        [StringLength(21)]
        [Unicode(false)]
        public string LocId { get; set; }

        [Column("loc_name")]
        [StringLength(50)]
        [Unicode(false)]
        public string LocName { get; set; }

        [Column("insurance_id")]
        [StringLength(21)]
        [Unicode(false)]
        public string InsuranceId { get; set; }

        [Column("is_locked")]
        public int IsLocked { get; set; }

        [Column("is_all_warehouse")]
        public byte IsAllWarehouse { get; set; }

        [Column("last_password_changes", TypeName = "date")]
        public DateTime? LastPasswordChanges { get; set; }

        [Required]
        [Column("role_id")]
        [StringLength(21)]
        [Unicode(false)]
        public string RoleId { get; set; }

        [Required]
        [Column("role_name")]
        [StringLength(50)]
        [Unicode(false)]
        public string RoleName { get; set; }
    }


    [Keyless]
    public class FindSessionDataQuery
    {
        [Column("employee_id")]
        public string? EmployeeId { get; set; }
        [Column("user_full_name")]
        public string fullName { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("department_id")]
        public string DepartmentId { get; set; }
        [Column("department_name")]
        public string? DepartmentName { get; set; }

        [Column("title_id")]
        public string TitleId { get; set; }

        [Column("div_id")]
        public string DivId { get; set; }

        [Column("level_id")]
        public string LevelId { get; set; }
    }

    [Keyless]
    public class FindEmployeeIdByNumberWA
    {
        [Column("employee_id")]
        public string EmployeeId { get; set; }
        [Column("user_id")]
        public string? userId { get; set; }

    }


    [Keyless]
    public class FindRequestData
    {
        [Column("employee_id")]
        public string EmployeeId { get; set; }

    }


}
