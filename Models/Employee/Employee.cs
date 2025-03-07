using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using BackendHrdAgro.Controllers.Employee;
using BackendHrdAgro.Models.Database;
using BackendHrdAgro.Models.Database.MySql;

namespace BackendHrdAgro.Models.Employee
{
    public class EmployeeFactory
    {
        /*public static List<EmployeeGroup>? GetEmployeeGroup(EmployeeGroupAction action, string id)
        {
            using DatabaseContext context = new DatabaseContext();
            try
            {
                var query = from a in context.TmEmployeeAffairs
                            join b in context.TmDepartments on a.DepartmentId equals b.DepartmentId into bGroup
                            from department in bGroup.DefaultIfEmpty()
                            join c in context.TmTitles on a.TitleId equals c.TitleId into cGroup
                            from title in cGroup.DefaultIfEmpty()
                            join d in context.TmLevels on a.LevelId equals d.LevelId into dGroup
                            from level in dGroup.DefaultIfEmpty()
                            join e in context.TmEmployeeStatuses on a.EmployeeStatusId equals e.EmployeeStatusId into eGroup
                            from employeeStatus in eGroup.DefaultIfEmpty()
                            join f in context.TmReligis on a.ReligiId equals f.ReligiId into fGroup
                            from religion in fGroup.DefaultIfEmpty()
                            join g in context.TmMarrieds on a.MarriedId equals g.MarriedId into gGroup
                            from married in gGroup.DefaultIfEmpty()
                            join h in context.TmBanks on a.BankId equals h.BankId into hGroup
                            from bank in hGroup.DefaultIfEmpty()
                            join j in context.TmSexes on a.GenderId equals j.GenderId into jGroup
                            from gender in jGroup.DefaultIfEmpty()
                            join k in context.TmBloodTypes on a.BloodTypeId equals k.BloodTypeId into kGroup
                            from bloodType in kGroup.DefaultIfEmpty()
                            join i in (from tp in context.TpTerminations
                                       where DateTime.Now <= tp.TerminationDate.AddMonths(1)
                                       select tp) on a.EmployeeId equals i.EmployeeId into iGroup
                            from termination in iGroup.DefaultIfEmpty()
                            where new List<int> { 1, 5, 7, 9 }.Contains(a.Status)
                            select new EmployeeGroup
                            {
                                EmployeeAffair = a,
                                MyStatus = a.Status == 1 ? "Active" :
                                           a.Status == 0 ? "Non Active" :
                                           a.Status == 5 ? "PHK" :
                                           a.Status == 7 ? "Kontrak Habis / Kontrak Tidak diperpanjang" :
                                                          "Resign",
                                BankName = bank.BankName,
                                DepartmentName = department.DepartmentName,
                                TitleName = title.TitleName,
                                ReligiName = religion.ReligiName,
                                LevelName = level.LevelName,
                                GenderName = gender.GenderName,
                                EmployeeStatusName = employeeStatus.EmployeeStatusName,
                                MarriedName = married.MarriedName,
                                TerminationDate = termination.TerminationDate,
                                BloodTypeName = bloodType.BloodTypeName,
                                DivisionId =  department.DivId
                            };
                switch (action)
                {
                    case EmployeeGroupAction.EmployeeId:
                        return query.Where(x=> x.EmployeeAffair.EmployeeId == id).OrderBy(x=> x.EmployeeAffair.EmployeeFirstName).ToList();
                    case EmployeeGroupAction.DepartementId:
                        return query.Where(x => x.EmployeeAffair.DepartmentId == id).OrderBy(x => x.EmployeeAffair.EmployeeFirstName).ToList();
                    case EmployeeGroupAction.DivisionId:
                        return query.Where(x => x.DivisionId== id).OrderBy(x => x.EmployeeAffair.EmployeeFirstName).ToList();
                    case EmployeeGroupAction.All: 
                        return query.OrderBy(x => x.EmployeeAffair.EmployeeFirstName).ToList();
                    default: return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }
        */
        public static List<TmEmployeeAffairReturn> GetEmployee() => new DatabaseContext().TmEmployeeAffairs.Where(x => new[] { 1, 0, 9 }.Contains(x.Status)).Select(x => new TmEmployeeAffairReturn { EmployeeId = x.EmployeeId, EmployeeFirstName = x.EmployeeFirstName, EmployeeLastName = x.EmployeeLastName }).ToList();
        
        public static List<TmEmployeeAffairReturn> GetEmployee(string id) => new DatabaseContext().TmEmployeeAffairs.Where(x => x.EmployeeId.Equals(id) && new[] { 1, 0, 9 }.Contains(x.Status)).Select(x => new TmEmployeeAffairReturn { EmployeeId = x.EmployeeId, EmployeeFirstName = x.EmployeeFirstName, EmployeeLastName = x.EmployeeLastName }).ToList();
        
        public static object GetEmployeeWithDepartment(string id)
        {
            try
            {
                using DatabaseContext context = new DatabaseContext();
                var query = from a in context.TmEmployeeAffairs
                            join b in context.TmDepartments on a.DepartmentId equals b.DepartmentId
                            where a.EmployeeId == id
                            select new
                            {
                                a.EmployeeFirstName,
                                a.EmployeeLastName,
                                DepartmentName = b.DepartmentName
                            };
                return query.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static EmployeeMinDetail? GetEmployeeDepartment(string employeeId)
        {
            using DatabaseContext context = new DatabaseContext();
            try
            {
                return (from emp in context.TmEmployeeAffairs
                        join dep in context.TmDepartments on emp.DepartmentId equals dep.DepartmentId
                        where emp.EmployeeId == employeeId
                        select new EmployeeMinDetail
                        {
                            EmployeeName = $"{emp.EmployeeFirstName} {emp.EmployeeLastName}",
                            DepartmentId = dep.DepartmentId,
                            DivisionId = dep.DivId,
                            LevelID = emp.LevelId,
                            TittleId = emp.LevelId
                        }).FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static string? EmployeeName(string employeeId) => new DatabaseContext().TmUsers.Where(x => x.EmployeeId.Equals(employeeId)).Select(x => x.UserFullName).FirstOrDefault();
        //public static string? EmployeeWANumber(string employeeId) => new DatabaseContext().TmUsers.Where(x => x.EmployeeId.Equals(employeeId)).Select(x => x.Phone).FirstOrDefault();

        public static bool IsManagement(string employeeId) => new DatabaseContext().TmEmployeeAffairs.Where(x => x.EmployeeId.Equals(employeeId) && x.TitleId.Equals("DS002")).Any();

        public static bool IsSuperUser(string employeeId) => new DatabaseContext().TmUsers.Where(x => x.EmployeeId.Equals(employeeId) && x.AllEmployeeAllowed == 1).Any();

        public enum EmployeeGroupAction
        {
            All,
            EmployeeId,
            DepartementId,
            DivisionId
        }

        public static List<EmployeeIdentifier> GetDivisionHead(string departmentId) => new DatabaseContext().TmEmployeeAffairs.Where(x => x.DepartmentId.Equals(departmentId) && x.LevelId.Equals("TL019")).Select(x => new EmployeeIdentifier { EmployeeId = x.EmployeeId}).ToList();

        public static List<EmployeeIdentifier> GetDepartmentHead(string departmentId) => new DatabaseContext().TmEmployeeAffairs.Where(x => x.DepartmentId.Equals(departmentId) && x.LevelId.Equals("TL024")).Select(x => new EmployeeIdentifier { EmployeeId = x.EmployeeId }).ToList();

        public static bool IsHeadDivision(string employeeId) => new DatabaseContext().TmEmployeeAffairs.Where(x => x.EmployeeId.Equals(employeeId) && x.LevelId.Equals("TL019")).Any();

        public static bool IsHeadDepartment(string employeeId) => new DatabaseContext().TmEmployeeAffairs.Where(x => x.EmployeeId.Equals(employeeId) && x.LevelId.Equals("TL024")).Any();

        public static string GetHeadDivision(string divisionId)
        {
            using DatabaseContext context = new DatabaseContext();
            try
            {
                return (from employee in context.TmEmployeeAffairs
                             join department in context.TmDepartments on employee.DepartmentId equals department.DepartmentId
                             where department.DivId == divisionId && employee.LevelId == "TL019"
                             select employee.EmployeeId).FirstOrDefault() ?? throw new Exception("Invalid data received, " + divisionId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /*public static string? GetHeadDepartment(string departmentId) => new DatabaseContext().TmEmployeeAffairs.Where(x => x.DepartmentId.Equals(departmentId) && x.LevelId.Equals("TL024")).Select(x => x.EmployeeId).FirstOrDefault();*/
        public static string? GetHeadDepartment(string departmentId) => new DatabaseContext().TmEmployeeAffairs.Where(x => x.DepartmentId.Equals(departmentId) && (x.LevelId.Equals("TL024") || x.LevelId.Equals("TL017"))).Select(x => x.EmployeeId).FirstOrDefault();
        public static bool HasDepartmentHead(string departmentId) => new DatabaseContext().TmEmployeeAffairs.Where(x => x.DepartmentId.Equals(departmentId) && x.LevelId.Equals("TL024")).Any();
            
    }
    public class EmployeeMinDetail
    {
        public string EmployeeName { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public string DivisionId { get; set; } = null!;
        public string LevelID { get; set; } = null!;
        public string TittleId { get; set; } = null!;

    }
    public class EmployeeGroup
    {
        public TmEmployeeAffair EmployeeAffair { get; set; } = null!;
        public string? MyStatus { get; set; }
        public string BankName { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public string DivisionId { get; set; } = null!;
        public string TitleName { get; set; } = null!;
        public string ReligiName { get; set; } = null!;
        public string LevelName { get; set; } = null!;
        public string GenderName { get; set; } = null!;
        public string EmployeeStatusName { get; set; } = null!;
        public string? MarriedName { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string? BloodTypeName { get; set; }
    }

    public class EmployeeIdentifier
    {
        public string EmployeeId { get; set; } = null!;
    }

}
