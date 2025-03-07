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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BackendHrdAgro.Models.Permission
{
    public class PermissionDB
    {
        public List<TmPermissionType> PermissionTypeFind() => new DatabaseContext().TmPermissionTypes.Where(x => x.Status.Equals(1)).ToList();
        public List<TmPermissionType> PermissionTypeFind(string id) => new DatabaseContext().TmPermissionTypes.Where(x => x.Status.Equals(1) && x.PermissionTypeId.Equals(id)).ToList();
        public static string? GetEmployee(string permissionId) => new DatabaseContext().TpPermissions.Where(x => x.PermissionId.Equals(permissionId)).Select(x => x.EmployeeId).FirstOrDefault();
        public static string? GetPermissionType(string id) => new DatabaseContext().TmPermissionTypes.Where(x => x.PermissionTypeId.Equals(id)).Select(x => x.PermissionType).FirstOrDefault();

        public static string? GetPermissionTypeId(string permissionId) => new DatabaseContext().TpPermissions.Where(x => x.PermissionId.Equals(permissionId)).Select(x => x.PermissionTypeId).FirstOrDefault();

        public List<TpPermissionDetail> ListPermissionDetail(string permissionId) => new DatabaseContext().TpPermissionDetails.Where(x => x.Status.Equals(1) && x.PermissionId.Equals(permissionId)).ToList();

        public List<TpPermissionDetail> ListPermissionDetailINNERJoin____NOTUSED(string permissionId)
        {
            using (var context = new DatabaseContext())
            {
                var query = (from detail in context.TpPermissionDetails
                             join permission in context.TpPermissions
                             on detail.PermissionId equals permission.PermissionId
                             where detail.Status == 1 && detail.PermissionId == permissionId
                             select new TpPermissionDetail
                             {
                                 PermissionDetailId = detail.PermissionDetailId,
                                 PermissionId = detail.PermissionId,
                                 Status = detail.Status,

                             }).ToList();

                return query;
            }
        }

        public List<TpPermissionDetail> ListPermissionDetailEFTJoin___NOTUSED(string permissionId)
        {
            using (var context = new DatabaseContext())
            {
                var query = (from detail in context.TpPermissionDetails
                             where detail.Status == 1 && detail.PermissionId == permissionId
                             join permission in context.TpPermissions
                             on detail.PermissionId equals permission.PermissionId into permissionGroup
                             from permission in permissionGroup.DefaultIfEmpty() // This ensures a LEFT JOIN
                             select new TpPermissionDetail
                             {
                                 // Map properties from detail and permission to TpPermissionDetail
                                 PermissionDetailId = detail.PermissionDetailId,
                                 PermissionId = detail.PermissionId,
                                 Status = detail.Status,
                                 // Map additional properties from TpPermission if needed, handling nulls
                                 // Example: PermissionName = permission != null ? permission.PermissionName : null
                             }).ToList();

                return query;
            }
        }

        public int GetPermissionWithoutLetter(string employeeId)
        {
            using (var context = new DatabaseContext())
            {
                var count = (from detail in context.TpPermissionDetails
                             join permission in context.TpPermissions
                             on detail.PermissionId equals permission.PermissionId
                             where detail.Status == 1 && permission.EmployeeId == employeeId
                             && permission.PermissionTypeId == "PER002" && permission.DeletedAt == null
                             && detail.PermissionDate.Year == DateTime.Now.Year
                             select detail).Count();

                return count;
            }
        }

        public int CheckPermissionComeLate(string employeeId)
        {
            using (var context = new DatabaseContext())
            {

                var count = (from detail in context.TpPermissionDetails
                             join permission in context.TpPermissions
                             on detail.PermissionId equals permission.PermissionId
                             where (permission.PermissionTypeId == "PER004" || permission.PermissionTypeId == "PER005")
                             && permission.EmployeeId == employeeId
                             && permission.DeletedBy == null
                             && detail.PermissionDate.Month == DateTime.Now.Month
                             && detail.PermissionDate.Year == DateTime.Now.Year
                             select detail.PermissionDetailId).Count();

                return count;
            }
        }

        public List<PermissionQuery> ListPermission(string filter)
        {

            string sql = "";


            sql = $"SELECT a.permission_id,a.employee_id, CONCAT(b.employee_first_name,' ',b.employee_last_name) as employee_name, " +
                $"DATE_FORMAT(a.request_date, '%d %M %Y') as request_view,a.status, " +
                $"c.permission_type,a.reason,a.file_name, " +
                $"case when a.status = 1 then 'Accepted' when a.status=0 then 'waiting' when a.status=5 then 'Rejected' end as my_status, " +
                $"a.noted,concat(d.employee_first_name,' ',d.employee_last_name) as approve_name,b.level_id, " +
                $"'' as link_permission_del,'' as link_permission_app " +
                $"from tp_permission a " +
                $"left outer join tm_employee_affair b on a.employee_id=b.employee_id " +
                $"left outer join tm_permission_type c on a.permission_type_id=c.permission_type_id " +
                $"left outer join tm_employee_affair d on a.approve_by=d.employee_id " +
                $"left outer join tm_department h on b.department_id=h.department_id " +
                $"where a.status in (1,0,5) {filter} and a.deleted_at is null  " +
                $" order by a.status asc, a.request_date DESC ";

            var list = new DatabaseContext().PermissionQueries.FromSqlRaw(sql).ToList();

            if (list.Count > 0)
            {
                foreach (var l in list)
                {

                    if (l.Detail == null)
                    {
                        l.Detail = new List<PermissionDetailQuery>();
                    }
                    sql = $"SELECT " +
                          $"a.permission_date " +
                          $"from tp_permission_detail a  " +
                          $"where permission_id='{l.PermissionId}' order by a.permission_date DESC";
                    var list_detail = new DatabaseContext().PermissionDetailQueries.FromSqlRaw(sql).ToList();

                    if (list_detail.Count > 0)
                    {
                        l.Detail.AddRange(list_detail);

                    }
                    else
                    {
                        l.Detail.AddRange(null);
                    }

                }


            }
            return list;
        }


        public List<PermissionQuery> ListPermissionWithoutLetterThisYear(string employeeId)
        {

            string sql = "";


            DateTime dateNow = DateTime.Now;
            string myYear = "";
            myYear = dateNow.ToString("yyyy");

            sql = $"SELECT a.permission_id,a.employee_id, CONCAT(b.employee_first_name,' ',b.employee_last_name) as employee_name, " +
                $"DATE_FORMAT(a.request_date, '%d %M %Y') as request_view,a.status, " +
                $"c.permission_type,a.reason,a.file_name, " +
                $"case when a.status = 1 then 'Accepted' when a.status=0 then 'waiting' when a.status=5 then 'Rejected' end as my_status, " +
                $"a.noted,concat(d.employee_first_name,' ',d.employee_last_name) as approve_name,b.level_id, " +
                $"'' as link_permission_del,'' as link_permission_app " +
                $"from tp_permission a " +
                $"left outer join tm_employee_affair b on a.employee_id=b.employee_id " +
                $"left outer join tm_permission_type c on a.permission_type_id=c.permission_type_id " +
                $"left outer join tm_employee_affair d on a.approve_by=d.employee_id " +
                $"left outer join tm_department h on b.department_id=h.department_id " +
                $"where a.status in (1) and a.deleted_at is null AND a.permission_type_id='PER002' and a.employee_id='{employeeId}' " +
                $" order by a.status asc, a.request_date DESC ";

            var list = new DatabaseContext().PermissionQueries.FromSqlRaw(sql).ToList();
            Console.WriteLine("list = " + list);
            if (list.Count > 0)
            {
                foreach (var l in list)
                {

                    if (l.Detail == null)
                    {
                        l.Detail = new List<PermissionDetailQuery>();
                    }
                    sql = $"SELECT " +
                          $"a.permission_date " +
                          $"from tp_permission_detail a  " +
                          $"where permission_id='{l.PermissionId}' AND year(a.permission_date)='{myYear}' order by a.permission_date DESC";
                    Console.WriteLine(sql);
                    var list_detail = new DatabaseContext().PermissionDetailQueries.FromSqlRaw(sql).ToList();

                    if (list_detail.Count > 0)
                    {
                        l.Detail.AddRange(list_detail);
                        Console.WriteLine("masuk true");

                    }
                    else
                    {
                        Console.WriteLine("masuk else");
                        // l.Detail.AddRange(null); //dikomen setelah maseko ada error
                    }

                }


            }
            return list;
        }

        public List<Dictionary<string, dynamic>> CreatePermission(TpPermission value, String permissionId, List<string> insertDates)
        {

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            string[] myArrDate;
            string myDate = "";
            int n = 0;
            string permissionDetailId = "";

            List<TpPermissionDetail> insertDetailPermission = new List<TpPermissionDetail>();

            using DatabaseContext context = new DatabaseContext();


            var executionStrategy = context.Database.CreateExecutionStrategy();
            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {

                    try
                    {
                        context.TpPermissions.Add(value);
                        context.SaveChanges();


                        foreach (var i in insertDates)
                        {

                            myArrDate = i.Split('-');
                            myDate = myArrDate[2] + '-' + myArrDate[1] + '-' + myArrDate[0];

                            if (n == 0)
                            {
                                permissionDetailId = BaseModel.GenerateId(tableName: "tp_permission_detail", str: "RPD", primaryKey: "permission_detail_id", trailing: 4, lastKey: "NONE", date: DateTime.Now.ToString("yyMM"));
                            }
                            else
                            {
                                permissionDetailId = BaseModel.GenerateId(tableName: "tp_permission_detail", str: "RPD", primaryKey: "permission_detail_id", trailing: 4, lastKey: permissionDetailId, date: DateTime.Now.ToString("yyMM"));
                            }

                            TpPermissionDetail detailDate = new TpPermissionDetail()
                            {

                                PermissionDetailId = permissionDetailId,
                                PermissionId = permissionId,
                                PermissionDate = DateTime.Parse(myDate),
                                Status = 1
                            };

                            insertDetailPermission.Add(detailDate);
                            n = n + 1;
                        }
                        context.TpPermissionDetails.AddRangeAsync(insertDetailPermission);
                        context.SaveChanges();


                        transaction.Commit();

                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Berhasil Membuat pengajuan izin");

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


        public List<Dictionary<string, dynamic>> ApprovePermission(TpPermission value, string id)
        {

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();


            var current = context.TpPermissions.Where(x => x.PermissionId == value.PermissionId).FirstOrDefault() ?? throw new Exception(value.Noted + " not found");

            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        TpPermission approvePermission = new TpPermission();
                        approvePermission = current;
                        approvePermission.ApproveBy = value.ApproveBy;
                        approvePermission.ApproveDate = value.ApproveDate;
                        approvePermission.Noted = value.Noted;
                        approvePermission.Status = value.Status;
                        approvePermission.IsNotify = value.IsNotify;
                        approvePermission.UpdatedAt = value.UpdatedAt;
                        approvePermission.UpdatedBy = value.ApproveBy;

                        context.TpPermissions.Entry(current).CurrentValues.SetValues(approvePermission);
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


        public List<Dictionary<string, dynamic>> DeletePermission(TpPermission value, string id)
        {

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            using DatabaseContext context = new DatabaseContext();


            var current = context.TpPermissions.Where(x => x.PermissionId == value.PermissionId).FirstOrDefault() ?? throw new Exception(value.PermissionId + " not found");



            var executionStrategy = context.Database.CreateExecutionStrategy();

            executionStrategy.Execute(() =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        TpPermission deletePermission = new TpPermission();
                        deletePermission = current;
                        deletePermission.DeletedBy = value.DeletedBy;
                        deletePermission.DeletedAt = value.DeletedAt;

                        context.TpPermissions.Entry(current).CurrentValues.SetValues(deletePermission);
                        context.SaveChanges();
                        transaction.Commit();


                        data.Clear();
                        data.Add("result", true);
                        data.Add("message", "Permission berhasil dihapus");

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


        //samsul
        public int CountPermissionData(string PermissionId) => new DatabaseContext().TpPermissions.Where(x => x.PermissionId.Equals(PermissionId)).Count();

        public bool CheckIsApprove(string PermissionId)
        {
            using (var context = new DatabaseContext())
            {
                // Cek apakah ada record dengan leaveId dan status 0
                var isStatusZero = context.TpPermissions
                    .Any(x => x.PermissionId.Equals(PermissionId) && x.Status.Equals(0));

                // Jika ada status 0, kembalikan false, jika tidak, kembalikan true
                return isStatusZero;
            }
        }

        public List<PermissionQuery> CheckApprovalStatus()
        {
            var sql = $"SELECT a.permission_id,a.employee_id, CONCAT(b.employee_first_name,' ',b.employee_last_name) as employee_name, " +
                $"DATE_FORMAT(a.request_date, '%d %M %Y') as request_view,a.status, " +
                $"c.permission_type,a.reason,a.file_name, " +
                $"case when a.status = 1 then 'Accepted' when a.status=0 then 'waiting' when a.status=5 then 'Rejected' end as my_status, " +
                $"a.noted,concat(d.employee_first_name,' ',d.employee_last_name) as approve_name,b.level_id, " +
                $"'' as link_permission_del,'' as link_permission_app " +
                $"from tp_permission a " +
                $"left outer join tm_employee_affair b on a.employee_id=b.employee_id " +
                $"left outer join tm_permission_type c on a.permission_type_id=c.permission_type_id " +
                $"left outer join tm_employee_affair d on a.approve_by=d.employee_id " +
                $"left outer join tm_department h on b.department_id=h.department_id " +
                $"where a.status = 0 and a.deleted_at is null  " +
                $"and a.request_date >= DATE_FORMAT(CURDATE(), '%Y-%m-01') " +  // awal bulan
                $"and a.request_date <= LAST_DAY(CURDATE()) " +  // akhir bulan
                $" order by a.status asc, a.request_date DESC ";
            var list = new DatabaseContext().PermissionQueries.FromSqlRaw(sql).ToList();
            return list;
        }

        public DateTime FindPermissionDate(string id) => new DatabaseContext().TpPermissions.Where(x => x.PermissionId.Equals(id)).Select(x => x.RequestDate).FirstOrDefault();

        //

    }


    [Keyless]
    public class PermissionQuery
    {


        [Column("permission_id")]
        public string PermissionId { get; set; }

        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("employee_name")]
        public string? EmployeeName { get; set; }

        [Column("request_view")]
        public string RequestView { get; set; }

        [Column("permission_type")]
        public string PermissionType { get; set; }

        [Column("level_id")]
        public string? LevelId { get; set; }

        [Column("reason")]
        public string Reason { get; set; }

        [Column("file_name")]
        public string? FileName { get; set; }


        /*
        [Column("permission_type_id")]
        public string PermissionTypeId { get; set; }
        
        [Column("request_date")]
        public DateOnly RequestDate { get; set; }
        
        [Column("is_read")]
        public int IsRead { get; set; }
        */
        [Column("status")]
        public int Status { get; set; }

        [Column("my_status")]
        public string MyStatus { get; set; }

        [Column("approve_name")]
        public string? ApproveName { get; set; }


        [Column("noted")]
        public string? Noted { get; set; }

        /*

        [Column("approve_date")]
        public DateOnly ApproveDate { get; set; }

        [Column("is_notify")]
        public int IsNotify { get; set; }

        
        [Column("is_cut_absent")]
        public int IsCutAbsent { get; set; }

        [Column("cut_absent")]
        
        public float CutAbsent { get; set; }

        [Column("created_at")]
        

        public DateTime CreatedAt { get; set; }

        [Column("created_by")]
        public string CreatedBy { get; set; }
        
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        
        [Column("updated_by")]
        public string UpdatedBy { get; set; }
        
        [Column("deleted_at")]
        public DateTime DeletedAt { get; set; }

        [Column("deleted_by")]
        public string DeletedBy { get; set; }
        */

        [Column("link_permission_del")]
        public string? LinkPermissionDel { get; set; }

        [Column("link_permission_app")]
        public string? LinkPermissionApp { get; set; }

        public List<PermissionDetailQuery> Detail { get; set; }

    }

    [Keyless]
    public class PermissionDetailQuery
    {

        [Column("permission_date")]
        public DateTime PermissionDate { get; set; }
    }


    public class CreatePermissionModel
    {
        //public DateTime RequestDate { get; set; }
        public string EmployeeId { get; set; }
        public string Reason { get; set; }
        public string PermissionTypeId { get; set; }
        public string DateString { get; set; }

        public IFormFile? FileName { get; set; }

    }




    public class ApprovalPermissionModel
    {
        //public DateTime RequestDate { get; set; }
        public string ApproveBy { get; set; }
        public string Noted { get; set; }
        public string PermissionId { get; set; }
        public int Status { get; set; }


    }

    public class DeletePermissionModel
    {

        public string DeletedBy { get; set; }
        public string PermissionId { get; set; }



    }
}
