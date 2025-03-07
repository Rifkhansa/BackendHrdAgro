using BackendHrdAgro.Models.Database.MySql;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendHrdAgro.Models.Layout
{
    public class OutgoingLetter
    {
        public List<OutGoingLetterIndex> group()
        {
            var sql = "select a.*, b.letter_form_name,c.letter_type_name, d.department_name,f.cob_name,g.security_name,\r\n  " +
                "DATE_FORMAT(a.letter_date, '%d-%m-%Y') as letter_date_view,DATE_FORMAT(a.receipt_dt, '%d-%m-%Y') " +
                "as receipt_dt_view ,\r\n  DATE_FORMAT(a.letter_date, '%m/%d/%Y') as letter_date_box,DATE_FORMAT(a.receipt_dt, '%m/%d/%Y') " +
                "as receipt_dt_box,\r\n  case when a.status = 1 then 'On Process' when a.status=0 then 'Non Active' " +
                "when a.status=5 then 'Terkirim' end as my_status,\r\n  concat(h.employee_first_name ,' ',h.employee_last_name ) " +
                "as sender,\r\n  concat(x.employee_first_name ,' ',x.employee_last_name ) as created\r\n\t\t" +
                "from tp_outgoing_letter a\r\n\t\tinner join tm_letter_form b on a.letter_form_id=b.letter_form_id\r\n\t\t" +
                "inner join tm_letter_type c on a.letter_type_id=c.letter_type_id\r\n\t\tinner join tm_department d on a.department_id=d.department_id\r\n\t\t" +
                "inner join (\r\n\t  \t\t  select 1 as my_order, title_id,title_name from tm_title a  where a.status in (1) and title_id in ('DS001','DS002')  " +
                "union all select 2 as my_order, 'NONE' as title_id, 'Appointed person' as title_name) e on a.signature_by=e.title_id\r\n\t\t" +
                "left outer join tm_cob f on a.cob_id=f.cob_id\r\n\t\tleft outer join tm_security g on a.security_id=g.security_id\r\n\t\t" +
                "left outer join tm_employee_affair h on a.user_sender=h.employee_id\r\n\t\tleft outer join tm_employee_affair x on a.user_etr=x.employee_id\r\n  \t\t" +
                $"where a.status in (1,5) order by a.letter_id DESC";
            var groups = new DatabaseContext().OutGoingLetters.FromSqlRaw(sql).ToList();
            return groups;
        }

        public List<LetterForm> letterForms(string departmentId)
        {
            var sql = $"select * from tm_letter_form where status=1 and department_id = '{departmentId}' order by letter_form_id desc";
            var letterForms = new DatabaseContext().LetterForms.FromSqlRaw(sql).ToList();
            return letterForms;
        }

        public List<LetterType> letterTypes()
        {
            var sql = "select * from tm_letter_type where status=1 order by letter_type_id desc";
            var letterType = new DatabaseContext().LetterTypes.FromSqlRaw(sql).ToList();
            return letterType;
        }

        public List<TitleOutgoing> titleOutgoings()
        {
            var sql = "select * from (select 1 as my_order, title_id,title_name from tm_title a  " +
                "where a.status in (1) and title_id in ('DS001','DS002')  union all select 2 as my_order, 'NONE' " +
                "as title_id, 'Appointed person' as title_name) a\r\n\t\t\t  order by my_order asc, title_name desc";
            var title = new DatabaseContext().TitleOutgoings.FromSqlRaw(sql).ToList();
            return title;
        }

        public List<Cob> cobs()
        {
            var sql = "select * from tm_cob where status=1 order by cob_name asc";
            var cob = new DatabaseContext().Cobs.FromSqlRaw(sql).ToList();
            return cob;
        }

        public List<Security> security()
        {
            var sql = "select * from tm_security where status=1 order by security_name asc";
            var securities = new DatabaseContext().Securities.FromSqlRaw(sql).ToList();
            return securities;
        }

        public List<Sender> senders()
        {
            var sql = "SELECT * FROM tm_employee_affair where department_id='DP011'";
            var senders = new DatabaseContext().Senders.FromSqlRaw(sql).ToList();
            return senders;
        }

    }

    public class CreateOutgoing
    {
        public string? DepartmentId { get; set; }
        public string? EmployeeId { get; set; }
        public DateTime LetterDate { get; set; }
        public string LetterFormId { get; set; }
        public string LetterTypeId { get; set; }
        public string TitleId { get; set; }
        public string Destination { get; set; }
        public string? Remark { get; set; }
        public string? CobId { get; set; }
        public string? SecurityId { get; set; }
        public string? LetterFormatNumber { get; set; }
        public string? SenderId { get; set; }
        public DateTime? ReceiptDt { get; set; }
        public string? ReceiptBy { get; set; }
    }

    public class UpdateOutgoing
    {
        public string DepartmentId { get; set; }
        public string LetterId { get; set; }
        public DateTime LetterDate { get; set; }
        public string LetterFormId { get; set; }
        public string LetterTypeId { get; set; }
        public string TitleId { get; set; }
        public string Destination { get; set; }
        public string? Remark { get; set; }
        public string? CobId { get; set; }
        public string? SecurityId { get; set; }
        public string LetterFormatNummber { get; set; }
        public string? SenderId { get; set; }
        public DateTime ReceiptDt { get; set; }
        public string? ReceiptBy { get; set; } = null!;
        public string? letterNumber { get; set; }
        public IFormFile File { get; set; }

    }

    public class doSent
    {
        public string LetterId { get; set; }
        public string? SenderId { get; set; }
        public DateTime ReceiptDt { get; set; }
        public string? ReceiptBy { get; set; }
        public IFormFile File { get; set; }
    }

    public class doDelete
    {
        public string letterId { get; set; }
        public string letterNumber { get; set; }
        public string destination { get; set; }
    }

    public class deleteDocumentOut
    {
        public string Id { get; set; }
        public string FileName { get; set; }
    }

    [Keyless]
    public class MappingId
    {
        [Column("mapping_id")]
        public string LetterMappingId { get; set; }
    }

    [Keyless]
    public class MaxIdResponse
    {
        [Column("max_id")]
        public string? MaxId { get; set; }
    }

    [Keyless]
    public class LetterNumberResponse
    {
        [Column("letter_number")]
        public string? LetterNumber { get; set; }
    }

    [Keyless]
    public class DepartmentOutgoing
    {
        [Column("dept_code")]
        public string DeptCode { get; set; }
    }

    [Keyless]
    public class OutGoingLetterIndex
    {
        [Column("letter_id")]
        public string LetterId { get; set; }

        [Column("letter_date")]
        public DateTime LetterDate { get; set; }

        [Column("letter_form_id")]
        public string LetterFormId { get; set; }

        [Column("mapping_id")]
        public string? MappingId { get; set; }

        [Column("checking")]
        public int? Checking { get; set; }

        [Column("letter_type_id")]
        public string? LetterTypeId { get; set; }

        [Column("letter_number")]
        public string? LetterNumber { get; set; }

        [Column("department_id")]
        public string DepartmentId { get; set; }

        [Column("signature_by")]
        public string? SignatureBy { get; set; }

        [Column("letter_format_number")]
        public string? LetterFormatNumber { get; set; }

        [Column("destination")]
        public string? Destination { get; set; }

        [Column("remark")]
        public string? Remark { get; set; }

        [Column("tender")]
        public string? Tender { get; set; }

        [Column("cob_id")]
        public string? CobId { get; set; }

        [Column("security_id")]
        public string? SecurityId { get; set; }

        [Column("document_status")]
        public int? DocumentStatus { get; set; }

        [Column("file_name")]
        public string? FileName { get; set; }

        [Column("size")]
        public int? Size { get; set; }

        [Column("type")]
        public string? Type { get; set; }

        [Column("status")]
        public int? Status { get; set; }

        [Column("user_sender")]
        public string? UserSender { get; set; }

        [Column("receipt_by")]
        public string? ReceiptBy { get; set; }

        [Column("receipt_dt")]
        public DateTime? ReceiptDt { get; set; }

        [Column("is_accepted")]
        public int? IsAccepted { get; set; }

        [Column("dt_etr")]
        public DateTime DtEtr { get; set; }

        [Column("user_etr")]
        public string UserEtr { get; set; }

        [Column("dt_update")]
        public DateTime DtUpdate { get; set; }

        [Column("user_update")]
        public string? UserUpdate { get; set; }

        [Column("letter_form_name")]
        public string? LetterFormName { get; set; }

        [Column("letter_type_name")]
        public string? LetterTypeName { get; set; }

        [Column("department_name")]
        public string? DepartmentName { get; set; }

        [Column("cob_name")]
        public string? CobName { get; set; }

        [Column("security_name")]
        public string? SecurityName { get; set; }

        [Column("letter_date_view")]
        public string? LetterDateView { get; set; }

        [Column("receipt_dt_view")]
        public string? ReceiptDtView { get; set; }

        [Column("letter_date_box")]
        public string? LetterDateBox { get; set; }

        [Column("receipt_dt_box")]
        public string? ReceiptDtBox { get; set; }

        [Column("my_status")]
        public string? MyStatus { get; set; }

        [Column("sender")]
        public string? Sender { get; set; }

        [Column("created")]
        public string? Created { get; set; }
    }

    [Keyless]
    public class LetterForm
    {
        [Column("letter_form_id")]
        public string LetterFormId { get; set; }

        [Column("letter_form_name")]
        public string LetterFormName { get; set; }
    }

    [Keyless]
    public class LetterType
    {
        [Column("letter_type_id")]
        public string LetterTypeId { get; set; }

        [Column("letter_type_name")]
        public string LetterTypeName { get; set; }

        [Column("mapping_id")]
        public string MappingId { get; set; }

        [Column("status")]
        public int Status { get; set; }
    }

    [Keyless]
    public class TitleOutgoing
    {
        [Column("my_order")]
        public int MyOrder { get; set; }

        [Key]
        [Column("title_id")]
        public string TitleId { get; set; }

        [Column("title_name")]
        public string TitleName { get; set; }
    }

    [Keyless]
    public class Cob
    {
        [Column("cob_id")]
        public string CobId { get; set; }

        [Column("cob_name")]
        public string CobName { get; set; }

        [Column("status")]
        public int Status { get; set; }
    }

    [Keyless]
    public class Security
    {
        [Column("security_id")]
        public string SecurityId { get; set; }

        [Column("security_name")]
        public string SecurityName { get; set; }

        [Column("status")]
        public int Status { get; set; }
    }

    [Keyless]
    public class Sender
    {
        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [Column("employee_first_name")]
        public string EmployeeFirstName { get; set; }

        [Column("employee_last_name")]
        public string EmployeeLastName { get; set; }
    }

}
