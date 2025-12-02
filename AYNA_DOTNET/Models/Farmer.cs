using System;
using System.Collections.Generic;

namespace Ayna.Models;

public partial class Farmer
{
    public int FarId { get; set; }

    public string FarFirstName { get; set; } = null!;

    public string FarLastName { get; set; } = null!;

    public string? FarLocation { get; set; }

    public int UserId { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual ICollection<Basket> Baskets { get; set; } = new List<Basket>();

    public virtual ICollection<Crop> Crops { get; set; } = new List<Crop>();

    public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();

    public virtual User User { get; set; } = null!;
}
