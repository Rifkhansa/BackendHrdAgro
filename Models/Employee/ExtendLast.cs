using BackendHrdAgro.Models.Database.MySql;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendHrdAgro.Models.Employee
{
    public class ExtendLastModel
    {
        public List<ExtendLastGroupQuery> extendLastGroupQueries()
        {
            var sql = "select a.*,case when a.status = 1 then 'Active' when a.status=0 then 'Non Active' when a.status=5 " +
                "then 'PHK' when a.status=7 then 'Kontrak Habis / Kontrak Tidak diperpanjang' when a.status=9 then 'Resign' end as my_status,\r\n      " +
                "bank_name,department_name,title_name,religi_name,level_name,employee_status_name,married_name,i.termination_date,\r\n      " +
                "DATE_FORMAT(a.tanggal_lahir, '%d %M %Y') as tanggal_lahir_view,DATE_FORMAT(a.joint_date, '%d %M %Y') as joint_date_view,DATE_FORMAT(a.end_of_contract, '%d %M %Y') " +
                "as end_of_contract_view,DATE_FORMAT(a.permanent_date, '%d %M %Y') as permanent_date_view\r\n    \t\tfrom tm_employee_affair a left outer join tm_department b on a.department_id=b.department_id " +
                "left outer join tm_title c on a.title_id=c.title_id\r\n        left outer join tm_level d on a.level_id=d.level_id\r\n        left outer join tm_employee_status e on a.employee_status_id=e.employee_status_id\r\n        " +
                "left outer join tm_religi f on a.religi_id=f.religi_id\r\n        left outer join tm_married g on a.married_id=g.married_id\r\n        left outer join tm_bank h on a.bank_id=h.bank_id\r\n        " +
                "left outer join (select * from tp_termination where  DATE_FORMAT(now(),'%Y-%m-%d') <= DATE_FORMAT(DATE_ADD(termination_date, INTERVAL 1 MONTH),'%Y-%m-%d') ) as i on a.employee_id=i.employee_id " +
                "where a.status in (1,5,7,9) order by employee_first_name ASC";
            var extendLast = new DatabaseContext().ExtendLastGroupQueries.FromSqlRaw(sql).ToList();
            return extendLast;
        }

        public  List<RemindersExtend> reminders()
        {
            var sql = "select a.*,department_name ,title_name,bank_name,religi_name,\r\n        " +
                "level_name,employee_status_name,married_name,DATE_FORMAT(a.tanggal_lahir, '%d %M %Y') as tanggal_lahir_view,\r\n        " +
                "DATE_FORMAT(a.joint_date, '%d %M %Y') as joint_date_view,DATE_FORMAT(a.end_of_contract, '%d %M %Y') as end_of_contract_view,\r\n        " +
                "DATE_FORMAT(a.permanent_date, '%d %M %Y') as permanent_date_view\r\n        from tm_employee_affair a " +
                "left outer join tm_department b on a.department_id=b.department_id\r\n        left outer join tm_title c on a.title_id=c.title_id\r\n        " +
                "left outer join tm_level d on a.level_id=d.level_id\r\n        left outer join tm_employee_status e on a.employee_status_id=e.employee_status_id\r\n        " +
                "left outer join tm_religi f on a.religi_id=f.religi_id\r\n        left outer join tm_married g on a.married_id=g.married_id\r\n        " +
                "left outer join tm_bank h on a.bank_id=h.bank_id\r\n        where DATE_FORMAT(end_of_contract,'%Y-%m-%d') >= DATE_FORMAT(now(),'%Y-%m-%d')\r\n        " +
                "and  DATE_FORMAT(DATE_ADD(end_of_contract, INTERVAL -2 MONTH),'%Y-%m-%d')<= DATE_FORMAT(now(),'%Y-%m-%d')   order by employee_first_name ASC";
            var reminder = new DatabaseContext().RemindersExtends.FromSqlRaw(sql).ToList();
            return reminder;
        }

        public List<Dictionary<string, dynamic>> CreateExtendLast(TpExtend value)
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
                        context.TpExtends.Add(value);
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
    }

    public class ExtendLastSignRequestBody
    {
        public string EmployeeId { get; set; } = null!;
        public DateTime StartExtendDate { get; set; }
        public DateTime EndExtendDate { get; set; }
        public string Reason { get; set; }
    }

    [Keyless]
    public class ExtendLastGroupQuery
    {
        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("employee_first_name")]
        public string EmployeeFirstName { get; set; }

        [Column("employee_last_name")]
        public string? EmployeeLastName { get; set; }

        [Column("department_id")]
        public string DepartmentId { get; set; }

        [Column("title_id")]
        public string TitleId { get; set; }

        [Column("level_id")]
        public string LevelId { get; set; }

        [Column("employee_status_id")]
        public string EmployeeStatusId { get; set; }

        [Column("joint_date")]
        public DateTime? JointDate { get; set; }

        [Column("end_of_contract")]
        public DateTime? EndOfContract { get; set; }

        [Column("permanent_date")]
        public DateTime? PermanentDate { get; set; }

        [Column("tempat_lahir")]
        public string PlaceOfBirth { get; set; }

        [Column("tanggal_lahir")]
        public DateTime DateOfBirth { get; set; }

        [Column("alamat")]
        public string? Address { get; set; }

        [Column("alamat_tinggal")]
        public string? ResidentialAddress { get; set; }

        [Column("religi_id")]
        public string? ReligionId { get; set; }

        [Column("gender_id")]
        public string? GenderId { get; set; }

        [Column("married_id")]
        public string? MarriedId { get; set; }

        [Column("bank_id")]
        public string? BankId { get; set; }

        [Column("no_rekening")]
        public string? AccountNumber { get; set; }

        [Column("email")]
        public string? Email { get; set; }

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

        [Column("bank_name")]
        public string? BankName { get; set; }

        [Column("department_name")]
        public string? DepartmentName { get; set; }

        [Column("title_name")]
        public string? TitleName { get; set; }

        [Column("religi_name")]
        public string? ReligionName { get; set; }

        [Column("level_name")]
        public string LevelName { get; set; }

        [Column("employee_status_name")]
        public string? EmployeeStatusName { get; set; }

        [Column("married_name")]
        public string? MarriedName { get; set; }

        [Column("termination_date")]
        public DateTime? TerminationDate { get; set; }

        [NotMapped]
        public string DateOfBirthView => DateOfBirth.ToString("dd MMMM yyyy");

        [NotMapped]
        public string? JointDateView => JointDate?.ToString("dd MMMM yyyy");

        [NotMapped]
        public string? EndOfContractView => EndOfContract?.ToString("dd MMMM yyyy");

        [NotMapped]
        public string? PermanentDateView => PermanentDate?.ToString("dd MMMM yyyy");
    }

    [Keyless]
    public class RemindersExtend
    {
        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("employee_first_name")]
        public string? EmployeeFirstName { get; set; }

        [Column("employee_last_name")]
        public string? EmployeeLastName { get; set; }

        [Column("department_id")]
        public string? DepartmentId { get; set; }

        [Column("department_name")]
        public string? DepartmentName { get; set; }

        [Column("title_id")]
        public string? TitleId { get; set; }

        [Column("title_name")]
        public string? TitleName { get; set; }

        [Column("level_id")]
        public string? LevelId { get; set; }

        [Column("level_name")]
        public string? LevelName { get; set; }

        [Column("employee_status_id")]
        public string? EmployeeStatusId { get; set; }

        [Column("employee_status_name")]
        public string? EmployeeStatusName { get; set; }

        [Column("joint_date")]
        public string? JointDate { get; set; }

        [Column("end_of_contract")]
        public string? EndOfContract { get; set; }

        [Column("permanent_date")]
        public DateTime PermanentDate { get; set; }

        [Column("tempat_lahir")]
        public string? TempatLahir { get; set; }

        [Column("tanggal_lahir")]
        public DateTime TanggalLahir { get; set; }

        [Column("alamat")]
        public string? Alamat { get; set; }

        [Column("alamat_tinggal")]
        public string? AlamatTinggal { get; set; }

        [Column("religi_id")]
        public string? ReligiId { get; set; }

        [Column("religi_name")]
        public string? ReligiName { get; set; }

        [Column("married_id")]
        public string? MarriedId { get; set; }

        [Column("married_name")]
        public string? MarriedName { get; set; }

        [Column("bank_id")]
        public string? BankId { get; set; }

        [Column("bank_name")]
        public string? BankName { get; set; }

        [Column("no_rekening")]
        public string? NoRekening { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("dt_etr")]
        public string? DtEtr { get; set; }

        [Column("user_etr")]
        public string? UserEtr { get; set; }

        [Column("dt_update")]
        public string? DtUpdate { get; set; }

        [Column("user_update")]
        public string? UserUpdate { get; set; }

        [Column("tanggal_lahir_view")]
        public DateTime TanggalLahirView { get; set; }

        [Column("joint_date_view")]
        public DateTime JointDateView { get; set; }

        [Column("end_of_contract_view")]
        public DateTime EndOfContractView { get; set; }

        [Column("permanent_date_view")]
        public DateTime PermanentDateView { get; set; }
    }


}
