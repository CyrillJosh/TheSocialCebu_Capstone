using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class OrderItem
{
    public string OrderItemId { get; set; } = null!;

    public string OrderId { get; set; } = null!;

    public string ProdId { get; set; } = null!;

    public int? Quantity { get; set; }

    public string? Instructions { get; set; }

    public int OrderItemStatusId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual OrderItemStatus OrderItemStatus { get; set; } = null!;

    public virtual Product Prod { get; set; } = null!;
}
