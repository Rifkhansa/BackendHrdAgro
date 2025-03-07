﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackendHrdAgro.Models;

[Table("tp_employee_family")]
[MySqlCharSet("latin1")]
[MySqlCollation("latin1_swedish_ci")]
public partial class TpEmployeeFamily
{
    [Key]
    [Column("family_id")]
    [StringLength(21)]
    public string FamilyId { get; set; }

    [Required]
    [Column("employee_id")]
    [StringLength(21)]
    public string EmployeeId { get; set; }

    [Required]
    [Column("hubungan_id")]
    [StringLength(21)]
    public string HubunganId { get; set; }

    [Required]
    [Column("family_name")]
    [StringLength(100)]
    public string FamilyName { get; set; }

    [Required]
    [Column("tempat_lahir")]
    [StringLength(75)]
    public string TempatLahir { get; set; }

    [Required]
    [Column("tanggal_lahir")]
    [StringLength(25)]
    public string TanggalLahir { get; set; }

    [Required]
    [Column("no_telp")]
    [StringLength(15)]
    public string NoTelp { get; set; }

    [Column("status")]
    public int Status { get; set; }
}