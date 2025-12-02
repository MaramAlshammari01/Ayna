using System;
using System.Collections.Generic;

namespace Ayna.Models;

public partial class Basket
{
    public int BasId { get; set; }

    public string? BasContent { get; set; }

    public decimal? BasPrice { get; set; }

    public int? BasQty { get; set; }

    public int FarId { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual ICollection<BasketCrop> BasketCrops { get; set; } = new List<BasketCrop>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Farmer Far { get; set; } = null!;
}
