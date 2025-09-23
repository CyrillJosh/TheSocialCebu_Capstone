using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class DiscountType
{
    public string DiscountTypeId { get; set; } = null!;

    public string DiscountName { get; set; } = null!;

    public decimal Percentage { get; set; }

    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();
}
