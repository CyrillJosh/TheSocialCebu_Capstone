using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class SubCategory
{
    public string SubcategoryId { get; set; } = null!;

    public string SubcategoryName { get; set; } = null!;

    public string CategoryId { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
