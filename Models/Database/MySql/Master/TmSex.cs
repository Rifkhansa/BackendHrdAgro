﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackendHrdAgro.Models;

[Table("tm_sex")]
[MySqlCharSet("latin1")]
[MySqlCollation("latin1_swedish_ci")]
public partial class TmSex
{
    [Key]
    [Column("gender_id")]
    [StringLength(8)]
    public string GenderId { get; set; }

    [Required]
    [Column("gender_name")]
    [StringLength(20)]
    public string GenderName { get; set; }

    [Column("status")]
    public bool Status { get; set; }
}