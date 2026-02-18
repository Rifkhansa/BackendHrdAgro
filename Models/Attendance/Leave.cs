using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Azure.Core;
using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models;
using BackendHrdAgro.Models.Database.MySql;
using BackendHrdAgro.Models.Database.MySql.Support;
using BackendHrdAgro.Models.Database.MySql.View;
using BackendHrdAgro.Models.Employee;
using BackendHrdAgro.Models.Permission;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BackendHrdAgro.Models.Leave
{
    public class LeaveDB
    {
        public List<TmTypeCuti> LeaveTypeFindOnlyReplacement() => new DatabaseContext().TmTypeCutis.Where(x => x.Status == 1 && x.TypeCutiId.Equals("CUT003")).ToList();

        public List<TmTypeCuti> LeaveTypeFindOnlyAnnual()
        {
            var annualIds = new[] { "CUT001", "CUT005", "CUT006" };

            return new DatabaseContext().TmTypeCutis
                .Where(x => x.Status == 1 && annualIds.Contains(x.TypeCutiId))
                .ToList();
        }

        public List<TmTypeCuti> LeaveTypeFindOnlyLong() => new DatabaseContext().TmTypeCutis.Where(x => x.Status.Equals(1) && !x.TypeCutiId.Equals("CUT001")).ToList();
        public List<TmTypeCuti> LeaveTypeFind() => new DatabaseContext().TmTypeCutis.Where(x => x.Status.Equals(1)).ToList();
        public List<TmTypeCuti> LeaveTypeFind(string id) => new DatabaseContext().TmTypeCutis.Where(x => x.Status.Equals(1) && x.TypeCutiId.Equals(id)).ToList();

        public static string? GetEmployee(string leaveId) => new DatabaseContext().TpCutis.Where(x => x.CutiId.Equals(leaveId)).Select(x => x.EmployeeId).FirstOrDefault();

        public static string? GetLeaveType(string id) => new DatabaseContext().TmTypeCutis.Where(x => x.TypeCutiId.Equals(id)).Select(x => x.NamaCuti).FirstOrDefault();

        public static string? GetLeaveTypeId(string leaveId) => new DatabaseContext().TpCutis.Where(x => x.CutiId.Equals(leaveId)).Select(x => x.TypeCutiId).FirstOrDefault();

        public List<TmSisaCuti> LeaveRemaining(string id) => new DatabaseContext().TmSisaCutis.Where(x => x.StatusCuti.Equals(1) && x.EmployeeId.Equals(id)).ToList();

        public List<TpDetailCuti> ListLeaveDetail(string leaveId) => new DatabaseContext().TpDetailCutis.Where(x => x.Status.Equals(1) && x.CutiId.Equals(leaveId)).ToList();

        public int CountLeaveData(string leaveId) => new DatabaseContext().TpCutis.Where(x => x.CutiId.Equals(leaveId)).Count();

        public bool CheckIsApprove(string leaveId)
        {
            using (var context = new DatabaseContext())
            {
                // Cek apakah ada record dengan leaveId dan status 0
                var isStatusZero = context.TpCutis
                    .Any(x => x.CutiId.Equals(leaveId) && x.Status.Equals(0));

                // Jika ada status 0, kembalikan false, jika tidak, kembalikan true
                return isStatusZero;
            }
        }

        public List<LeaveQuery> ListLeave(string filter)
        {
            string sql = "";

            sql = $"SELECT " +
                $"a.cuti_id,a.employee_id, a.jumlah_cuti,a.keperluan,a.noted,a.status," +
                $"DATE_FORMAT(a.request_date, '%d %M %Y') as request_view, " +
                $"concat(b.employee_first_name, ' ', b.employee_last_name) as employee_name, c.nama_cuti, " +
                $"case when a.status = 1 then 'Accepted' when a.status=0 then 'waiting' when a.status=5 then 'Rejected' end as my_status, " +
                $"concat(d.employee_first_name,' ',d.employee_last_name) as approve_name, " +
                $"e.sisa_cuti_annual,e.sisa_cuti_Maternity,e.sisa_cuti_long,e.sisa_cuti_replacement, " +
                $"case when a.type_cuti_id = 1 then case when DATE_FORMAT(a.request_date, '%d %M %Y') = DATE_FORMAT(now(), '%d %M %Y') then 1 else 0 end else 1 end as dimunculin, " +
                $"b.level_id,'' as link_leave_del,'' as link_leave_app " +
                $"from tp_cuti a  " +
                $"left outer join tm_employee_affair b on a.employee_id = b.employee_id " +
                $"left outer join tm_type_cuti c on a.type_cuti_id = c.type_cuti_id " +
                $"left outer join tm_employee_affair d on a.approve_by = d.employee_id " +
                $"left outer join tm_sisa_cuti e on a.employee_id = e.employee_id " +
                $"left outer join tm_department h on b.department_id = h.department_id " +
                $"where a.status in (1, 0, 5) {filter} and a.deleted_at is null and b.status in (1) " +
                $"order by status asc, a.request_date DESC LIMIT 200";
            //test

            var list = new DatabaseContext().LeaveQueries.FromSqlRaw(sql).ToList();

            if (list.Count > 0)
            {
                foreach (var l in list)
                {
                    if (l.Detail == null)
                    {
                        l.Detail = new List<LeaveDetailQuery>();
                    }
                    sql = $"SELECT " +
                          $"a.cuti_date " +
                          $"from tp_detail_cuti a  " +
                          $"where cuti_id='{l.CutiId}' order by a.cuti_date DESC";
                    var list_detail = new DatabaseContext().LeaveDetailQueries.FromSqlRaw(sql).ToList();

                    if (list_detail.Count > 0)
                    {
                        l.Detail.AddRange(list_detail);

                    }

                }


            }
            return list;
        }

        public List<TotalLeave> TotalLeave(string departmentId, string employeeId)
        {

            string sql = "";

            string filter = "";
            if (departmentId != null && departmentId != "DP003" && employeeId != "010116" && departmentId != "DP004")
            {
                filter = $"and b.department_id  = '{departmentId}'";
            }
            sql = $"SELECT b.employee_first_name,b.employee_last_name,a.employee_id, a.sisa_cuti_annual, a.sisa_cuti_long FROM tm_sisa_cuti a inner JOIN tm_employee_affair b on a.employee_id = b.employee_id WHERE b.status = 1 {filter} order by b.employee_first_name ASC;";
            //test

            var list = new DatabaseContext().TotalLeaves.FromSqlRaw(sql).ToList();


            return list;
        }

        public List<Dictionary<string, dynamic>> CreateLeave(TpCuti value, String leaveId, float leaveTypeDay, List<string> insertDates)
        {
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            string[] myArrDate;
            string myDate = "";
            int n = 0;
            string leaveDetailId = "";

            List<TpDetailCuti> insertDetailCutiList = new List<TpDetailCuti>();

            using DatabaseContext context = new DatabaseContext();

            var executionStrategy = context.Database.CreateExecutionStrategy();
            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.TpCutis.Add(value);
                        context.SaveChanges();

                        foreach (var i in insertDates)
                        {
                            myArrDate = i.Split('-');
                            myDate = myArrDate[2] + '-' + myArrDate[1] + '-' + myArrDate[0];

                            if (n == 0)
                            {
                                leaveDetailId = BaseModel.GenerateId(tableName: "tp_detail_cuti", str: "RCD", primaryKey: "id_detail_cuti", trailing: 4, lastKey: "NONE", date: DateTime.Now.ToString("yyMM"));
                            }
                            else
                            {
                                leaveDetailId = BaseModel.GenerateId(tableName: "tp_detail_cuti", str: "RCD", primaryKey: "id_detail_cuti", trailing: 4, lastKey: leaveDetailId, date: DateTime.Now.ToString("yyMM"));
                            }

                            TpDetailCuti detailDate = new TpDetailCuti()
                            {
                                IdDetailCuti = leaveDetailId,
                                CutiId = leaveId,
                                CutiDate = DateTime.Parse(myDate),
                                Qty = leaveTypeDay,
                                Status = 1
                            };

                            insertDetailCutiList.Add(detailDate);
                            n = n + 1;
                        }
                        context.TpDetailCutis.AddRangeAsync(insertDetailCutiList);
                        context.SaveChanges();

                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil Membuat pengajuan cuti");

                        result.Clear();
                        result.Add(data);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.InnerException == null ? e : e.InnerException);
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

        public List<Dictionary<string, dynamic>> ApproveLeave(TpCuti value, string id)
        {
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();

            var current = context.TpCutis.Where(x => x.CutiId == value.CutiId).FirstOrDefault() ?? throw new Exception(value.Noted + " not found");

            var currentLeaveRemaining = context.TmSisaCutis.Where(x => x.StatusCuti.Equals(1) && x.EmployeeId.Equals(current.EmployeeId)).FirstOrDefault() ?? throw new Exception(value.Noted + " not found");

            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        TpCuti approveLeave = new TpCuti();
                        approveLeave = current;
                        approveLeave.ApproveBy = value.ApproveBy;
                        approveLeave.ApproveDate = value.ApproveDate;
                        approveLeave.Noted = value.Noted;
                        approveLeave.Status = value.Status;
                        approveLeave.IsNotify = value.IsNotify;
                        approveLeave.UpdatedAt = value.UpdatedAt;
                        approveLeave.UpdatedBy = value.ApproveBy;

                        context.TpCutis.Entry(current).CurrentValues.SetValues(approveLeave);
                        context.SaveChanges();

                        float calc = 0;
                        TmSisaCuti leaveRemaining = new TmSisaCuti();
                        leaveRemaining = currentLeaveRemaining;
                        if (current.TypeCutiId == "CUT001")
                        {
                            if (value.Status == 1)
                            {
                                calc = leaveRemaining.SisaCutiAnnual - current.JumlahCuti;
                                leaveRemaining.SisaCutiAnnual = calc;

                                context.TmSisaCutis.Entry(currentLeaveRemaining).CurrentValues.SetValues(leaveRemaining);
                                context.SaveChanges();
                            }
                        }
                        else if (current.TypeCutiId == "CUT003")
                        {
                            if (value.Status == 1)
                            {
                                calc = leaveRemaining.SisaCutiLong - current.JumlahCuti;
                                leaveRemaining.SisaCutiLong = calc;

                                context.TmSisaCutis.Entry(currentLeaveRemaining).CurrentValues.SetValues(leaveRemaining);
                                context.SaveChanges();

                            }
                        }


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

        public List<Dictionary<string, dynamic>> DeleteLeave(TpCuti value, string id)
        {
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();

            var current = context.TpCutis.Where(x => x.CutiId == value.CutiId).FirstOrDefault() ?? throw new Exception(value.CutiId + " not found");

            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        TpCuti deleteLeave = new TpCuti();
                        deleteLeave = current;
                        deleteLeave.DeletedBy = value.DeletedBy;
                        deleteLeave.DeletedAt = value.DeletedAt;

                        context.TpCutis.Entry(current).CurrentValues.SetValues(deleteLeave);
                        context.SaveChanges();
                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Leave berhasil dihapus");

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

        public DateTime findLeaveDate(string cutiId) => new DatabaseContext().TpCutis.Where(x => x.CutiId.Equals(cutiId)).Select(x => x.RequestDate).FirstOrDefault();

        public List<LeaveQuery> CheckApprovalStatus()
        {
            var sql = $"SELECT " +
                $"a.cuti_id,a.employee_id, a.jumlah_cuti,a.keperluan,a.noted,a.status," +
                $"DATE_FORMAT(a.request_date, '%d %M %Y') as request_view, " +
                $"concat(b.employee_first_name, ' ', b.employee_last_name) as employee_name, c.nama_cuti, " +
                $"case when a.status = 1 then 'Accepted' when a.status=0 then 'waiting' when a.status=5 then 'Rejected' end as my_status, " +
                $"concat(d.employee_first_name,' ',d.employee_last_name) as approve_name, " +
                $"e.sisa_cuti_annual,e.sisa_cuti_Maternity,e.sisa_cuti_long, " +
                $"case when a.type_cuti_id = 1 then case when DATE_FORMAT(a.request_date, '%d %M %Y') = DATE_FORMAT(now(), '%d %M %Y') then 1 else 0 end else 1 end as dimunculin, " +
                $"b.level_id,'' as link_leave_del,'' as link_leave_app " +
                $"from tp_cuti a  " +
                $"left outer join tm_employee_affair b on a.employee_id = b.employee_id " +
                $"left outer join tm_type_cuti c on a.type_cuti_id = c.type_cuti_id " +
                $"left outer join tm_employee_affair d on a.approve_by = d.employee_id " +
                $"left outer join tm_sisa_cuti e on a.employee_id = e.employee_id " +
                $"left outer join tm_department h on b.department_id = h.department_id " +
                $"where a.status = 0 and a.deleted_at is null and b.status in (1) " +
                $"order by status asc, a.request_date DESC";
            var list = new DatabaseContext().LeaveQueries.FromSqlRaw(sql).ToList();
            return list;
        }

        public DateTime FindLeaveDate(string id) => new DatabaseContext().TpCutis.Where(x => x.CutiId.Equals(id)).Select(x => x.RequestDate).FirstOrDefault();

    }

}


[Keyless]
public class LeaveQuery
{

    [Column("cuti_id")]
    public string CutiId { get; set; }

    [Column("employee_id")]
    public string EmployeeId { get; set; }

    [Column("request_view")]
    public string RequestView { get; set; }

    [Column("jumlah_cuti")]
    public float JumlahCuti { get; set; }

    [Column("keperluan")]
    public string? Keperluan { get; set; }

    [Column("noted")]
    public string? Noted { get; set; }

    [Column("status")]
    public int Status { get; set; }

    /* [Column("is_cut")]
     public int IsCut { get; set; }*/

    [Column("nama_cuti")]
    public string? NamaCuti { get; set; }

    [Column("employee_name")]
    public string EmployeeName { get; set; }

    [Column("my_status")]
    public string MyStatus { get; set; }

    [Column("approve_name")]
    public string? ApproveName { get; set; }

    [Column("sisa_cuti_annual")]
    public float? SisaCutiAnnual { get; set; }

    [Column("sisa_cuti_Maternity")]
    public float? SisaCutiMaternity { get; set; }

    [Column("sisa_cuti_long")]
    public float? SisaCutiLong { get; set; }

    [Column("sisa_cuti_replacement")]
    public float? SisaCutiReplacement { get; set; }

    [Column("dimunculin")]
    public int? DiMunculin { get; set; }

    [Column("link_leave_del")]
    public string? LinkLeaveDel { get; set; }

    [Column("link_leave_app")]
    public string? LinkLeaveApp { get; set; }

    public List<LeaveDetailQuery> Detail { get; set; }

}

[Keyless]
public class LeaveDetailQuery
{
    /*
    [Column("cuti_id")]
    public string CutiId { get; set; }
    */

    [Column("cuti_date")]
    public DateTime CutiDate { get; set; }
}

public class CreateLeaveModel
{
    //public DateTime RequestDate { get; set; }
    public string EmployeeId { get; set; }
    public string Keperluan { get; set; }
    public string TypeCutiId { get; set; }
    public string TypeDay { get; set; }
    public string DateString { get; set; }

}


public class ApprovalLeaveModel
{
    //public DateTime RequestDate { get; set; }
    public string ApproveBy { get; set; }
    public string Noted { get; set; }
    public string CutiId { get; set; }
    public int Status { get; set; }


}

public class DeleteLeaveModel
{

    public string DeletedBy { get; set; }
    public string CutiId { get; set; }



}


[Keyless]
public class TotalLeave
{
    [Column("employee_first_name")]
    public string employeeFirstName { get; set; }

    [Column("employee_last_name")]
    public string employeeLastName { get; set; }

    [Column("employee_id")]
    public string employeeId { get; set; }

    [Column("sisa_cuti_annual")]
    public double sisaCutiAnnual { get; set; }

    [Column("sisa_cuti_long")]
    public double sisaCutiLong { get; set; }
}


