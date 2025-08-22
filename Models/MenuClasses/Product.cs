using System;
using System.Collections.Generic;
using TheSocialCebu_Capstone.Models.OrderClasses;

namespace TheSocialCebu_Capstone.Models.MenuClasses;

public partial class Product
{
    public string ProdId { get; set; } = null!;

    public string ProdName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public bool Availability { get; set; }

    public byte[]? ProdImage { get; set; }

    public string SubcategoryId { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual SubCategory Subcategory { get; set; } = null!;
}
