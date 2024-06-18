using System;
using System.Collections.Generic;

namespace ShopMusicProject.Models;

public partial class Supplier
{
    public string Sid { get; set; } = null!;

    public string? SupName { get; set; }

    public string? Sphone { get; set; }

    public string? Semail { get; set; }

    public string? Saddress { get; set; }

    public string? Sremark { get; set; }

    public string? SupContact { get; set; }
}
