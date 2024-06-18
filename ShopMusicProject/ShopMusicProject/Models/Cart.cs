using System;
using System.Collections.Generic;

namespace ShopMusicProject.Models;

public partial class Cart
{
    public string CartId { get; set; } = null!;

    public string? CusId { get; set; }

    public double? Cmoney { get; set; }

    public DateOnly? CdateAt { get; set; }

    public string? Cf { get; set; }

    public string? Cpay { get; set; }

    public string? Csend { get; set; }

    public string? Cvoid { get; set; }

    public double? Cqty { get; set; }
}
