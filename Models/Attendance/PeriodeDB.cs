using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Database.MySql.Support;
using BackendHrdAgro.Models.Master;
using NPOI.Util;
using BackendHrdAgro.Controllers.Master;

namespace BackendHrdAgro.Models.Attendance
{
    public class PeriodeDB
    {
        DatabaseContext databaseContext = new DatabaseContext();

        public bool IsPeriodeAbsentClosed(string PeriodeId) => databaseContext.TpAbsenteePeriodes.Where(x => x.PeriodeId.Equals(PeriodeId) && x.Status == 99).Any();
        public List<PeriodeQuery> Periodes()
        {

            string sql = $"select a.periode_id,a.year,a.month, a.status, " +
                $"DATE_FORMAT(CONCAT(a.year,'-',a.month,'-01'),'%M %Y') as periode_desc, " +
                $"case when a.status=1 then 'open' when a.status=99 then 'closed' else 'Error' end as status_desc,'' as link_cLosed " +
                $"from tp_absentee_periode a " +
                $"order by a.periode_id desc";
            var list = new DatabaseContext().PeriodeQueries.FromSqlRaw(sql).ToList();
            return list;
        }


        public List<PeriodeCreateQuery> PeriodesCreate()
        {

            string sql = $"select a.periode_id,periode_desc,'' as link_create " +
                $"from ( " +
                $"      SELECT " +
                $"      DATE_FORMAT(DATE_ADD(NOW(), INTERVAL z.month_offset MONTH), '%Y%m') AS periode_id, " +
                $"      DATE_FORMAT(DATE_ADD(NOW(), INTERVAL z.month_offset MONTH), '%M %Y') AS periode_desc " +
                $"      FROM (" +
                $"          SELECT -3 AS month_offset UNION ALL " +
                $"          SELECT -2 UNION ALL " +
                $"          SELECT -1 UNION ALL " +
                $"          SELECT 0 UNION ALL " +
                $"          SELECT 1 UNION ALL " +
                $"          SELECT 2 UNION ALL " +
                $"          SELECT 3 " +
                $"          ) z" +
                $"  ) a " +
                $" where a.periode_id not in (select periode_id from tp_absentee_periode )";
            var list = new DatabaseContext().PeriodeCreateQueries.FromSqlRaw(sql).ToList();
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


        public List<Dictionary<string, dynamic>> CreatePeriode(TpAbsenteePeriode value, string id)
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
                        context.TpAbsenteePeriodes.Add(value);
                        context.SaveChanges();
 
                        transaction.Commit();


                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "periode berhasil ditambahkan");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.InnerException == null ? e.Message : e.InnerException);
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

        public List<Dictionary<string, dynamic>> ClosedPeriode(TpAbsenteePeriode value, string id)
        {

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();


            var current = context.TpAbsenteePeriodes.Where(x => x.PeriodeId == value.PeriodeId).FirstOrDefault() ?? throw new Exception(value.PeriodeId + " not found");



            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        TpAbsenteePeriode closedPeriode = new TpAbsenteePeriode();
                        closedPeriode = current;
                        closedPeriode.UserEtrClosing = value.UserEtrClosing;
                        closedPeriode.DtEtrClosing = value.DtEtrClosing;
                        closedPeriode.Status = value.Status;

                        context.TpAbsenteePeriodes.Entry(current).CurrentValues.SetValues(closedPeriode);
                        context.SaveChanges();
                        transaction.Commit();


                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "periode berhasil ditutup");

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


    [Keyless]
    public class PeriodeQuery
    {
        [Column("periode_id")]
        public string PeriodeId { get; set; }

        [Column("periode_desc")]
        public string PeriodeDesc { get; set; }

        [Column("year")]
        public string Year { get; set; }

        [Column("month")]
        public string Month { get; set; }
         

        [Column("status")]
        public int Status { get; set; }

        [Column("status_desc")]
        public string StatusDesc { get; set; }

        [Column("link_cLosed")]
        public string LinkCLosed { get; set; }


    }


    [Keyless]
    public class PeriodeCreateQuery
    {
        [Column("periode_id")]
        public string PeriodeId { get; set; }

        [Column("periode_desc")]
        public string PeriodeDesc { get; set; }


        [Column("link_create")]
        public string LinkCreate { get; set; }


    }

   
    public class CreatePeriodeModel
    {
        public string PeriodeId { get; set; }

        public string UserEtrBy { get; set; }
    }
     
   


    public class ClosedPeriodeModel
    {

        public string ClosedBy { get; set; }
        public string PeriodeId { get; set; }



    }

}
