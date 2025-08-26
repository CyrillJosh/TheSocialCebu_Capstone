using System;
using System.Collections.Generic;
using TheSocialCebu_Capstone.Models.UserClasses;

namespace TheSocialCebu_Capstone.Models;

public partial class Discount
{
    public string DiscountId { get; set; } = null!;

    public string BillingId { get; set; } = null!;

    public string DiscountTypeId { get; set; } = null!;

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public virtual Person? ApprovedByNavigation { get; set; }

    public virtual Billing Billing { get; set; } = null!;

    public virtual DiscountType DiscountType { get; set; } = null!;
}
