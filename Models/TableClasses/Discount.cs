using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models.OrderClasses;

public partial class Discount
{
    public string DiscountId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Rate { get; set; }

    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();
}
