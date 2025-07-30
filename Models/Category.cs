using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheSocialCebu_Capstone.Models;

public partial class Category
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string CategoryId { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
}
