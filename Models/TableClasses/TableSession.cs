using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class TableSession
{
    public string SessionId { get; set; } = null!;

    public string TableId { get; set; } = null!;

    public DateTime StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Table Table { get; set; } = null!;
}
