using BackendHrdAgro.Models.Database.MySql;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendHrdAgro.Models.Employee
{
    public class MutationModel
    {
        public List<MutationGroup> mutationGroups()
        {
            var sql = "select a.*,case when a.status = 1 then 'Yes' when a.status=0 then 'No' end as my_status,\r\n      " +
                "employee_first_name,employee_last_name,DATE_FORMAT(a.mutasi_date, '%d %M %Y') as mutasi_view,\r\n      " +
                "g.department_name as department_old_name, c.department_name,\r\n      h.title_name as title_old_name, d.title_name,\r\n      " +
                "i.level_name as level_old_name, e.level_name,\r\n      type_name from tp_mutasi a left outer join tm_employee_affair b on a.employee_id=b.employee_id\r\n      " +
                "left outer join tm_department c on a.department_id=c.department_id\r\n      left outer join tm_title d on a.title_id=d.title_id\r\n      " +
                "left outer join tm_level e on a.level_id=e.level_id\r\n      left outer join tm_type f on a.type_id=f.type_id\r\n      " +
                "left outer join tm_department g on a.department_old_id=g.department_id\r\n      left outer join tm_title h on a.title_old_id=h.title_id\r\n      " +
                "left outer join tm_level i on a.level_old_id=i.level_id  where a.status in (1,0,9) order by employee_first_name ASC";
            var mutation = new DatabaseContext().MutationGroups.FromSqlRaw(sql).ToList();
            return mutation;
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

        public List<Dictionary<string, dynamic>> CreateMutation(TpMutasi value)
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
                        context.TpMutasis.Add(value);
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

        public List<Dictionary<string, dynamic>> UpdateMutasi(MutationUpdate value)
        {

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();


            var current = context.TpMutasis.Where(x => x.MutasiId == value.MutasiId).FirstOrDefault() ?? throw new Exception(value.MutasiId + " not found");

            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        TpMutasi update = new TpMutasi()
                        {
                            MutasiId = value.MutasiId,
                            EmployeeId = value.EmployeeId,
                            TypeId = value.TypeId,
                            DepartmentOldId = value.DepartmentOldId,
                            DepartmentId = value.DepartmentId,
                            TitleOldId = value.TitleOldId,
                            TitleId = value.TitleId,
                            LevelOldId = value.LevelOldId,
                            LevelId = value.LevelId,
                            MutasiDate = value.MutasiDate,
                            Deskripsi = value.Deskripsi,
                            Status = 0,

                            FileName = value.File.FileName,
                            Type = value.File.ContentType,
                            Size = value.File.Length
                        };

                        context.TpMutasis.Entry(current).CurrentValues.SetValues(update);
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

        public List<Dictionary<string, dynamic>> DeleteMutation(MutationId value)
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
                        var current = context.TpMutasis.Where(x => x.MutasiId == value.MutasiId).FirstOrDefault() ?? throw new Exception(value.MutasiId + " not found");

                        TpMutasi delete = new TpMutasi()
                        {
                            MutasiId = value.MutasiId,
                            Status = 0
                        };

                        context.TpMutasis.Entry(current).CurrentValues.SetValues(delete);
                        context.TpMutasis.Remove(current);
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

    public class MutationRequestBody
    {
        public string EmployeeId { get; set; }
        public string? TypeId { get; set; }
        public string DepartmentOldId { get; set; }
        public string DepartmentId { get; set; }
        public string TitleOldId { get; set; }
        public string TitleId { get; set; }
        public string LevelOldId { get; set; }
        public string LevelId { get; set; }
        public DateTime MutasiDate { get; set; }
        public string Deskripsi { get; set; }
        public IFormFile File { get; set; }
    }

    public class MutationUpdate
    {
        public string MutasiId { get; set; }
        public string EmployeeId { get; set; }
        public string TypeId { get; set; }
        public string DepartmentOldId { get; set; }
        public string DepartmentId { get; set; }
        public string TitleOldId { get; set; }
        public string TitleId { get; set; }
        public string LevelOldId { get; set; }
        public string LevelId { get; set; }
        public DateTime MutasiDate { get; set; }
        public string Deskripsi { get; set; }
        public IFormFile File { get; set; }
    }

    public class MutationId
    {
        public string MutasiId { get; set; }
    }

    [Keyless]
    public class MutationGroup
    {
        [Column("mutasi_id")]
        public string MutasiId { get; set; }

        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("department_old_id")]
        public string? DepartmentOldId { get; set; }

        [Column("department_id")]
        public string? DepartmentId { get; set; }

        [Column("title_old_id")]
        public string? TitleOldId { get; set; }

        [Column("title_id")]
        public string? TitleId { get; set; }

        [Column("level_old_id")]
        public string? LevelOldId { get; set; }

        [Column("level_id")]
        public string? LevelId { get; set; }

        [Column("file_name")]
        public string? FileName { get; set; }

        [Column("type")]
        public string? Type { get; set; }

        [Column("size")]
        public int Size { get; set; }

        [Column("type_id")]
        public string? TypeId { get; set; }

        [Column("mutasi_date")]
        public DateTime MutasiDate { get; set; }

        [Column("deskripsi")]
        public string? Deskripsi { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("my_status")]
        public string? MyStatus { get; set; }

        [Column("employee_first_name")]
        public string? EmployeeFirstName { get; set; }

        [Column("employee_last_name")]
        public string? EmployeeLastName { get; set; }

        [NotMapped]
        public string? MutasiView => MutasiDate.ToString("dd MMMM yyyy");

        [Column("department_old_name")]
        public string? DepartmentOldName { get; set; }

        [Column("department_name")]
        public string? DepartmentName { get; set; }

        [Column("title_old_name")]
        public string? TitleOldName { get; set; }

        [Column("title_name")]
        public string? TitleName { get; set; }

        [Column("level_old_name")]
        public string? LevelOldName { get; set; }

        [Column("level_name")]
        public string? LevelName { get; set; }

        [Column("type_name")]
        public string? TypeName { get; set; }

        [NotMapped]
        public string SizeFormatted => Size.ToString("N0");
    }

}
