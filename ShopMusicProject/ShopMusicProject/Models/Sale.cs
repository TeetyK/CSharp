using System;
using System.Collections.Generic;

namespace ShopMusicProject.Models;

public partial class Sale
{
    public DateOnly DateAt { get; set; }

    public int PdId { get; set; }

    public double PdPrice { get; set; }

    public int Amount { get; set; }

    public int Cid { get; set; }
}
