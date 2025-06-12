using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class Product
{
    public string ProdId { get; set; } = null!;

    public string ProdName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string CategoryId { get; set; } = null!;

    public string SubcategoryId { get; set; } = null!;

    public bool Availability { get; set; }

    public byte[]? ProdImage { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual SubCategory Subcategory { get; set; } = null!;
}
