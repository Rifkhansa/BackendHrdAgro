using BackendHrdAgro.Models.Database.MySql.Support;
using BackendHrdAgro.Models.Database.MySql;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendHrdAgro.Models
{
    public class DashboardDB
    {
        DatabaseContext context = new DatabaseContext();
        public List<GetSetMenu> getSetMenu(string url)
        {
            try
            {
                var res2 = from c in context.TmMenus
                           where c.Url == url
                           join p in context.TmMenus on c.Parent equals p.Id into kk
                           from k in kk.DefaultIfEmpty()
                           select new GetSetMenu
                           {
                               Id = c.Id,
                               Name = c.Name,
                               Caption = c.Caption,
                               MyParent = k == null ? null : k.Caption
                           };
                return res2.ToList();
            }
            catch (Exception)
            {
                throw;
            }

        }
        public string generateMenu(string parent, string userId, string setMenuParent, string setMenuChild, string baseUrl)
        {
            List<TmMenu> Menus = new List<TmMenu>();
            var menu = string.Empty;

            if (parent == null)
            {
                Console.WriteLine("null parent");
                var sql = from i in context.TmMenus
                          where i.DeletedAt == null && i.DeletedBy == null && i.Parent == null
                          orderby i.Parent, i.Hierarchy, i.Caption, i.Name ascending
                          select i;
                Menus = sql.ToList();
            }
            else
            {
                Console.WriteLine("available parent");
                var sql = from i in context.TmMenus
                          where i.DeletedAt == null && i.DeletedBy == null && i.Parent == parent
                          orderby i.Parent, i.Hierarchy, i.Caption, i.Name ascending
                          select i;
                Menus = sql.ToList();
            }

            try
            {
                foreach (var i in Menus)
                {
                    if (i.Url != "#")
                    {
                        Console.WriteLine(i.Url);
                        var url = baseUrl + i.Url;
                        //Console.WriteLine(url);
                        menu += $"<li><a class='nav-link' href='{url}'>{i.Caption}</a>";
                    }
                    else
                    {
                        menu += $"<li class='nav nav-parent'><a class='nav-link' href='#'>{i.Caption}</a>";
                    }
                    var men = generateMenu(i.Id, userId, i.Parent, i.Caption, baseUrl);
                    menu += $"<ul class='nav nav-children'>{men}</ul>";
                    menu += "</li>";
                }
                Console.WriteLine(menu);
                return menu;

            }
            catch (Exception e)
            {

                return null;
            }

        }

        //index
        public List<Dashboardpresence> GetPresence(string titleId, string employeeId, string departmentId, string levelId, string divId)
        {
            using DatabaseContext context = new DatabaseContext();
            try
            {
                var today = DateTime.Now.ToString("yyyy/MM/dd");
                var myCriteria = "";

                var arrayAsmen = new[] { "0615041", "0913021" };
                if (titleId == "DS002" || arrayAsmen.Contains(employeeId)) // manager
                {
                    if (departmentId == "DP006" || employeeId == "0808003") // hrd
                    {
                        myCriteria = "";
                    }
                    else if (titleId == "DS002" && levelId == "TL019")
                    {
                        myCriteria = $" and h.div_id='{divId}'";
                    }
                    else
                    {
                        myCriteria = $" and a.department_id='{departmentId}'";
                    }
                }
                else
                {
                    if (departmentId == "DP006" && titleId == "DS003")
                    {
                        myCriteria = "";
                    }
                    else
                    {
                        myCriteria = $" and a.employee_id='{employeeId}'";
                    }
                }

                var query = $"SELECT CASE WHEN time_in IS NULL THEN 1 WHEN time_in = '' THEN 2 ELSE 3 END AS order_by, a.employee_first_name, a.employee_last_name, CASE WHEN time_in IS NULL THEN '--:--:--' WHEN time_in = '' THEN '--:--:--' ELSE time_in END AS time_in, CASE WHEN time_out IS NULL THEN '--:--:--' WHEN time_out = '' THEN '--:--:--' ELSE time_out END AS time_out, b.absent_date FROM tm_employee_affair a LEFT OUTER JOIN tp_absent b ON a.employee_id = b.employee_id AND b.absent_date = '{today}' LEFT OUTER JOIN tm_department h ON a.department_id = h.department_id WHERE a.status = 1 {myCriteria} ORDER BY order_by DESC, time_in ASC";

                //var resultList = query.ToList();

                var list = new DatabaseContext().Dashboardpresences.FromSqlRaw(query).ToList();

                return list;


            }
            catch (Exception)
            {

                throw;
            }
        }


        /*public int GetJumlahPunishment()
        {
            return new DatabaseContext().TpWarnings.Count();
        }*/
        public int GetJumlahApproval(string departmentId)
        {
            var count = (from req in context.TpRequests
                         join afair in context.TmEmployeeAffairs
                         on req.EmployeeId equals afair.EmployeeId
                         where req.Status == 0 && afair.DepartmentId == $"{departmentId}"
                         && req.RequestDate.Month == DateTime.Now.Month
                         select req).Count();

            return count;

        }
        public int GetJumlahTermination()
        {
            return new DatabaseContext().TpTerminations
    .Where(t => DateTime.Now <= t.TerminationDate.AddMonths(1))
    .Count();
        }
        public int GetJumlahApprovalCuti(string departmentId, string employeeId)
        {
            return context.TpCutis
              .Where(cuti => cuti.Status == 0)
              .Join(context.TmEmployeeAffairs, a => a.EmployeeId, b => b.EmployeeId, (a, b) => new { a, b })
              .Where(x => x.b.DepartmentId == departmentId && x.a.EmployeeId != employeeId && x.a.DeletedAt == null)
              .GroupBy(x => x.b.DepartmentId)
              .Select(group => group.Count())
              .FirstOrDefault();

          /*  return new DatabaseContext().TpCutis
                .Where(cuti => cuti.Status == 0)
                .Count();*/
        }

        /*public int GetJumlahApprovalPermission(string departmentId)
        {
            return context.TpPermissions
              .Where(i => i.Status == 0)
              .Join(context.TmEmployeeAffairs, a => a.EmployeeId, b => b.EmployeeId, (a, b) => new { a, b })
              .Where(x => x.b.DepartmentId == departmentId && x.a.DeletedAt == null)
              .GroupBy(x => x.b.DepartmentId)
              .Select(group => group.Count())
              .FirstOrDefault();
        }*/
        //
        public int GetExtendAmount()
        {
            /*   var sql = "";
               sql = $"select count(employee_id) as jumlah_extend from tm_employee_affair where DATE_FORMAT(end_of_contract,'%Y-%m-%d')>= DATE_FORMAT(now(),'%Y-%m-%d') and DATE_FORMAT(DATE_ADD(end_of_contract, INTERVAL -2 MONTH),'%Y-%m-%d')<= DATE_FORMAT(now(),'%Y-%m-%d')";

               var result = context.AmountExtends.FromSqlRaw(sql).FirstOrDefault().ToString();*/

            return new DatabaseContext().TmEmployeeAffairs.Where(e => e.EndOfContract >= DateTime.Now.Date &&
                        e.EndOfContract.AddMonths(-2) <= DateTime.Now.Date).Count();

        }
        public int GetIncomingAmount(string employeeId)
        {
            return new DatabaseContext().TpIncomingLetters
    .Where(l => l.IsAccepted == 0 && l.ReceiptId == employeeId)
    .Count();

        }

        public float GetJumlahCutiLong(string employeeId)
        {
            return new DatabaseContext().TmSisaCutis.Where(c => c.EmployeeId == employeeId).Select(c => c.SisaCutiLong).FirstOrDefault();

        }
        public float GetJumlahCutiAnnual(string employeeId)
        {
            return new DatabaseContext().TmSisaCutis
            .Where(c => c.EmployeeId == employeeId)
            .Select(c => c.SisaCutiAnnual)
            .FirstOrDefault();

        }
        public float GetJumlahApprovalCutiDiv(string employeeId, string departmentId)
        {
            return context.TpCutis
                .Where(cuti => cuti.Status == 0)
                .Join(context.TmEmployeeAffairs, a => a.EmployeeId, b => b.EmployeeId, (a, b) => new { a, b })
                .Where(x => x.b.DepartmentId == departmentId)
                .GroupBy(x => x.b.DepartmentId)
                .Select(group => group.Count())
                .FirstOrDefault();

        }
        public float GetJumlahApprovalDiv(string employeeId, string departmentId)
        {

            return context.TpRequests
                .Where(request => request.Status == 0)
                .Join(context.TmEmployeeAffairs, a => a.EmployeeId, b => b.EmployeeId, (a, b) => new { a, b })
                .Where(x => x.b.DepartmentId == departmentId)
                .GroupBy(x => x.b.DepartmentId)
                .Select(group => group.Count())
                .FirstOrDefault();

        }
        public int GetJumlahRequestIT(string employeeId, string departmentId)
        {
            return new DatabaseContext().TpFrqItServices
                .Where(itService => itService.Status == 10 && itService.UserIdAssigned == employeeId)
                .Count();

        }


        public int GetJumlahRequestCuti(string employeeId)
        {
            return new DatabaseContext().TpCutis
                .Where(cuti => cuti.Status == 0 && cuti.EmployeeId == employeeId)
                .Count();

        }
        public int GetJumlahRequest(string employeeId)
        {
            return new DatabaseContext().TpRequests
                .Where(request => request.Status == 0 && request.EmployeeId == employeeId)
                .Count();

        }
        public int GetJumlahUltah(string employeeId)
        {
           /* return new DatabaseContext().TmEmployeeAffairs
                .Where(employee => employee.TanggalLahir.Month == DateTime.Now.Month)
                .Count();*/ //ini perbulan
            return new DatabaseContext().TmEmployeeAffairs
    .Where(e => e.TanggalLahir.Month == DateTime.Now.Month && e.TanggalLahir.Day == DateTime.Now.Day)
    .Count();

        }
        /* public int GetPresentTotal(string employeeId)
         {
             *//* // Query SQL untuk menghitung kehadiran
              var query = $"SELECT COUNT(b.absent_date) AS total_kehadiran " +
                          $"FROM tm_employee_affair a " +
                          $"LEFT OUTER JOIN tp_absent b ON a.employee_id = b.employee_id " +
                          $"AND DATE_FORMAT(b.absent_date, '%Y-%m') = '2022-12' " +
                          $"WHERE a.employee_id = '1022091'";

              // Menjalankan query dan mengambil hasil berupa integer
              var result = new DatabaseContext()
                  .Database
                  .ExecuteSqlRaw(query);

              // Mengembalikan hasil sebagai integer
              return result;*//*

             return new DatabaseContext().TpAbsents
     .Where(absent => absent.EmployeeId == "1022091"
         && absent.AbsentDate.Year == 2022
         && absent.AbsentDate.Month == 12)
     .Count();
         }*/

        public int GetPresentTotal(string employeeId)
        {
            // Mengambil bulan dan tahun saat ini
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int today = DateTime.Now.Day; // Mendapatkan hari ini

            // Hitung jumlah hari kerja dari tanggal 1 hingga hari ini (tanpa Sabtu dan Minggu)
            int totalWorkingDays = Enumerable.Range(1, today) // Dari tanggal 1 sampai hari ini di bulan ini
                .Select(day => new DateTime(year, month, day)) // Ubah menjadi DateTime
                .Count(date => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday); // Hanya hitung hari kerja (bukan Sabtu/Minggu)

            // Gunakan satu instance dari DatabaseContext
            using (var context = new DatabaseContext())
            {
                // Hitung jumlah kehadiran dari database di bulan yang sama
                int totalKehadiran = context.TpAbsents
                    .Where(absent => absent.EmployeeId == employeeId
                        && absent.AbsentDate.Year == year    // Filter berdasarkan tahun ini
                        && absent.AbsentDate.Month == month  // Filter berdasarkan bulan ini
                        && absent.AbsentDate.Day <= today    // Hingga hari ini
                        && absent.AbsentDate.DayOfWeek != DayOfWeek.Saturday // Abaikan Sabtu
                        && absent.AbsentDate.DayOfWeek != DayOfWeek.Sunday)  // Abaikan Minggu
                    .Count();

                // Hitung persentase kehadiran
                double presentPercentage = (double)totalKehadiran / totalWorkingDays * 100;
                Console.WriteLine("total present: " + totalKehadiran);
                Console.WriteLine("workday: " + totalWorkingDays);
                Console.WriteLine("present percentage: " + presentPercentage);

                return Convert.ToInt32(presentPercentage);
            }
        }

        /*public int GetPermissionTotal(string employeeId)
        {
            // Mengambil bulan dan tahun saat ini
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int today = DateTime.Now.Day; // Mendapatkan hari ini

            // Hitung jumlah hari kerja dari tanggal 1 hingga hari ini (tanpa Sabtu dan Minggu)
            int totalWorkingDays = Enumerable.Range(1, today) // Dari tanggal 1 sampai hari ini di bulan ini
                .Select(day => new DateTime(year, month, day)) // Ubah menjadi DateTime
                .Count(date => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday); // Hanya hitung hari kerja (bukan Sabtu/Minggu)

            // Buat satu instance DatabaseContext untuk digunakan di seluruh query
            using (var context = new DatabaseContext())
            {
                int totalPermission = context.TpPermissions
              .Where(permission => permission.EmployeeId == employeeId)
              .Join(context.TpPermissionDetails,
                    permission => permission.PermissionId,  // Key di tabel TpPermissions
                    detail => detail.PermissionId,          // Key di tabel TpPermissionDetails
                    (permission, detail) => detail)
              .Count(detail => detail.PermissionDate != null &&
                               detail.PermissionDate.Year == year &&    // Filter berdasarkan tahun ini
                               detail.PermissionDate.Month == month &&  // Filter berdasarkan bulan ini
                               detail.PermissionDate.Day <= today);     // Filter hingga hari ini


                double presentPercentage = (double)totalPermission / totalWorkingDays * 100;

                Console.WriteLine("total permission: " + totalPermission);
                Console.WriteLine("workday: " + totalWorkingDays);
                Console.WriteLine("present percentage: " + presentPercentage);

                return Convert.ToInt32(presentPercentage);
            }
        }*/


        public int GetAbsentTotal(string employeeId)
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int today = DateTime.Now.Day;

            int totalWorkingDays = Enumerable.Range(1, today)
                .Select(day => new DateTime(year, month, day))
                .Count(date => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday);

            using (var context = new DatabaseContext())
            {
                int totalPresent = context.TpAbsents
                    .Where(absent => absent.EmployeeId == employeeId
                        && absent.AbsentDate.Year == year
                        && absent.AbsentDate.Month == month
                        && absent.AbsentDate.Day <= today
                        && absent.AbsentDate.DayOfWeek != DayOfWeek.Saturday
                        && absent.AbsentDate.DayOfWeek != DayOfWeek.Sunday)
                    .Count();


                /*int totalPermission = context.TpPermissions
                    .Where(permission => permission.EmployeeId == employeeId)
                    .Join(context.TpPermissionDetails,
                            permission => permission.PermissionId,
                            detail => detail.PermissionId,
                            (permission, detail) => detail)
                    .Count(detail => detail.PermissionDate != null &&
                             detail.PermissionDate.Year == year &&
                             detail.PermissionDate.Month == month &&
                             detail.PermissionDate.Day <= today);*/


                //int totalAbsent = totalWorkingDays - (totalPresent + totalPermission);
                int totalAbsent = totalWorkingDays - totalPresent;
                double presentPercentage = (double)totalAbsent / totalWorkingDays * 100;
                Console.WriteLine("total absent: " + GetPresentTotal(employeeId));
                Console.WriteLine("total absent: " + totalAbsent);
                Console.WriteLine("workday: " + totalWorkingDays);
                Console.WriteLine("present percentage: " + presentPercentage);

                return Convert.ToInt32(presentPercentage);
            }
        }



    }

    //index
    public enum RetriveAction
    {
        All,
        EmployeeId,
        DepartementId,
        DivisionId
    }
    
    public class DashboardShowMenuModel
    {
        [Required] public string baseUrl { get; set; } = null!;
        [Required] public string extendedUrl { get; set; } = null!;
    }
    public class GetSetMenu
    {
        [Column("id")]
        public string Id { get; set; }
        [Column("name")]
        public string? Name { get; set; }
        [Column("caption")]
        public string? Caption { get; set; }
        [Column("my_parent")]
        public string? MyParent { get; set; }
    }
    public partial class DashboardBniModel
    {
        [Required] public string locId { get; set; } = null!;
        public string? insuranceId { get; set; }
    }

    [Keyless]
    public partial class DashboardFindThisMonth
    {
        [Column("my_production")]
        public int? MyProduction { get; set; }
        [Column("my_claim")]
        public int? MyClaim { get; set; }
        [Column("my_restitusi")]
        public int? MyRestitusi { get; set; }
        [Column("my_payment")]
        public int? MyPayment { get; set; }
    }
    [Keyless]
    public partial class DashboardFindPaymentClaim
    {
        [Column("amount", TypeName = "money")]
        public decimal? Amount { get; set; }
    }
    [Keyless]
    public partial class DashboardFindLimitStopLoss
    {
        [Column("premium", TypeName = "decimal(38, 0)")]
        public decimal? Premium { get; set; }
        [Column("klaim", TypeName = "money")]
        public decimal? Klaim { get; set; }
        [Column("subrogasi", TypeName = "money")]
        public decimal? Subrogasi { get; set; }
        [Column("rasio", TypeName = "decimal(38, 23)")]
        public decimal? Rasio { get; set; }
    }
    [Keyless]
    public partial class DashboardFindClaimTimePeriod
    {
        [Column("broker_id")]
        [StringLength(3)]
        [Unicode(false)]
        public string? BrokerId { get; set; }
        [Column("broker_name")]
        [StringLength(10)]
        [Unicode(false)]
        public string? BrokerName { get; set; }
        [Column("insurance_id")]
        [StringLength(3)]
        [Unicode(false)]
        public string? InsuranceId { get; set; }
        [Column("insurance_short_name")]
        [StringLength(20)]
        [Unicode(false)]
        public string? InsuranceShortName { get; set; }
        [Column("a0_30days")]
        public int? A030days { get; set; }
        [Column("a31_60days")]
        public int? A3160days { get; set; }
        [Column("a61_90days")]
        public int? A6190days { get; set; }
        [Column("more90days")]
        public int? More90days { get; set; }
    }
    [Keyless]
    public partial class DashboardFindClaimReserveSubro
    {
        [Column("module_name")]
        [StringLength(14)]
        [Unicode(false)]
        public string ModuleName { get; set; } = null!;
        [Column("total_in_askrida", TypeName = "money")]
        public decimal? TotalInAskrida { get; set; }
        [Column("total_out_askrida", TypeName = "money")]
        public decimal? TotalOutAskrida { get; set; }
        [Column("total_askrida", TypeName = "money")]
        public decimal? TotalAskrida { get; set; }
        [Column("total_in_tripa", TypeName = "money")]
        public decimal? TotalInTripa { get; set; }
        [Column("total_out_tripa", TypeName = "money")]
        public decimal? TotalOutTripa { get; set; }
        [Column("total_tripa", TypeName = "money")]
        public decimal? TotalTripa { get; set; }
        [Column("total_in_aca", TypeName = "money")]
        public decimal? TotalInAca { get; set; }
        [Column("total_out_aca", TypeName = "money")]
        public decimal? TotalOutAca { get; set; }
        [Column("total_aca", TypeName = "money")]
        public decimal? TotalAca { get; set; }
    }
    [Keyless]
    public partial class DashboardFindClaimPerformanceQty
    {
        [Column("broker_id")]
        [StringLength(3)]
        [Unicode(false)]
        public string? BrokerId { get; set; }
        [Column("broker_name")]
        [StringLength(10)]
        [Unicode(false)]
        public string? BrokerName { get; set; }
        [Column("insurance_id")]
        [StringLength(3)]
        [Unicode(false)]
        public string? InsuranceId { get; set; }
        [Column("insurance_short_name")]
        [StringLength(20)]
        [Unicode(false)]
        public string? InsuranceShortName { get; set; }
        [Column("claim_incurred")]
        public int? ClaimIncurred { get; set; }
        [Column("claim_reject")]
        public int? ClaimReject { get; set; }
        [Column("claim_process")]
        public int? ClaimProcess { get; set; }
        [Column("claim_paid")]
        public int? ClaimPaid { get; set; }
    }
    [Keyless]
    public partial class DashboardFindClaimPerformanceAmount
    {
        [Column("broker_id")]
        [StringLength(3)]
        [Unicode(false)]
        public string? BrokerId { get; set; }
        [Column("broker_name")]
        [StringLength(10)]
        [Unicode(false)]
        public string? BrokerName { get; set; }
        [Column("insurance_id")]
        [StringLength(3)]
        [Unicode(false)]
        public string? InsuranceId { get; set; }
        [Column("insurance_short_name")]
        [StringLength(20)]
        [Unicode(false)]
        public string? InsuranceShortName { get; set; }
        [Column("claim_incurred", TypeName = "money")]
        public decimal? ClaimIncurred { get; set; }
        [Column("claim_reject", TypeName = "money")]
        public decimal? ClaimReject { get; set; }
        [Column("claim_process", TypeName = "money")]
        public decimal? ClaimProcess { get; set; }
        [Column("claim_paid", TypeName = "money")]
        public decimal? ClaimPaid { get; set; }
        [Column("claim_paid_own", TypeName = "money")]
        public decimal? ClaimPaidOwn { get; set; }
    }
    [Keyless]
    public partial class DashboardSumPolicy
    {
        [Column("broker_id")]
        [StringLength(15)]
        [Unicode(false)]
        public string? BrokerId { get; set; }
        [Column("broker_name")]
        [StringLength(10)]
        [Unicode(false)]
        public string? BrokerName { get; set; }
        [Column("insurance_id")]
        [StringLength(21)]
        [Unicode(false)]
        public string InsuranceId { get; set; } = null!;
        [Column("insurance_short_name")]
        [StringLength(20)]
        [Unicode(false)]
        public string? InsuranceShortName { get; set; }
        [Column("qty_pk_finish")]
        public int? QtyPkFinish { get; set; }
        [Column("coverage_pk_finish", TypeName = "money")]
        public decimal? CoveragePkFinish { get; set; }
        [Column("qty_request_claim")]
        public int? QtyRequestClaim { get; set; }
        [Column("coverage_request_claim", TypeName = "money")]
        public decimal? CoverageRequestClaim { get; set; }
        [Column("qty_non_request_claim")]
        public int? QtyNonRequestClaim { get; set; }
        [Column("coverage_non_request_claim", TypeName = "money")]
        public decimal? CoverageNonRequestClaim { get; set; }
    }
    [Keyless]
    public partial class DashboardSumClaimSla
    {
        [Column("insurance_id")]
        [StringLength(3)]
        [Unicode(false)]
        public string? InsuranceId { get; set; }
        [Column("insurance_short_name")]
        [StringLength(20)]
        [Unicode(false)]
        public string? InsuranceShortName { get; set; }
        [Column("pengajuan_30")]
        public int? Pengajuan30 { get; set; }
        [Column("pengajuan_30_more")]
        public int? Pengajuan30More { get; set; }
        [Column("kelengkapan_20")]
        public int? Kelengkapan20 { get; set; }
        [Column("kelengkapan_20_more")]
        public int? Kelengkapan20More { get; set; }
    }

    [Keyless]
    public partial class DashboardFindEarnedPremi
    {
        [Column("earned_premi")]
        public double? EarnedPremi { get; set; }
        [Column("klaim_paid", TypeName = "money")]
        public decimal? KlaimPaid { get; set; }
        [Column("subrogasi", TypeName = "money")]
        public decimal? Subrogasi { get; set; }
        [Column("rasio")]
        public double? Rasio { get; set; }
    }
}
