﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackendHrdAgro.Models;

[Table("tm_hubungan")]
[MySqlCharSet("latin1")]
[MySqlCollation("latin1_swedish_ci")]
public partial class TmHubungan
{
    [Key]
    [Column("hubungan_id")]
    [StringLength(21)]
    public string HubunganId { get; set; }

    [Required]
    [Column("hubungan_name")]
    [StringLength(21)]
    public string HubunganName { get; set; }

    [Column("is_family")]
    public sbyte IsFamily { get; set; }

    [Column("status")]
    public bool Status { get; set; }
}