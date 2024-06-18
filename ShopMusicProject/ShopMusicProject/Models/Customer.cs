using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopMusicProject.Models;

public partial class Customer
{
    [Display(Name = "รหัส")]
    public string CusId { get; set; } = null!;
    [Display(Name = "ชื่อ")]
    public string Fname { get; set; } = null!;
    [Display(Name = "นามสกุล")]
    public string? Lname { get; set; }
    [Display(Name = "อีเมล")]
    public string Email { get; set; } = null!;
    [Display(Name = "ที่อยู่")]
    public string? Address { get; set; }
    [Required(ErrorMessage = "ต้องระบุรหัสผ่าน")]
    [Display(Name = "รหัสผ่าน")]
    public string Password { get; set; } = null!;
    [Required(ErrorMessage = "ต้องระบุผู้ใช้งาน")]
    [Display(Name = "ผู้ใช้งาน")]
    public string Username { get; set; } = null!;
  
    public DateOnly? CreateAt { get; set; }
  
    public DateOnly? LoginAt { get; set; }
  
    public string? Phone { get; set; }

    [Compare(nameof(Password), ErrorMessage = "รหัสผ่านไม่ตรงกัน")]
    [NotMapped]
    public string ConFirmPassword { get; set; } = null!;
}
