using BackendHrdAgro.Models.Database.MySql;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendHrdAgro.Models.Attendance
{
    public class ReplacementModel
    {
        public List<ReplacementView> ListReplacement(string filter)
        {
            var sql = "SELECT r.replacement_id, r.employee_id, u.user_full_name,   r.request_date,  " +
                "r.description,  r.approve_by, r.approve_status, " +
                "CONCAT(m.employee_first_name, ' ', m.employee_last_name) AS approve_name, " +
                "CASE  WHEN r.approve_status = 1 THEN 'Accepted' " +
                "WHEN r.approve_status = 0 THEN 'Waiting'  " +
                "WHEN r.approve_status = 5 THEN 'Rejected'  " +
                "END AS approve_view, sc.sisa_cuti_replacement," +
                "g.level_id,'' as link_leave_app  FROM tp_replacement_leave r " +
                "left join tm_users u  ON r.employee_id = u.employee_id " +
                "left join tm_employee_affair g on r.employee_id = g.employee_id " +
                "left join tm_employee_affair m ON r.approve_by = m.employee_id " +
                "left join tm_sisa_cuti sc on r.employee_id = sc.employee_id " +
                "join tm_department d on g.department_id = d.department_id " +
                $"join tm_title t on g.title_id=t.title_id {filter} " +
                $"and r.deleted_at IS NULL";
            var list = new DatabaseContext().ReplacementViews.FromSqlRaw(sql).ToList();

            if (list.Count > 0)
            {
                foreach (var l in list)
                {

                    if (l.Detail == null)
                    {
                        l.Detail = new List<ReplacementDetailQuery>();
                    }
                    sql = $"SELECT " +
                          $"a.replacement_date " +
                          $"from tp_replacement_detail a  " +
                          $"where replacement_id='{l.ReplacementId}' order by a.replacement_date DESC";
                    var list_detail = new DatabaseContext().ReplacementDetailQueries.FromSqlRaw(sql).ToList();

                    if (list_detail.Count > 0)
                    {
                        l.Detail.AddRange(list_detail);

                    }

                }

            }

            return list;
        }

        public List<Dictionary<string, dynamic>> CreateReplacements(TpReplacementLeave value, string replacementId, List<string> insertDates)
        {

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            string[] myArrDate;
            string myDate = "";

            List<TpReplacementDetail> insertDetailReplacement = new List<TpReplacementDetail>();

            using DatabaseContext context = new DatabaseContext();

            var executionStrategy = context.Database.CreateExecutionStrategy();
            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {

                    try
                    {
                        context.TpReplacementLeaves.Add(value);
                        context.SaveChanges();

                        string lastId = BaseModel.GenerateId(tableName: "tp_replacement_detail", str: "RD", primaryKey: "id", trailing: 3, lastKey: "NONE");

                        int baseNumber = int.Parse(lastId.Substring(2));

                        foreach (var i in insertDates)
                        {
                            myArrDate = i.Split('-');
                            myDate = myArrDate[2] + '-' + myArrDate[1] + '-' + myArrDate[0];

                            string id = "RD" + baseNumber.ToString("D3");
                            baseNumber++; // increment setelah dipakai

                            TpReplacementDetail detailDate = new TpReplacementDetail()
                            {
                                Id = id,
                                ReplacementId = replacementId,
                                ReplacementDate = DateTime.Parse(myDate),
                            };

                            insertDetailReplacement.Add(detailDate);
                            Console.WriteLine($"✅ Detail ditambahkan: {detailDate.ReplacementDate:yyyy-MM-dd}");
                            //n = n + 1;
                        }
                        context.TpReplacementDetails.AddRange(insertDetailReplacement);
                        context.SaveChanges();

                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil Membuat Replacement.");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("❌ ERROR: " + e.Message);
                        Console.WriteLine("❌ INNER: " + (e.InnerException?.Message ?? "no inner"));
                        Console.WriteLine("❌ STACK: " + e.StackTrace);

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

        public int CountLeaveData(string leaveId) => new DatabaseContext().TpReplacementLeaves.Where(x => x.ReplacementId.Equals(leaveId)).Count();

        public bool CheckIsApprove(string leaveId)
        {
            using (var context = new DatabaseContext())
            {
                var isStatusZero = context.TpReplacementLeaves
                    .Any(x => x.ReplacementId.Equals(leaveId) && x.ApproveStatus.Equals(0));

                return isStatusZero;
            }
        }

        public List<Dictionary<string, dynamic>> ApproveReplacement(TpReplacementLeave value)
        {
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();

            var current = context.TpReplacementLeaves.Where(x => x.ReplacementId == value.ReplacementId).FirstOrDefault() ?? throw new Exception(value.ApproveReason + " not found");

            var currentLeaveRemaining = context.TmSisaCutis.Where(x => x.StatusCuti.Equals(1) && x.EmployeeId.Equals(current.EmployeeId)).FirstOrDefault() ?? throw new Exception(value.ApproveReason + " not found");

            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        TpReplacementLeave approveReplacement = new TpReplacementLeave();
                        approveReplacement = current;
                        approveReplacement.ApproveBy = value.ApproveBy;
                        approveReplacement.ApproveDate = value.ApproveDate;
                        approveReplacement.ApproveReason = value.ApproveReason;
                        approveReplacement.ApproveStatus = value.ApproveStatus;
                        approveReplacement.UpdatedAt = value.UpdatedAt;
                        approveReplacement.UpdatedBy = value.ApproveBy;

                        context.TpReplacementLeaves.Entry(current).CurrentValues.SetValues(approveReplacement);
                        context.SaveChanges();

                        //update saldo tmsisacuti, jika approve ditambahkan ke sisacutireplacement
                        if (value.ApproveStatus == 1)
                        {
                            // hitung jumlah tanggal (row) untuk replacement_id tertentu
                            var jumlahHari = context.TpReplacementDetails
                                .Count(d => d.ReplacementId == value.ReplacementId);

                            if (jumlahHari > 0)
                            {
                                float currentSaldo = currentLeaveRemaining.SisaCutiReplacement;
                                currentLeaveRemaining.SisaCutiReplacement = currentSaldo + jumlahHari;

                                context.TmSisaCutis.Entry(currentLeaveRemaining).CurrentValues.SetValues(currentLeaveRemaining);
                                context.SaveChanges();
                            }
                        }
                        //

                        transaction.Commit();
                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Proses Approval berhasil dilakukan!");

                        result.Clear();
                        result.Add(data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.InnerException == null ? e : e.InnerException);
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

        public List<Dictionary<string, dynamic>> DeleteReplacement(TpReplacementLeave value, string id)
        {
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();

            var current = context.TpReplacementLeaves.Where(x => x.ReplacementId == value.ReplacementId).FirstOrDefault() ?? throw new Exception(value.ReplacementId + " not found");

            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        TpReplacementLeave deleteReplacement = new TpReplacementLeave();
                        deleteReplacement = current;
                        deleteReplacement.DeletedBy = value.DeletedBy;
                        deleteReplacement.DeletedAt = value.DeletedAt;

                        context.TpReplacementLeaves.Entry(current).CurrentValues.SetValues(deleteReplacement);
                        context.SaveChanges();
                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Replacement berhasil dihapus");

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

    public class CreateReplacement
    {
        public string EmployeeId { get; set; }
        public string Description { get; set; }
        public string DateString { get; set; }
    }

    public class ApprovalReplacementModel
    {
        public string replacementId { get; set; }
        public string ApproveBy { get; set; }
        public int ApproveStatus { get; set; }
        public string ApproveReason { get; set; }
    }

    public class DeleteReplacementModel
    {
        public string ReplacementId { get; set; }
        public string DeletedBy { get; set; }
    }

    [Keyless]
    public class ReplacementView
    {
        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("replacement_id")]
        public string ReplacementId { get; set; }

        [Column("user_full_name")]
        public string UserFullName { get; set; }

        [Column("request_date")]
        public DateTime RequestDate { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("approve_by")]
        public string? ApproveBy { get; set; }

        [Column("approve_status")]
        public int ApproveStatus { get; set; }

        [Column("approve_view")]
        public string ApproveView { get; set; }

        [Column("sisa_cuti_replacement")]
        public float? SisaCutiReplacement { get; set; }

        [Column("link_leave_app")]
        public string? LinkLeaveApp { get; set; }

        [Column("approve_name")]
        public string? ApproveName { get; set; }

        public List<ReplacementDetailQuery> Detail { get; set; }

    }

    [Keyless]
    public class ReplacementDetailQuery
    {
        [Column("replacement_date")]
        public DateTime ReplacementDate { get; set; }
    }

}
