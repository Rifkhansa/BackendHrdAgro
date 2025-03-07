using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using BackendHrdAgro.Models.Database.MySql;
using NPOI.HSSF.Record.Chart;

namespace BackendHrdAgro.Models.Layout
{
    public class IncomingLetterModel
    {
        public List<Letter> letters()
        {
            var sql = "select a.*, b.destination_name,CONCAT(c.employee_first_name,' ',c.employee_last_name) " +
                "as receipt_by, DATE_FORMAT(a.letter_date, '%d-%m-%Y') as letter_date_view, case when is_accepted = 0 then 'Open' else 'Closed' end status_accepted " +
                "from tp_incoming_letter a\r\n\t\tinner join\r\n\t\t(\r\n\t\tselect a.employee_id as destination_id, CONCAT(a.employee_first_name,' ',a.employee_last_name) " +
                "as destination_name    from `tm_employee_affair` a\r\n        where a.status=1       union all\r\n        " +
                "select a.`department_id` as destination_id, a.`description` as destination_name\r\n        from `tm_department` a\r\n        " +
                "where a.status=1 and a.`department_id` in ('DP001','DP002','DP003','DP004','DP005','DP006','DP007','DP008','DP010','DP011')\r\n\t\t) " +
                "b on a.destination_id=b.destination_id\r\n\t\tinner join tm_employee_affair c on a.receipt_id=c.employee_id\r\n   \t\twhere a.status in (1)  order by letter_date DESC";
            var letter = new DatabaseContext().Letters.FromSqlRaw(sql).ToList();
            return letter;
        }

        public List<destination> destinations()
        {
            var sql = "select * from (\r\n\t\tselect 1 as my_order, a.employee_id as destination_id, CONCAT(a.employee_first_name,' ',a.employee_last_name) " +
                "as destination_name      from `tm_employee_affair` a\r\n        where a.status=1      union all\r\n        " +
                "select 0 as my_order, a.`department_id` as destination_id, a.`description` as destination_name\r\n        " +
                "from `tm_department` a\r\n        where a.status=1 and a.`department_id` " +
                "in ('DP001','DP002','DP003','DP004','DP005','DP006','DP007','DP008','DP010','DP011')) a\r\n\t\torder by my_order asc, destination_id";
            var destination = new DatabaseContext().Destinations.FromSqlRaw(sql).ToList();
            return destination;
        }

        public List<receiptBy> receipts()
        {
            var sql = "select a.employee_id as receipt_id, CONCAT(a.employee_first_name,' ',a.employee_last_name) as receipt_by\r\n        " +
                "from `tm_employee_affair` a\r\n        where a.status=1";
            var receipt = new DatabaseContext().Receipts.FromSqlRaw(sql).ToList();
            return receipt;
        }

        public List<Dictionary<string, dynamic>> CreateIncomingLetter(TpIncomingLetter value)
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
                        context.TpIncomingLetters.Add(value);
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

    }

    public class createIncoming
    {
        public DateTime LetterDate { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string DestinationId { get; set; }
        public string ReceiptId { get; set; }
        public string Description { get; set; }
    }

    public class updateIncoming
    {
        public string LetterId { get; set; }
        public DateTime LetterDate { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string DestinationId { get; set; }
        public string ReceiptId { get; set; }
        public string Description { get; set; }
    }

    public class doDeleteIn
    {
        public string letterId { get; set; }
        public string remark { get; set; }
    }

    public class closedIn
    {
        public string LetterId { get; set; }
    }

    [Keyless]
    public class Letter
    {
        [Column("letter_id")]
        public string LetterId { get; set; }

        [Column("letter_date")]
        public DateTime LetterDate { get; set; }

        [Column("sender")]
        public string Sender { get; set; }

        [Column("destination_id")]
        public string DestinationId { get; set; }

        [Column("remark")]
        public string Remark { get; set; }

        [Column("subject")]
        public string Subject { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("receipt_id")]
        public string ReceiptId { get; set; }

        [Column("receipt_dt")]
        public DateTime? ReceiptDt { get; set; }

        [Column("is_accepted")]
        public int IsAccepted { get; set; }

        [Column("accepted_dt")]
        public DateTime? AcceptedDt { get; set; }

        [Column("dt_etr")]
        public DateTime DtEtr { get; set; }

        [Column("user_etr")]
        public string UserEtr { get; set; }

        [Column("dt_update")]
        public DateTime DtUpdate { get; set; }

        [Column("user_update")]
        public string UserUpdate { get; set; }

        [Column("destination_name")]
        public string DestinationName { get; set; }

        [Column("receipt_by")]
        public string ReceiptBy { get; set; }

        [Column("letter_date_view")]
        public string LetterDateView { get; set; }

        [Column("status_accepted")]
        public string StatusAccepted { get; set; }
    }

    [Keyless]
    public class destination
    {
        [Column("my_order")]
        public int MyOrder { get; set; }

        [Column("destination_id")]
        public string DestinationId { get; set; }

        [Column("destination_name")]
        public string DestinationName { get; set; }
    }

    [Keyless]
    public class receiptBy
    {
        [Column("receipt_id")]
        public string ReceiptId { get; set; }

        [Column("receipt_by")]
        public string ReceiptBy { get; set; }
    }

}
