using System;
using System.Collections.Generic;
using TheSocialCebu_Capstone.Models.MenuClasses;

namespace TheSocialCebu_Capstone.Models.OrderClasses;

public partial class OrderItem
{
    public string OrderItemId { get; set; } = null!;

    public int Qty { get; set; }

    public string Instructions { get; set; } = null!;

    public bool Status { get; set; }
    public string ProdId { get; set; } = null!;

    public string OrderId { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual Product Prod { get; set; } = null!;
}
