﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackendHrdAgro.Models;

[Table("tm_religi")]
[MySqlCharSet("latin1")]
[MySqlCollation("latin1_swedish_ci")]
public partial class TmReligi
{
    [Key]
    [Column("religi_id")]
    public int ReligiId { get; set; }

    [Required]
    [Column("religi_name")]
    [StringLength(15)]
    public string ReligiName { get; set; }

    [Column("status")]
    public bool Status { get; set; }
}