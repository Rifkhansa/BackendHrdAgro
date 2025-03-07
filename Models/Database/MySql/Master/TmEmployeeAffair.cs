﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackendHrdAgro.Models.Database.MySql
{
    public class TmEmployeeAffairReturn
    {
        public string EmployeeId { get; set; } = null!;
        public string? EmployeeFirstName { get; set; }
        public string? EmployeeLastName { get; set; }
    }

    [Table("tm_employee_affair")]
    [MySqlCharSet("latin1")]
    [MySqlCollation("latin1_swedish_ci")]
    public partial class TmEmployeeAffair
    {
        [Key]
        [Column("employee_id")]
        [StringLength(21)]
        public string EmployeeId { get; set; }

        [Column("employee_first_name")]
        [StringLength(100)]
        public string EmployeeFirstName { get; set; }

        [Column("employee_last_name")]
        [StringLength(100)]
        public string EmployeeLastName { get; set; }

        [Required]
        [Column("department_id")]
        [StringLength(21)]
        public string DepartmentId { get; set; }

        [Required]
        [Column("title_id")]
        [StringLength(21)]
        public string TitleId { get; set; }

        [Required]
        [Column("level_id")]
        [StringLength(10)]
        public string LevelId { get; set; }

        [Required]
        [Column("employee_status_id")]
        [StringLength(10)]
        public string EmployeeStatusId { get; set; }

        [Column("joint_date")]
        public DateTime JointDate { get; set; }

        [Column("end_of_contract")]
        public DateTime EndOfContract { get; set; }

        [Column("permanent_date")]
        public DateTime? PermanentDate { get; set; }

        [Required]
        [Column("tempat_lahir")]
        [StringLength(25)]
        public string TempatLahir { get; set; }

        [Column("tanggal_lahir")]
        public DateTime TanggalLahir { get; set; }

        [Required]
        [Column("alamat")]
        [StringLength(150)]
        public string Alamat { get; set; }

        [Required]
        [Column("alamat_tinggal")]
        [StringLength(150)]
        public string AlamatTinggal { get; set; }

        [Required]
        [Column("religi_id")]
        [StringLength(5)]
        public string ReligiId { get; set; }

        [Required]
        [Column("gender_id")]
        [StringLength(5)]
        public string GenderId { get; set; }

        [Required]
        [Column("married_id")]
        [StringLength(5)]
        public string MarriedId { get; set; }

        [Required]
        [Column("bank_id")]
        [StringLength(5)]
        public string BankId { get; set; }

        [Required]
        [Column("no_rekening")]
        [StringLength(25)]
        public string NoRekening { get; set; }

        [Required]
        [Column("email")]
        [StringLength(75)]
        public string Email { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("dt_etr", TypeName = "datetime")]
        public DateTime DtEtr { get; set; }

        [Required]
        [Column("user_etr")]
        [StringLength(21)]
        public string UserEtr { get; set; }

        [Column("dt_update", TypeName = "datetime")]
        public DateTime DtUpdate { get; set; }

        [Required]
        [Column("user_update")]
        [StringLength(21)]
        public string UserUpdate { get; set; }
    }
}