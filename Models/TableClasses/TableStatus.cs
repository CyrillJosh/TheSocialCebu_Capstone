using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models.TableClasses;

public partial class TableStatus
{
    public int TableStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();
}
