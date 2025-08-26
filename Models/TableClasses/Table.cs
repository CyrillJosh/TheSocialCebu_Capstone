using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class Table
{
    public string TableId { get; set; } = null!;

    public string TableNumber { get; set; } = null!;

    public byte[]? QrcodeImage { get; set; }

    public int TableStatusId { get; set; }

    public string LocationId { get; set; } = null!;

    public virtual Location Location { get; set; } = null!;

    public virtual ICollection<TableSession> TableSessions { get; set; } = new List<TableSession>();

    public virtual TableStatus TableStatus { get; set; } = null!;
}
