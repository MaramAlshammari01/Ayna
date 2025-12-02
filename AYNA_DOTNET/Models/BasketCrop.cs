using System;
using System.Collections.Generic;

namespace Ayna.Models;

public partial class BasketCrop
{
    public int BcId { get; set; }

    public int BcQty { get; set; }

    public int CroId { get; set; }

    public int BasId { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual Basket Bas { get; set; } = null!;

    public virtual Crop Cro { get; set; } = null!;
}
