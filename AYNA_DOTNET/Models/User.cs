using System;
using System.Collections.Generic;

namespace Ayna.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserEmail { get; set; } = null!;

    public string UserPassword { get; set; } = null!;

    public string? UserPhone { get; set; }

    public string? UserType { get; set; }

    public string? UserStatus { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual ICollection<Charity> Charities { get; set; } = new List<Charity>();

    public virtual ICollection<Donor> Donors { get; set; } = new List<Donor>();

    public virtual ICollection<Farmer> Farmers { get; set; } = new List<Farmer>();
}
