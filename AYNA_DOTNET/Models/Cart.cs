using System;
using System.Collections.Generic;

namespace Ayna.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int CartQty { get; set; }

    public decimal? CartPrice { get; set; }

    public int OrdId { get; set; }

    public int BasId { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual Basket Bas { get; set; } = null!;

    public virtual Order Ord { get; set; } = null!;
}
