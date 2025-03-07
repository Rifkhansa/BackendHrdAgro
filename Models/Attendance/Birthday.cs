using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models;
using BackendHrdAgro.Controllers.Master;
using BackendHrdAgro.Models.Database.MySql.Support;
using BackendHrdAgro.Models.Employee;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendHrdAgro.Models.Attendance
{
    public class BirthdayModel
    {
        public List<Reminders> reminders()
        {
            var reminders = new DatabaseContext().Reminders.FromSqlRaw("select a.*,department_name ,\r\n        " +
                "DATE_FORMAT(a.tanggal_lahir, '%d %M %Y') as tanggal_lahir_view\r\n        " +
                "from tm_employee_affair a\r\n    \t\tleft outer join tm_department b on a.department_id=b.department_id  " +
                "where a.status = 1 order by employee_first_name ASC").ToList();
            return reminders;
        }

        public List<Dictionary<string, dynamic>> CreateExtendBirthday(TpExtend value)
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

    }

    public class BirthdayReqBody
    {
        public string EmployeeId { get; set; }
        public DateTime startExtendDate { get; set; }
        public DateTime endExtendDate { get; set; }
        public string Reason { get; set; }
    }

    [Keyless]
    public class Reminders
    {
        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("employee_first_name")]
        public string? EmployeeFirstName { get; set; }

        [Column("employee_last_name")]
        public string? EmployeeLastName { get; set; }

        [Column("department_name")]
        public string? DepartmentName { get; set; }

        [Column("tempat_lahir")]
        public string? TempatLahir { get; set; }

        [Column("tanggal_lahir")]
        public DateTime TanggalLahir { get; set; }

        [Column("tanggal_lahir_view")]
        public string? TanggalLahirView { get; set; }
    }

}
