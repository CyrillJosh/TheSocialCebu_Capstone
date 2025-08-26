using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class OrderStatus
{
    public int OrderStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
