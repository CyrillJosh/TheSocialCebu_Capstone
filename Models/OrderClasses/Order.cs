using System;
using System.Collections.Generic;
using TheSocialCebu_Capstone.Models.BillingClasses;
using TheSocialCebu_Capstone.Models.TableClasses;

namespace TheSocialCebu_Capstone.Models.OrderClasses;

public partial class Order
{
    public string OrderId { get; set; } = null!;

    public string TableId { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<BillingOrder> BillingOrders { get; set; } = new List<BillingOrder>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Table Table { get; set; } = null!;
}
