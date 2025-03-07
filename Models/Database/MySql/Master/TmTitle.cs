﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackendHrdAgro.Models;

[Table("tm_title")]
[MySqlCharSet("latin1")]
[MySqlCollation("latin1_swedish_ci")]
public partial class TmTitle
{
    [Key]
    [Column("title_id")]
    [StringLength(21)]
    public string TitleId { get; set; }

    [Required]
    [Column("title_name")]
    [StringLength(50)]
    public string TitleName { get; set; }

    [Column("is_overtime")]
    public sbyte IsOvertime { get; set; }

    [Column("is_cut_absentee")]
    public sbyte IsCutAbsentee { get; set; }

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