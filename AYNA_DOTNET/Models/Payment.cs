using System;
using System.Collections.Generic;

namespace Ayna.Models;

public partial class Payment
{
    public int PayId { get; set; }

    public string? PayMethod { get; set; }

    public DateOnly? PayDate { get; set; }

    public TimeOnly? PayTime { get; set; }

    public string? PayStatus { get; set; }

    public decimal? PayAmount { get; set; }

    public string? TransactionId { get; set; }

    public int OrdId { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual Order Ord { get; set; } = null!;
}
