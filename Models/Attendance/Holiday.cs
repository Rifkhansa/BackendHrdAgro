using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Employee;
using BackendHrdAgro.Models.Master;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendHrdAgro.Models.Attendance
{
    public class HolidayModel
    {
        public List<HolidayGroup> holidayGroups()
        {
            var sql = "select a.*,case when status = 1 then 'Active' when status=0 " +
                "then 'Non Active' end as my_status from tm_holiday_date a where status in (1,0,9) order by holiday_id ASC";
            var holiday = new DatabaseContext().HolidayGroups.FromSqlRaw(sql).ToList();
            return holiday;
        }

        public List<Dictionary<string, dynamic>> CreateHolidays(TmHolidayDate value)
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
                        context.TmHolidayDates.Add(value);
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

        public List<Dictionary<string, dynamic>> UpdateHolidays(UpdateHoliday value, string id = "USR-201710052")
        {

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();

            var current = context.TmHolidayDates.Where(x => x.HolidayId == value.HolidayId).FirstOrDefault() ?? throw new Exception(value.HolidayId + " not found");

            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        UserDB userDB = new UserDB();
                        var findSessionData = userDB.FindSessionDataUser(id);
                        string userId = findSessionData[0].UserId;

                        TmHolidayDate UpdateholidayDate = new TmHolidayDate()
                        {
                            HolidayId = value.HolidayId,
                            HolidayName = value.HolidayName,
                            HolidayDate = value.HolidayDate,
                            Status = 1,

                            DtEtr = DateTime.Now,
                            UserEtr = userId,
                            DtUpdate = DateTime.Now,
                            UserUpdate = userId
                        };

                        context.TmHolidayDates.Entry(current).CurrentValues.SetValues(UpdateholidayDate);
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

        public List<Dictionary<string, dynamic>> DeleteHolidays(DeleteHoliday value)
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
                        var current = context.TmHolidayDates.Where(x => x.HolidayId == value.holidayId).FirstOrDefault() ?? throw new Exception(value.holidayId + " not found");

                        TmHolidayDate delete = new TmHolidayDate()
                        {
                            HolidayId = value.holidayId,
                            Status = 0
                        };

                        context.TmHolidayDates.Entry(current).CurrentValues.SetValues(delete);
                        context.TmHolidayDates.Remove(current);
                        context.SaveChanges();
                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Holiday berhasil dihapus");

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

    public class CreateHoliday
    {
        public string HolidayName { get; set; }
        public DateTime HolidayDate { get; set; }
    }

    public class UpdateHoliday
    {
        public string HolidayId { get; set; }
        public string HolidayName { get; set; }
        public DateTime HolidayDate { get; set; }
    }

    public class DeleteHoliday
    {
        public string holidayId { get; set; }
    }

    [Keyless]
    public class HolidayGroup
    {
        [Column("holiday_id")]
        public string HolidayId { get; set; }

        [Column("holiday_name")]
        public string HolidayName { get; set; }

        [Column("holiday_date")]
        public DateTime HolidayDate { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("dt_etr")]
        public DateTime DtEtr { get; set; }

        [Column("user_etr")]
        public string UserEtr { get; set; }

        [Column("dt_update")]
        public DateTime DtUpdate { get; set; }

        [Column("user_update")]
        public string UserUpdate { get; set; }

        [Column("my_status")]
        public string MyStatus { get; set; }
    }
}
