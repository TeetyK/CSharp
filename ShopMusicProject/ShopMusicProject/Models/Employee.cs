using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopMusicProject.Models;

public partial class Employee
{

    [Display(Name = "รหัสพนักงาน")]
    public string EmId { get; set; } = null!;
    [Display(Name = "ผู้ใช้งาน")]
    public string? Username { get; set; }
    [Display(Name = "รหัสผ่าน")]
    public string? Password { get; set; }
    [Display(Name = "ชื่อ")]
    public string? Fname { get; set; }
    [Display(Name = "นามสกุล")]
    public string? Lname { get; set; }
    public DateOnly? StartAt { get; set; }
    public DateOnly? QuitAt { get; set; }
    [Display(Name = "หน้าที่")]
    public string? Type { get; set; }
}
