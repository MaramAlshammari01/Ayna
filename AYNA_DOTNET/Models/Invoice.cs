using System;
using System.Collections.Generic;

namespace Ayna.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public DateOnly? InvoiceDate { get; set; }

    public TimeOnly? InvoiceTime { get; set; }

    public decimal? InvoiceAmountPaid { get; set; }

    public int PayId { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual Payment Pay { get; set; } = null!;
}
