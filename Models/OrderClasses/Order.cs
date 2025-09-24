using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class Order
{
    public string OrderId { get; set; } = null!;

    public string TableId { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public int OrderStatusId { get; set; }
    public int OrderNumber { get; set; }

    public virtual ICollection<BillingOrder> BillingOrders { get; set; } = new List<BillingOrder>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual OrderStatus OrderStatus { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;
}
