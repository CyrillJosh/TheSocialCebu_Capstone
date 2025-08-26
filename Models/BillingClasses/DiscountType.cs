using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class DiscountType
{
    public string DiscountTypeId { get; set; } = null!;

    public string DiscountName { get; set; } = null!;

    public decimal Percentage { get; set; }

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
}
