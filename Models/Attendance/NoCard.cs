using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Database.MySql.Support;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendHrdAgro.Models.Attendance
{
    public class NoCardModel
    {
        public List<NoCardQuery> ListNoCard(string filter)
        {

            string sql = $"select  z.employee_id, concat(a.employee_first_name,' ',a.employee_last_name) as employee_name," +
                $"z.absent_date,z.file_name,z.dt_etr,b.department_name,c.title_name, " +
                $"case when DATE_FORMAT(z.dt_etr, '%d %M %Y') = DATE_FORMAT(now(), '%d %M %Y') then 1 else 0  end as dimunculin, " +
                $"'' as link_del " +
                $"from tp_absent_no_card z " +
                $"inner join tm_employee_affair a on z.employee_id=a.employee_id " +
                $"inner join tm_department b on a.department_id = b.department_id " +
                $"inner join tm_title c on a.title_id = c.title_id " +
                $"where a.status not in ('5','7','9') and  a.department_id not in ('DP001','DP009') {filter} " +
                $"order by z.absent_date DESC";
            var list = new DatabaseContext().NoCardQueries.FromSqlRaw(sql).ToList();
            return list;
        }

        public List<Dictionary<string, dynamic>> CreateNoCard(TpAbsentNoCard value)
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
                        context.Database.ExecuteSqlRaw($"DELETE FROM tp_absent_no_card WHERE employee_id='{value.EmployeeId}' and  DATE_FORMAT(absent_date,'%Y-%m-%d')='{value.AbsentDate.ToString("yyyy-MM-dd")}'");
                        context.SaveChanges();

                        context.TpAbsentNoCards.Add(value);
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

        public List<ListWAUser> listUser(string phone)
        {
            var listUser = new DatabaseContext().ListWAUsers.FromSqlRaw($"select employee_id,user_full_name  from tm_users a where phone = '{phone}'").ToList();
            return listUser;
        }


    }

    public class CreateNoCardWhatsappModel
    {
        public string? Phone { get; set; }
        public DateTime AbsentDate { get; set; }
        public IFormFile FileName { get; set; }
    }

    public class CreateNoCardModel
    {
        public string EmployeeId { get; set; }
        public DateTime AbsentDate { get; set; }
        public IFormFile FileName { get; set; }
    }

    [Keyless]
    public class NoCardQuery
    {
        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("file_name")]
        public string FileName { get; set; }

        [Column("employee_name")]
        public string EmployeeName { get; set; }

        [Column("department_name")]
        public string DepartmentName { get; set; }

        [Column("title_name")]
        public string TitleName { get; set; }

        [Column("absent_date")]
        public DateTime AbsentDate { get; set; }

        [Column("dt_etr")]
        public DateTime DtEtr { get; set; }

        [Column("link_del")]
        public string LinkDel { get; set; }

        [Column("dimunculin")]
        public int Dimunculin { get; set; }


    }

    [Keyless]
    public class ListWAUser
    {
        [Column("employee_id")]
        public string EmployeeId { get; set; }
        [Column("user_full_name")]
        public string UserFullName { get; set; }

    }

}
