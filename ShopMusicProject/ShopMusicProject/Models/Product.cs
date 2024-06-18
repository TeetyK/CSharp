using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopMusicProject.Models;

public partial class Product
{
    [Display(Name = "รหัสสินค้า")]
    public string Id { get; set; } = null!;
    [Display(Name = "ชื่อสินค้า")]
    public string Name { get; set; } = null!;
    [Display(Name = "ราคา")]
    public double? Price { get; set; }
    [Display(Name = "คงเหลือสินค้า")]
    public double? Stock { get; set; }
    [Display(Name = "ต้นทุน")]
    public double? Cost { get; set; }
    [Display(Name = "ซื้อล่าสุด")]
    public DateOnly? LastbuyAt { get; set; }
    [Display(Name = "ขายล่าสุด")]
    public DateOnly? LastsaleAt { get; set; }
    [Display(Name = "ยื่อห้อ")]
    public int? Brandid { get; set; }
    [Display(Name = "หมวดหมู่")]
    public int? Catid { get; set; }
}
