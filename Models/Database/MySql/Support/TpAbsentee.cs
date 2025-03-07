﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BackendHrdAgro.Models;

[Keyless]
[Table("tp_absentee")]
[MySqlCharSet("latin1")]
[MySqlCollation("latin1_swedish_ci")]
public partial class TpAbsentee
{
    [Required]
    [Column("employee_id")]
    [StringLength(21)]
    public string EmployeeId { get; set; }

    [Column("date")]
    public DateOnly Date { get; set; }

    [Required]
    [Column("in")]
    [StringLength(8)]
    public string In { get; set; }

    [Required]
    [Column("out")]
    [StringLength(8)]
    public string Out { get; set; }

    [Column("hour")]
    public float Hour { get; set; }

    [Column("hour_effective")]
    public float HourEffective { get; set; }

    /// <summary>
    /// 0: tidak ada overtime; 1: overtime; 
    /// </summary>
    [Column("is_overtime")]
    public sbyte IsOvertime { get; set; }

    [Column("amount_overtime")]
    public float? AmountOvertime { get; set; }

    [Column("overtime_reason", TypeName = "text")]
    public string OvertimeReason { get; set; }

    /// <summary>
    /// 0: belum ada alasan; 99:ada, perlu approval; -1 : reject; 1: approve
    /// </summary>
    [Column("is_approve_overtime")]
    public short? IsApproveOvertime { get; set; }

    [Column("approve_reason_overtime", TypeName = "text")]
    public string ApproveReasonOvertime { get; set; }

    [Column("approve_overtime_by")]
    [StringLength(21)]
    public string ApproveOvertimeBy { get; set; }

    [Column("approve_overtime_dt", TypeName = "datetime")]
    public DateTime? ApproveOvertimeDt { get; set; }

    [Column("is_cut_absentee")]
    public sbyte IsCutAbsentee { get; set; }

    /// <summary>
    /// N: tidak ada potongan; W: terlambat menit ke 14; C: terlambat 15 menit ke atas atau potong cuti 1/4; A:tidak masuk; I: izin
    /// </summary>
    [Column("cut_absentee_type")]
    [StringLength(1)]
    public string CutAbsenteeType { get; set; }

    [Column("absentee_complain", TypeName = "text")]
    public string AbsenteeComplain { get; set; }

    /// <summary>
    /// 0: belum ada complain; 99:ada, perlu approval; -1 : reject; 1: approve
    /// </summary>
    [Column("is_approve_complain")]
    public short? IsApproveComplain { get; set; }

    [Column("approve_reason_complain", TypeName = "text")]
    public string ApproveReasonComplain { get; set; }

    [Column("approve_complain_by")]
    [StringLength(21)]
    public string ApproveComplainBy { get; set; }

    [Column("approve_complain_dt", TypeName = "datetime")]
    public DateTime? ApproveComplainDt { get; set; }

    [Column("status")]
    public sbyte Status { get; set; }

    [Required]
    [Column("file_name")]
    [StringLength(50)]
    public string FileName { get; set; }
}