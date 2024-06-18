using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShopMusicProject.Models;

public partial class Buying
{
    [Required(ErrorMessage = "ต้องระบุรหัสซื้อ")]
    [Display(Name = "รหัสซื้อ")]
    public string BuyId { get; set; } = null!;
    [Required(ErrorMessage = "ต้องระบุรหัสผู้จำหน่าย")]
    [Display(Name = "รหัสผู้จำหน่าย")]
    public string? SupId { get; set; }
    [Required(ErrorMessage = "ต้องระบุวันที่")]
    [Display(Name = "วันที่")]
    public DateOnly? BuyDate { get; set; }
    [Required(ErrorMessage = "ต้องระบุรหัสพนักงาน")]
    [Display(Name = "รหัสพนักงาน")]
    public string? StfId { get; set; }
    [Required(ErrorMessage = "ต้องระบุเลขเอกสาร")]
    [Display(Name = "เลขเอกสาร")]
    public string? BuyDocId { get; set; }
    [Display(Name = "ผู้ขาย")]
    public string? Saleman { get; set; }
    
    [Display(Name = "จำนวน")]
    public double? BuyQty { get; set; }
    [Display(Name = "เงิน")]
    public double? BuyMoney { get; set; }
    [Display(Name = "หมายเหตุ")]
    public string? BuyRemark { get; set; }
}
