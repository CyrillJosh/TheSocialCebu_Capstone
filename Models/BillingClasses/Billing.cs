using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class Billing
{
    public string BillingId { get; set; } = null!;

    public string SessionId { get; set; } = null!;

    public DateTime? BillingTime { get; set; }

    public decimal? Subtotal { get; set; }

    public decimal? VatAmount { get; set; }

    public decimal? ServiceCharge { get; set; }

    public decimal? GrandTotal { get; set; }

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual TableSession Session { get; set; } = null!;
}
