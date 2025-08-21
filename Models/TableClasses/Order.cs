using System;
using System.Collections.Generic;
using TheSocialCebu_Capstone.Models.TableClasses;

namespace TheSocialCebu_Capstone.Models.OrderClasses;

public partial class Order
{
    public string OrderId { get; set; } = null!;

    public DateOnly CreatedAt { get; set; }

    public bool Status { get; set; }

    public bool Paid { get; set; }

    public string TableId { get; set; } = null!;

    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Table Table { get; set; } = null!;
}
