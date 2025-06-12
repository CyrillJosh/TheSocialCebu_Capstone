using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class Location
{
    public string LocationId { get; set; } = null!;

    public string LocationName { get; set; } = null!;

    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();
}
