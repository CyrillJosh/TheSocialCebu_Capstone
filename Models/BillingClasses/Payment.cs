using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class Payment
{
    public string PaymentId { get; set; } = null!;

    public decimal? AmountPaid { get; set; }

    public DateTime? PaymentTime { get; set; }

    public virtual Billing PaymentNavigation { get; set; } = null!;
}
