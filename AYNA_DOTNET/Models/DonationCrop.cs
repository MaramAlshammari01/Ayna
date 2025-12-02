using System;
using System.Collections.Generic;

namespace Ayna.Models;

public partial class DonationCrop
{
    public int DcId { get; set; }

    public int DcQuantity { get; set; }

    public int DonId { get; set; }

    public int CroId { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual Crop Cro { get; set; } = null!;

    public virtual Donation Don { get; set; } = null!;
}
