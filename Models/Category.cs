using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class Category
{
    public string CategoryId { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
}
