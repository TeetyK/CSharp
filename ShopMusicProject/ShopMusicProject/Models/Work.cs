using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopMusicProject.Models;

public partial class Work
{
    [Key]
    public string EmId { get; set; } = null!;
    [Key]
    public DateOnly? WorkDate { get; set; }

    public TimeOnly? WorkIn { get; set; }

    public TimeOnly? WorkOut { get; set; }
}
