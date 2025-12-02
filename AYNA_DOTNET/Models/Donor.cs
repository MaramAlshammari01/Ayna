using System;
using System.Collections.Generic;

namespace Ayna.Models;

public partial class Donor
{
    public int DonorId { get; set; }

    public string DonorFirstName { get; set; } = null!;

    public string DonorLastName { get; set; } = null!;

    public int UserId { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User User { get; set; } = null!;
}
