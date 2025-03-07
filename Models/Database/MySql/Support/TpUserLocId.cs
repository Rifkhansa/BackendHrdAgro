﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackendHrdAgro.Models;

[Table("tp_user_loc_id")]
public partial class TpUserLocId
{
    [Required]
    [Column("user_id")]
    [StringLength(21)]
    public string UserId { get; set; }

    [Required]
    [Column("loc_id")]
    [StringLength(21)]
    public string LocId { get; set; }

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

    [Key]
    [Column("id")]
    public int Id { get; set; }
}