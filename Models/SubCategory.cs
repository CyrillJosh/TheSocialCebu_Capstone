using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class SubCategory
{
    public Guid SubcategoryId { get; set; }

    public string SubcategoryName { get; set; } = null!;

    public Guid CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
