using BackendHrdAgro.Models.Database.MySql;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendHrdAgro.Models.Employee
{
    public class TerminationModel
    {
        public List<TerminationGroupQuery> terminationGroupQueries()
        {
            var sql = "select a.*,case when a.status = 1 then 'Yes' when a.status=0 then 'No' end as my_status,     " +
                "employee_first_name,employee_last_name,reason_name,DATE_FORMAT(a.termination_date, '%d %M %Y') " +
                "as termination_view\r\n      from tp_termination a\r\n      left outer join tm_employee_affair b on a.employee_id=b.employee_id\r\n      " +
                "left outer join tm_reason c on a.reason_id=c.reason_id\r\n      where a.status in (1,0,9) order by employee_first_name ASC";
            var termination = new DatabaseContext().TerminationGroupQueries.FromSqlRaw(sql).ToList();
            return termination;
        }

        public List<ReasonStatus> reasons()
        {
            var sql = "select reason_id,reason_name from tm_reason a  where a.status in (1,0,9) order by reason_id ASC";
            var reasonStatus = new DatabaseContext().ReasonStatuses.FromSqlRaw(sql).ToList();
            return reasonStatus;
        }

        public List<Dictionary<string, dynamic>> CreateTermination(TpTermination value)
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
                        context.TpTerminations.Add(value);
                        context.SaveChanges();

                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil Disimpan");

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

        public List<Dictionary<string, dynamic>> UpdateTermination(UpdateTermination value)
        {

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();


            var current = context.TpTerminations.Where(x => x.TerminationId == value.TerminationId).FirstOrDefault() ?? throw new Exception(value.TerminationId + " not found");

            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        TpTermination updateTermination = new TpTermination()
                        {
                            TerminationId = value.TerminationId,
                            EmployeeId = value.EmployeeId,
                            TerminationDate = value.TerminationDate,
                            Reason = value.Reason,
                            ReasonId = value.statusApp,
                            Status = 0,

                            FileName = value.File.FileName,
                            Type = value.File.ContentType,
                            Size = value.File.Length
                        };

                        context.TpTerminations.Entry(current).CurrentValues.SetValues(updateTermination);
                        context.SaveChanges();
                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil Update");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.InnerException == null ? e : e.InnerException);
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

        public List<Dictionary<string, dynamic>> DeleteTermination(TerminationIdModel value, string id)
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
                        var current = context.TpTerminations.Where(x => x.TerminationId == value.TerminationId).FirstOrDefault() ?? throw new Exception(value.TerminationId + " not found");

                        TpTermination delete = new TpTermination()
                        {
                            TerminationId = value.TerminationId,
                            Status = 0
                        };

                        context.TpTerminations.Entry(current).CurrentValues.SetValues(delete);
                        context.TpTerminations.Remove(current);
                        context.SaveChanges();
                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Termination berhasil dihapus");

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

    public class TerminationIdModel
    {
        public string TerminationId { get; set; }
    }

    [Keyless]
    public class TerminationGroupQuery
    {
        [Column("termination_id")]
        public string TerminationId { get; set; }

        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("termination_date")]
        public DateTime TerminationDate { get; set; }

        [Column("reason")]
        public string? Reason { get; set; }

        [Column("reason_id")]
        public int ReasonId { get; set; }

        [Column("file_name")]
        public string? FileName { get; set; }

        [Column("status")]
        public int? Status { get; set; }

        [Column("size")]
        public int? Size { get; set; }

        [Column("type")]
        public string? Type { get; set; }

        [Column("my_status")]
        public string? MyStatus { get; set; }

        [Column("employee_first_name")]
        public string? EmployeeFirstName { get; set; }

        [Column("employee_last_name")]
        public string? EmployeeLastName { get; set; }

        [Column("reason_name")]
        public string? ReasonName { get; set; }

        [Column("termination_view")]
        public string TerminationView { get; set; }
    }

    public class TerminationSignOut
    {
        [Required]
        public string EmployeeId { get; set; } = null!;
        public DateTime TerminationDate { get; set; }
        public string Reason { get; set; }
        public int Status { get; set; }
        public byte statusApp { get; set; }
        public IFormFile File { get; set; }
    }

    public class UpdateTermination
    {
        public string TerminationId { get; set; }
        public string EmployeeId { get; set; }
        public DateTime TerminationDate { get; set; }
        public string Reason { get; set; }
        public byte statusApp { get; set; }
        public IFormFile File { get; set; }
    }


}
