using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class Billing
{
    public string BillingId { get; set; } = null!;

    public string TableId { get; set; } = null!;

    public DateTime? BillingTime { get; set; }

    public decimal? Subtotal { get; set; }

    public decimal? VatAmount { get; set; }

    public decimal? ServiceCharge { get; set; }

    public decimal? GrandTotal { get; set; }

    public string? DiscountId { get; set; }

    public virtual ICollection<BillingOrder> BillingOrders { get; set; } = new List<BillingOrder>();

    public virtual Payment? Payment { get; set; }

    public virtual Table Table { get; set; } = null!;

    public virtual DiscountType Discount { get; set; } = null!;

}
