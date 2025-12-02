using System;
using System.Collections.Generic;

namespace Ayna.Models;

public partial class PickUp
{
    public int PickId { get; set; }

    public DateTime? PickTime { get; set; }

    public string? PickLocation { get; set; }

    public int DonId { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual Donation Don { get; set; } = null!;
}
