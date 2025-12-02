using System;
using System.Collections.Generic;

namespace Ayna.Models;

public partial class Order
{
    public int OrdId { get; set; }

    public DateOnly OrdDate { get; set; }

    public TimeOnly? OrdTime { get; set; }

    public decimal? OrdPrice { get; set; }

    public string? OrdStatus { get; set; }

    public int DonorId { get; set; }

    public int? CharId { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Charity? Char { get; set; }

    public virtual Donor Donor { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
