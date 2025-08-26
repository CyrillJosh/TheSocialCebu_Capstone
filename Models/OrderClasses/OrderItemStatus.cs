using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class OrderItemStatus
{
    public int OrderItemStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
