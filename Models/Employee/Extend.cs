using BackendHrdAgro.Models.Database.MySql;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendHrdAgro.Models.Employee
{
    public class ExtendModel
    {
        public List<ExtendGroupQuery> extendGroupQueries()
        {
            var sql = "select a.*,case when a.status = 1 then 'Yes' when a.status=0 then 'No' end as my_status,\r\n      " +
                "employee_first_name,employee_last_name,DATE_FORMAT(a.start_extend_date, '%d %M %Y') " +
                "as start_extend_view,DATE_FORMAT(a.end_extend_date, '%d %M %Y') as end_extend_view\r\n      " +
                "from tp_extend a\r\n      left outer join tm_employee_affair b on a.employee_id=b.employee_id\r\n      " +
                "where a.status in (1,0,9) order by employee_first_name ASC";
            var extend = new DatabaseContext().ExtendGroupQueries.FromSqlRaw(sql).ToList();
            return extend;
        } 

        public List<Departments> departments()
        {
            var sql = "select department_id,department_name from tm_department a  where a.status in (1,0,9) order by department_name ASC";
            var department = new DatabaseContext().Departments.FromSqlRaw(sql).ToList();
            return department;
        }

        public List<Types> types()
        {
            var sql = "select type_id,type_name from tm_type a  where a.status in (1,0,9) order by type_name ASC";
            var type = new DatabaseContext().Types.FromSqlRaw(sql).ToList();
            return type;
        }

        public List<Titles> titles()
        {
            var sql = "select title_id,title_name from tm_title a  where a.status in (1) order by title_name ASC";
            var title = new DatabaseContext().Titles.FromSqlRaw(sql).ToList();
            return title;
        }

        public List<Levels> levels()
        {
            var sql = "select level_id,level_name from tm_level a  where a.status in (1,0,9) order by level_id ASC";
            var level = new DatabaseContext().Levels.FromSqlRaw(sql).ToList();
            return level;
        }

        public List<ReasonStatus> reasonStatuses()
        {
            var sql = "select reason_id,reason_name from tm_reason a  where a.status in (1,0,9) order by reason_id ASC";
            var reasonStatus = new DatabaseContext().ReasonStatuses.FromSqlRaw(sql).ToList();
            return reasonStatus;
        }

        public List<Dictionary<string, dynamic>> CreateExtend(TpExtend value)
        {

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();
            var executionStrategy = context.Database.CreateExecutionStrategy();
            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {

                    try
                    {
                        context.TpExtends.Add(value);
                        context.SaveChanges();

                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil Dibuat");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        transaction.Rollback();

                        data.Clear();
                        data.Add("result", false);
                        data.Add("message", e.Message);

                        result.Clear();
                        result.Add(data);

                    }
                }

            });
            return result;
        }

        public List<Dictionary<string, dynamic>> UpdateExtend(ExtendUpdate value)
        {
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();

            var current = context.TpExtends.Where(x => x.ExtendId == value.ExtendId).FirstOrDefault() ?? throw new Exception(value.ExtendId + " not found");

            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        TpExtend UpdateExtende = new TpExtend()
                        {
                            ExtendId = value.ExtendId,
                            EmployeeId = value.EmployeeId,
                            StartExtendDate = value.StartExtendDate,
                            EndExtendDate = value.EndExtendDate,
                            Reason = value.Reason,
                            Status = 0,

                            FileName = value.File.FileName,
                            Type = value.File.ContentType,
                            Size = value.File.Length
                        };

                        context.TpExtends.Entry(current).CurrentValues.SetValues(UpdateExtende);
                        context.SaveChanges();
                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil update");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e.InnerException == null ? e : e.InnerException);
                        transaction.Rollback();
                        data.Clear();
                        data.Add("result", false);
                        data.Add("message", e.Message);

                        result.Clear();
                        result.Add(data);
                    }
                }
            });
            return result;
        }

        public List<Dictionary<string, dynamic>> DeleteExtend(ExtendIdModel value)
        {
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();

            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var current = context.TpExtends.Where(x => x.ExtendId == value.ExtendId).FirstOrDefault() ?? throw new Exception(value.ExtendId + " not found");

                        TpExtend delete = new TpExtend()
                        {
                            ExtendId = value.ExtendId,
                            Status = 0
                        };

                        context.TpExtends.Entry(current).CurrentValues.SetValues(delete);
                        context.TpExtends.Remove(current);
                        context.SaveChanges();
                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Data berhasil dihapus");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e.Message);
                        transaction.Rollback();
                        //throw;
                        data.Clear();
                        data.Add("result", false);
                        data.Add("message", e.Message);

                        result.Clear();
                        result.Add(data);
                    }
                }
            });
            return result;

        }

    }

    public class ExtendIdModel
    {
        public string ExtendId { get; set; }
    }

    public class ExtendSignRequestBody
    {
        public string EmployeeId { get; set; } = null!;
        public DateTime StartExtendDate { get; set; }
        public DateTime EndExtendDate { get; set; }
        public string Reason { get; set; }
        public IFormFile File { get; set; }
    }

    public class ExtendUpdate
    {
        public string ExtendId { get; set; }
        public string EmployeeId { get; set; }
        public DateTime StartExtendDate { get; set; }
        public DateTime EndExtendDate { get; set; }
        public string Reason { get; set; }
        public IFormFile File { get; set; }
    }

    [Keyless]
    public class ExtendGroupQuery
    {
        [Column("extend_id")]
        public string ExtendId { get; set; }

        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("start_extend_date")]
        public DateTime StartExtendDate { get; set; }

        [Column("end_extend_date")]
        public DateTime EndExtendDate { get; set; }

        [Column("reason")]
        public string Reason { get; set; }

        [Column("file_name")]
        public string? FileName { get; set; }

        [Column("type")]
        public string? Type { get; set; }

        [Column("size")]
        public int Size { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("my_status")]
        public string MyStatus { get; set; }

        [Column("employee_first_name")]
        public string? EmployeeFirstName { get; set; }

        [Column("employee_last_name")]
        public string? EmployeeLastName { get; set; }

        [Column("start_extend_view")]
        public string StartExtendView { get; set; }

        [Column("end_extend_view")]
        public string EndExtendView { get; set; }
    }


}
