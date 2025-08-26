using System;
using System.Collections.Generic;
using TheSocialCebu_Capstone.Models;

namespace TheSocialCebu_Capstone.Models.BillingClasses;

public partial class BillingOrder
{
    public string BillingOrderId { get; set; } = null!;

    public string BillingId { get; set; } = null!;

    public string OrderId { get; set; } = null!;

    public virtual Billing Billing { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
