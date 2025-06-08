using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class Product
{
    public Guid ProId { get; set; }

    public string ProdName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public Guid CategoryId { get; set; }

    public Guid SubcategoryId { get; set; }

    public bool Availability { get; set; }

    public byte[]? ProdImage { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual SubCategory Subcategory { get; set; } = null!;
}
