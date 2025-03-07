using BackendHrdAgro.Models.Database.MySql;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendHrdAgro.Models.Attendance
{
    public class OvertimeModel
    {
        AbsenteeModel absenteeModel = new AbsenteeModel();
        public List<OvertimeQuery> ListOvertimes(string filter)
        {
            string sql = "";

            sql = $"SELECT a.request_id, a.employee_id,b.employee_first_name,b.employee_last_name," +
                $"a.request_date, DATE_FORMAT(a.request_date, '%d %M %Y') as request_date_view, " +
                $"a.time_start, a.time_end,keperluan,a.request_type_id, " +
                $"case when a.status = 1 then 'Accepted' when a.status=0 then 'waiting' when a.status=5 then 'Rejected' end as my_status, " +
                $"noted,e.time_in as time_in_mesin, e.time_out,request_name,a.total_overtime, " +
                $"case when a.cut_absent = 0 then 'None' when a.cut_absent = 0.25 then '1/4 day' when a.cut_absent = 0.5 then '1/2 day' when a.cut_absent = 1 then 'Full day' end as my_cut_cuty, " +
                $"d.employee_first_name as approve_name,d.employee_last_name as approve_name2, " +
                $"case when a.request_type_id = 1 then  case when DATE_FORMAT(a.request_date, '%d %M %Y') = DATE_FORMAT(now(), '%d %M %Y') then 1 else 0 end else 1 end as dimunculin, " +
                $"a.status,'' as link_over_time_del,'' as link_over_time_app " +
                $"from tp_request a " +
                $"left outer join tm_employee_affair b on a.employee_id = b.employee_id " +
                $"left outer join tm_request c on a.request_type_id = c.request_type_id " +
                $"left outer join tm_employee_affair d on a.approve_by = d.employee_id " +
                $"left outer join tp_absent e on a.employee_id = e.employee_id and a.request_date = e.absent_date " +
                $"left outer join tm_department f on b.department_id = f.department_id " +
                $"where a.request_type_id = 1 and a.status in (1, 0, 5) {filter} and a.deleted_at is null " +
                $"order by a.status asc, a.request_date DESC ";
            var list = new DatabaseContext().OvertimeQueries.FromSqlRaw(sql).ToList();

            // Perulangan untuk menghitung total overtime
            TimeSpan startOvertime = new TimeSpan(18, 0, 1);
            TimeSpan timeOut = new TimeSpan();
            double totalOvertime = 0.0;
            foreach (var i in list)
            {
                var findIsOb = absenteeModel.FindIsOB(i.EmployeeId);
                int isOB = findIsOb[0].isOB;

                if (i.TimeOut != null)
                {
                    DayOfWeek dayOfWeek = i.RequestDate.DayOfWeek;

                    if (isOB == 1) // OB employee
                    {
                        if (dayOfWeek >= DayOfWeek.Monday && dayOfWeek <= DayOfWeek.Friday)
                        {
                            startOvertime = new TimeSpan(18, 0, 1); // Senin - Jumat: mulai lembur 18:01
                        }
                        else if (dayOfWeek == DayOfWeek.Saturday)
                        {
                            startOvertime = new TimeSpan(12, 0, 1); // Sabtu: mulai lembur 12:01
                        }
                        else
                        {
                            startOvertime = TimeSpan.Parse(i.TimeInMesin); // Minggu: mulai lembur saat dia mulai absen
                        }
                    }
                    else // Non-OB employee
                    {
                        if (dayOfWeek >= DayOfWeek.Monday && dayOfWeek <= DayOfWeek.Friday)
                        {
                            startOvertime = new TimeSpan(18, 0, 1); // Senin - Jumat: mulai lembur 18:01
                        }
                        else
                        {
                            startOvertime = TimeSpan.Parse(i.TimeInMesin); // Sabtu-Minggu: mulai lembur saat dia absen masuk
                        }
                    }

                    timeOut = TimeSpan.Parse(i.TimeOut);
                    Console.WriteLine("timeOut= " + timeOut);

                    // Hitung total lembur dalam bentuk TimeSpan
                    var overtimeDuration = timeOut - startOvertime;

                    // Konversi total lembur ke jam desimal
                    totalOvertime = overtimeDuration.TotalHours;
                    // Format output jam dan menit
                    int hours = (int)overtimeDuration.TotalHours;
                    int minutes = overtimeDuration.Minutes;

                    // Gabungkan ke dalam string hasil
                    i.TotalOvertime = $"{totalOvertime:0.0} jam ({hours} jam {minutes} menit)";

                }
            }

            return list;
        }

        public List<Dictionary<string, dynamic>> CreateOvertime(TpRequest value)
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
                        context.TpRequests.Add(value);
                        context.SaveChanges();

                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil Membuat Pengajuan Lembur");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.InnerException == null ? e.Message : e.InnerException);
                        System.Console.WriteLine(e.Message);
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

        public int CountOvertimeData(string RequestId) => new DatabaseContext().TpRequests.Where(x => x.RequestId.Equals(RequestId)).Count();

        public bool CheckIsApprove(string RequestId)
        {
            using (var context = new DatabaseContext())
            {
                // Cek apakah ada record dengan leaveId dan status 0
                var isStatusZero = context.TpRequests
                    .Any(x => x.RequestId.Equals(RequestId) && x.Status.Equals(0));

                // Jika ada status 0, kembalikan false, jika tidak, kembalikan true
                return isStatusZero;
            }
        }

        public List<Dictionary<string, dynamic>> ApproveOvertime(TpRequest value, string id)
        {

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();


            var current = context.TpRequests.Where(x => x.RequestId == value.RequestId).FirstOrDefault() ?? throw new Exception(value.Noted + " not found");

            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        TpRequest approveOvertime = new TpRequest();
                        approveOvertime = current;
                        approveOvertime.ApproveBy = value.ApproveBy;
                        approveOvertime.Noted = value.Noted;
                        approveOvertime.Status = value.Status;
                        approveOvertime.IsNotify = value.IsNotify;
                        approveOvertime.UpdatedAt = value.UpdatedAt;
                        approveOvertime.UpdatedBy = value.ApproveBy;

                        context.TpRequests.Entry(current).CurrentValues.SetValues(approveOvertime);
                        context.SaveChanges();
                        transaction.Commit();


                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Proses Approval berhasil dilakukan");

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


        public List<TpRequest> OvertimeFind(string id) => new DatabaseContext().TpRequests.Where(x => x.RequestId.Equals(id)).ToList();

        public DateTime FindOvertimeDate(string id) => new DatabaseContext().TpRequests.Where(x => x.RequestId.Equals(id)).Select(x => x.RequestDate).FirstOrDefault();

        public List<Dictionary<string, dynamic>> DeleteOvertime(TpRequest value, string id)
        {
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();

            var current = context.TpRequests.Where(x => x.RequestId == value.RequestId).FirstOrDefault() ?? throw new Exception(value.Noted + " not found");

            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        TpRequest deleteOvertime = new TpRequest();
                        deleteOvertime = current;
                        deleteOvertime.DeletedBy = value.DeletedBy;
                        deleteOvertime.DeletedAt = value.DeletedAt;

                        context.TpRequests.Entry(current).CurrentValues.SetValues(deleteOvertime);
                        context.SaveChanges();
                        transaction.Commit();


                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Overtime berhasil dihapus");

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
    public class OvertimeQuery
    {
        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("employee_first_name")]
        public string? EmployeeFirstName { get; set; }

        [Column("employee_last_name")]
        public string? EmployeeLastName { get; set; }

        [Column("request_id")]
        public string? RequestId { get; set; }

        [Column("request_date_view")]
        public string? RequestDateView { get; set; }

        [Column("request_date")]
        public DateTime RequestDate { get; set; }

        [Column("time_start")]
        public string? TimeStart { get; set; }

        [Column("time_end")]
        public TimeSpan? TimeEnd { get; set; }

        [Column("keperluan")]
        public string? Keperluan { get; set; }

        [Column("request_type_id")]
        public string? RequestTypeId { get; set; }

        [Column("my_status")]
        public string? MyStatus { get; set; }

        [Column("noted")]
        public string? Noted { get; set; }

        [Column("time_in_mesin")]
        public string? TimeInMesin { get; set; }

        [Column("request_name")]
        public string? RequestName { get; set; }

        [Column("total_overtime")]
        public string? TotalOvertime { get; set; }

        [Column("my_cut_cuty")]
        public string? MyCutCuty { get; set; }

        [Column("approve_name")]
        public string? ApproveName { get; set; }

        [Column("approve_name2")]
        public string? ApproveName2 { get; set; }

        [Column("dimunculin")]
        public int? Dimunculin { get; set; }

        [Column("status")]
        public int? Status { get; set; }

        [Column("link_over_time_del")]
        public string? linkOverTimeDel { get; set; }

        [Column("link_over_time_app")]
        public string? LinkOverTimeApp { get; set; }

        [Column("time_out")]
        public string? TimeOut { get; set; }
    }

    public class CreateOvertimeModel
    {
        public string EmployeeId { get; set; }
        public string Keperluan { get; set; }
        public string TimeEnd { get; set; }
    }

    public class ApprovalOvertimeModel
    {
        public string ApproveBy { get; set; }
        public string Noted { get; set; }
        public string RequestId { get; set; }
        public int Status { get; set; }
    }

    public class DeleteOvertimeModel
    {
        public string DeletedBy { get; set; }
        public string RequestId { get; set; }
    }

}
