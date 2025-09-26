using System;
using System.Collections.Generic;
using TheSocialCebu_Capstone.Models.BillingClasses;

namespace TheSocialCebu_Capstone.Models;

public partial class DiscountType
{
    public string DiscountTypeId { get; set; } = null!;

    public string DiscountName { get; set; } = null!;

    public decimal Percentage { get; set; }

    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();
    public virtual DiscountDetail? DiscountDetail { get; set; }

}
