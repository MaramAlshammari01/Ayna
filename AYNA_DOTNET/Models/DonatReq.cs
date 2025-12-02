using System;
using System.Collections.Generic;

namespace Ayna.Models;

public partial class DonatReq
{
    public int ReqId { get; set; }

    public string? ReqStatus { get; set; }

    public string? ReqDonation { get; set; }

    public string? DonationRequestPickupDuration { get; set; }

    public int DonId { get; set; }

    public int CharId { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual Charity Char { get; set; } = null!;

    public virtual Donation Don { get; set; } = null!;
}
