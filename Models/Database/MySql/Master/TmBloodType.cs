﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackendHrdAgro.Models.Database.MySql;

[Table("tm_blood_type")]
[MySqlCharSet("latin1")]
[MySqlCollation("latin1_swedish_ci")]
public partial class TmBloodType
{
    [Key]
    [Column("blood_type_id")]
    public int BloodTypeId { get; set; }

    [Required]
    [Column("blood_type_name")]
    [StringLength(5)]
    public string BloodTypeName { get; set; }

    [Column("status")]
    public sbyte Status { get; set; }
}