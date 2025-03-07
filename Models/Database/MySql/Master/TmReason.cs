﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackendHrdAgro.Models;

[Table("tm_reason")]
[MySqlCharSet("latin1")]
[MySqlCollation("latin1_swedish_ci")]
public partial class TmReason
{
    [Key]
    [Column("reason_id")]
    [StringLength(5)]
    public string ReasonId { get; set; }

    [Required]
    [Column("reason_name")]
    [StringLength(50)]
    public string ReasonName { get; set; }

    [Column("status")]
    public bool Status { get; set; }
}