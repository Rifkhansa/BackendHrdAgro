﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackendHrdAgro.Models;

[Table("tm_users")]
[MySqlCharSet("latin1")]
[MySqlCollation("latin1_swedish_ci")]
public partial class TmUser
{
    [Key]
    [Column("user_id")]
    [StringLength(21)]
    public string UserId { get; set; }

    [Column("remark")]
    [StringLength(200)]
    public string Remark { get; set; }

    [Required]
    [Column("user_name")]
    [StringLength(21)]
    public string UserName { get; set; }

    [Column("user_full_name")]
    [StringLength(50)]
    public string UserFullName { get; set; }

    [Required]
    [Column("employee_id")]
    [StringLength(21)]
    public string EmployeeId { get; set; }

    [Required]
    [Column("password")]
    [StringLength(250)]
    public string Password { get; set; }

    [Required]
    [Column("email")]
    [StringLength(50)]
    public string Email { get; set; }

    [Column("group_id")]
    [StringLength(21)]
    public string GroupId { get; set; }

    [Column("all_employee_allowed")]
    public sbyte AllEmployeeAllowed { get; set; }

    [Column("is_allowed_bjb")]
    public int IsAllowedBjb { get; set; }

    [Column("last_login", TypeName = "datetime")]
    public DateTime? LastLogin { get; set; }

    [Column("last_change_password", TypeName = "datetime")]
    public DateTime? LastChangePassword { get; set; }

    [Column("status")]
    public int Status { get; set; }

    [Required]
    [Column("user_etr")]
    [StringLength(21)]
    public string UserEtr { get; set; }

    [Column("dt_etr")]
    public DateTime DtEtr { get; set; }

    [Required]
    [Column("user_update")]
    [StringLength(21)]
    public string UserUpdate { get; set; }

    [Column("dt_update")]
    public DateTime DtUpdate { get; set; }
}