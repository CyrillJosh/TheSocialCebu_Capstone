using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class Table
{
    public string TableId { get; set; } = null!;

    public string TableNumber { get; set; } = null!;

    public byte[]? QrcodeImage { get; set; }

    public int TableStatusId { get; set; }

    public int LocationId { get; set; }

    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();

    public virtual Location Location { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual TableStatus TableStatus { get; set; } = null!;
}
