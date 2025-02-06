using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BackendHrdAgro.Models.Database.MySql.Support
{
    [Keyless]
    public partial class Dashboardpresence
    {
        [Column("employee_first_name")]
        [StringLength(100)]
        public string? EmployeeFirstName { get; set; }
        [Column("employee_last_name")]
        [StringLength(100)]
        public string? EmployeeLastName { get; set; }
        [Column("time_in")]
        [StringLength(8)]
        public string? TimeIn { get; set; }
        [Column("time_out")]
        [StringLength(8)]
        public string? TimeOut { get; set; }
        [Column("absent_date")]
        public DateOnly? AbsentDate { get; set; }
    }
}
