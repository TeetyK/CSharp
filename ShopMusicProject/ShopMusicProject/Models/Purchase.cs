using System;
using System.Collections.Generic;

namespace ShopMusicProject.Models;

public partial class Purchase
{
    public DateOnly DateAt { get; set; }

    public int PdId { get; set; }

    public double PdPrice { get; set; }

    public int Amount { get; set; }

    public double Cost { get; set; }

    public int Sid { get; set; }
}
