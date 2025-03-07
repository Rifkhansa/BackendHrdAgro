﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackendHrdAgro.Models;

[Table("tm_department")]
[MySqlCharSet("latin1")]
[MySqlCollation("latin1_swedish_ci")]
public partial class TmDepartment
{
    [Key]
    [Column("department_id")]
    [StringLength(21)]
    public string DepartmentId { get; set; }

    [Required]
    [Column("department_name")]
    [StringLength(75)]
    public string DepartmentName { get; set; }

    [Required]
    [Column("div_id")]
    [StringLength(21)]
    public string DivId { get; set; }

    [Required]
    [Column("dept_code")]
    [StringLength(3)]
    public string DeptCode { get; set; }

    [Column("description")]
    [StringLength(200)]
    public string Description { get; set; }

    [Column("status")]
    public bool Status { get; set; }

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