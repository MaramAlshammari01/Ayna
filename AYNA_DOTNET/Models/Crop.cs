using System;
using System.Collections.Generic;

namespace Ayna.Models;

public partial class Crop
{
    public int CroId { get; set; }

    public string CroName { get; set; } = null!;

    public string? CroType { get; set; }

    public decimal? CroWeight { get; set; }

    public int? CroQuantity { get; set; }

    public string? CroUnit { get; set; }

    public int FarId { get; set; }

    public DateTime AddedAt { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public string? CroShelfLife { get; set; }

    public virtual ICollection<BasketCrop> BasketCrops { get; set; } = new List<BasketCrop>();

    public virtual ICollection<DonationCrop> DonationCrops { get; set; } = new List<DonationCrop>();

    public virtual Farmer Far { get; set; } = null!;
}
