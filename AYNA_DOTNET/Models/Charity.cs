using System;
using System.Collections.Generic;

namespace Ayna.Models;

public partial class Charity
{
    public int CharId { get; set; }

    public string? CharCr { get; set; }

    public string CharName { get; set; } = null!;

    public string? CharLocation { get; set; }

    public string? CharContact { get; set; }

    public int UserId { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual ICollection<DonatReq> DonatReqs { get; set; } = new List<DonatReq>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User User { get; set; } = null!;
}
