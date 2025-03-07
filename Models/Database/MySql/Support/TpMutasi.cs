﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackendHrdAgro.Models;

[Table("tp_mutasi")]
[MySqlCharSet("latin1")]
[MySqlCollation("latin1_swedish_ci")]
public partial class TpMutasi
{
    [Key]
    [Column("mutasi_id")]
    [StringLength(5)]
    public string MutasiId { get; set; }

    [Required]
    [Column("employee_id")]
    [StringLength(25)]
    public string EmployeeId { get; set; }

    [Required]
    [Column("department_old_id")]
    [StringLength(21)]
    public string DepartmentOldId { get; set; }

    [Required]
    [Column("department_id")]
    [StringLength(21)]
    public string DepartmentId { get; set; }

    [Required]
    [Column("title_old_id")]
    [StringLength(10)]
    public string TitleOldId { get; set; }

    [Required]
    [Column("title_id")]
    [StringLength(10)]
    public string TitleId { get; set; }

    [Required]
    [Column("level_old_id")]
    [StringLength(10)]
    public string LevelOldId { get; set; }

    [Required]
    [Column("level_id")]
    [StringLength(10)]
    public string LevelId { get; set; }

    [Required]
    [Column("file_name")]
    [StringLength(100)]
    public string FileName { get; set; }

    [Required]
    [Column("type")]
    [StringLength(15)]
    public string Type { get; set; }

    [Column("size")]
    public long Size { get; set; }

    [Required]
    [Column("type_id")]
    [StringLength(5)]
    public string TypeId { get; set; }

    [Column("mutasi_date")]
    public DateTime MutasiDate { get; set; }

    [Required]
    [Column("deskripsi")]
    [StringLength(100)]
    public string Deskripsi { get; set; }

    [Column("status")]
    public int Status { get; set; }
}