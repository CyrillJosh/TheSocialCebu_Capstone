using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models.OrderClasses;

public partial class Billing
{
    public string BillingId { get; set; } = null!;

    public int Subtotal { get; set; }

    public int Vat { get; set; }

    public int Total { get; set; }

    public DateOnly CreatedAt { get; set; }

    public string DiscountId { get; set; } = null!;

    public string OrderId { get; set; } = null!;

    public virtual Discount Discount { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
