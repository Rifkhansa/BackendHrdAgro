using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using BackendHrdAgro.Models.Database.MySql.Support;
using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Master;

namespace BackendHrdAgro.Models.Employee
{
    public class WarningLetterModel
    {
        DatabaseContext databaseContext = new DatabaseContext();
        public IEnumerable<TpWarningLetter> warningFindById(string id)
        {
            var sql = databaseContext.TpWarnings.Where(x => x.Id == id).ToList();
            return sql;
        }
        public List<WarningLetterGroupQuery> WarningLetterGroups(string kriteria)
        {
            var warningletter = new DatabaseContext().WarningLetterGroupQueries.FromSqlRaw("select a.*,case when a.status = 1 then 'Berlaku' when a.status = 0 then 'Tidak Berlaku' end as my_status,  " +
                "DATE_FORMAT(a.begin_date, '%Y-%m-%d') as begin_date_view, DATE_FORMAT(a.end_date, '%Y-%m-%d') as end_date_view, case when a.status = 0 then 0 else DATEDIFF(a.end_date, NOW()) END AS " +
                "duration_in_days, u.user_full_name, d.department_name, t.title_name FROM tp_warning_letter a  " +
                "left join tm_users u on a.employee_id=u.employee_id " +
                "left join tm_employee_affair g on a.employee_id = g.employee_id join tm_department d on g.department_id = d.department_id " +
                $"join tm_title t on g.title_id=t.title_id {kriteria}").ToList();
            return warningletter;
        }
        public List<Departments> departments()
        {
            var departmen = new DatabaseContext().Departments.FromSqlRaw("select department_id,department_name from tm_department a  where a.status in (1,0,9) order by department_name ASC").ToList();
            return departmen;
        }
        public List<Titles> titles()
        {
            var title = new DatabaseContext().Titles.FromSqlRaw("select title_id,title_name from tm_title a  where a.status in (1) order by title_name ASC").ToList();
            return title;
        }
        public List<Users> users()
        {
            var user = new DatabaseContext().Users.FromSqlRaw("select employee_id,user_full_name from tm_users a where a.status in (1,0,9) order by user_full_name ASC").ToList();
            return user;
        }

        public bool warningDelete(DeleteReqBody value, string id)
        {
            try
            {
                var current = databaseContext.TpWarnings.Find(value.id) ?? throw new Exception("Data " + value.id + " Tidak ditemukan");
                databaseContext.TpWarnings.Remove(current);
                databaseContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException == null ? e.Message : e.InnerException);
                return false;
            }
        }

        public List<Dictionary<string, dynamic>> CreateWarningLetter(TpWarningLetter value)
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
                        var existingSp = context.TpWarnings.Where(w => w.EmployeeId == value.EmployeeId && w.Number == value.Number && w.EndDate > DateTime.Now)
                        .FirstOrDefault(); // cek SP aktif berdasarkan endDate

                        if (existingSp != null)
                        {
                            // Jika SP aktif dengan nomor yang sama ditemukan
                            data.Clear();
                            data.Add("result", false);
                            data.Add("message", "Karyawan masih memiliki SP yang aktif");

                            result.Clear();
                            result.Add(data);

                        }

                        // jika tidak ada SP aktif dengan number yang sama
                        context.TpWarnings.Add(value);
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

        public List<Dictionary<string, dynamic>> UpdateWarningLetter(UpdateWarningLetter value, string id = "USR-201710052")
        {
            UserDB userDB = new UserDB();
            var findSessionData = userDB.FindSessionDataUser(id);
            string userId = findSessionData[0].UserId;

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();

            var current = context.TpWarnings.Where(x => x.EmployeeId == value.employeeId).FirstOrDefault() ?? throw new Exception(value.employeeId + " not found");

            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        TpWarningLetter tpWarningLetter = new TpWarningLetter()
                        {
                            Id = value.id,
                            Number = value.number,
                            CausedBy = value.causedBy,
                            LetterDate = value.letterDate,
                            EmployeeId = value.employeeId,
                            BeginDate = value.beginDate,
                            EndDate = value.beginDate.AddMonths(6),
                            Description = value.description,
                            CreatedAt = DateTime.Now,
                            CreatedBy = userId,
                            UpdatedAt = DateTime.Now,
                            UpdatedBy = userId,
                            DeletedAt = DateTime.Now,
                            DeletedBy = userId,
                            Status = 0,

                            FileName = value.File.FileName,
                            Type = value.File.ContentType,
                            Size = value.File.Length
                        };

                        context.TpWarnings.Entry(current).CurrentValues.SetValues(tpWarningLetter);
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

    }

    public class CreateWarningLetter
    {
        public string employeeId { get; set; }
        public int number { get; set; }
        public string causedBy { get; set; }
        public DateTime letterDate { get; set; }
        public DateTime beginDate { get; set; }
        public string description { get; set; }
        public IFormFile File { get; set; }
    }

    public class UpdateWarningLetter
    {
        public string id { get; set; }
        public string employeeId { get; set; }
        public int number { get; set; }
        public string causedBy { get; set; }
        public DateTime letterDate { get; set; }
        public DateTime beginDate { get; set; }
        // public DateTime endDate { get; set; }
        public string description { get; set; }
        public IFormFile File { get; set; }
    }

    public class DeleteReqBody
    {
        public string id { get; set; }
    }

    [Keyless]
    public class WarningLetterGroupQuery
    {
        [Column("id")]
        public string id { get; set; }

        [Column("employee_id")]
        public string employeeId { get; set; }

        [Column("letter_date")]
        public DateTime letterDate { get; set; }

        [Column("user_full_name")]
        public string userFullName { get; set; }

        [Column("department_name")]
        public string? departmentName { get; set; }

        [Column("title_name")]
        public string? titleName { get; set; }

        [Column("number")]
        public int number { get; set; }

        [Column("caused_by")]
        public string causedBy { get; set; }

        [Column("description")]
        public string description { get; set; }

        [Column("begin_date")]
        public DateTime beginDate { get; set; }

        [Column("end_date")]
        public DateTime endDate { get; set; }

        [Column("begin_date_view")]
        public string BeginDateView { get; set; }

        [Column("end_date_view")]
        public string EndDateView { get; set; }

        [Column("duration_in_days")]
        public int duration { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("my_status")]
        public string? MyStatus { get; set; }

        [Column("file_name")]
        public string FileName { get; set; }

        /*[NotMapped]
        [Column("begin_date_view")]
        public string BeginDateView => beginDate.ToString("dd MMMM yyyy");

        [NotMapped]
        public string EndDateView => endDate.ToString("dd MMMM yyyy");*/
    }

    [Keyless]
    public class Users
    {
        [Column("employee_id")]
        public string employeeId { get; set; }

        [Column("user_full_name")]
        public string userFullName { get; set; }
    }
}
