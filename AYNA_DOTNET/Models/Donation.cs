using System;
using System.Collections.Generic;

namespace Ayna.Models;

public partial class Donation
{
    public int DonId { get; set; }

    public DateTime DonDate { get; set; }

    public string? DonStatus { get; set; }

    public string? DonDescription { get; set; }

    public int FarId { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual ICollection<DonatReq> DonatReqs { get; set; } = new List<DonatReq>();

    public virtual ICollection<DonationCrop> DonationCrops { get; set; } = new List<DonationCrop>();

    public virtual Farmer Far { get; set; } = null!;

    public virtual ICollection<PickUp> PickUps { get; set; } = new List<PickUp>();
}
