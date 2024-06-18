using System;
using System.Collections.Generic;

namespace ShopMusicProject.Models;

public partial class Stock
{
    public string? Cid { get; set; }

    public string? PdId { get; set; }

    public string? Amount { get; set; }

    public string? Price { get; set; }

    public string? Payment { get; set; }
}
