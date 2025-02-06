﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackendHrdAgro.Models;

[Table("tm_menu")]
public partial class TmMenu
{
    [Key]
    [Column("id")]
    [StringLength(21)]
    public string Id { get; set; }

    [Required]
    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; }

    [Column("caption")]
    [StringLength(50)]
    public string Caption { get; set; }

    [Column("url")]
    [StringLength(250)]
    public string Url { get; set; }

    [Column("icon")]
    [StringLength(50)]
    public string Icon { get; set; }

    [Column("parent")]
    [StringLength(100)]
    public string Parent { get; set; }

    [Column("hierarchy")]
    public int? Hierarchy { get; set; }

    [Required]
    [Column("created_by")]
    [StringLength(21)]
    public string CreatedBy { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_by")]
    [StringLength(21)]
    public string UpdatedBy { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }

    [Column("deleted_by")]
    [StringLength(21)]
    public string DeletedBy { get; set; }
}