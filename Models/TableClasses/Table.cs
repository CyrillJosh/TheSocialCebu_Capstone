using System;
using System.Collections.Generic;
using TheSocialCebu_Capstone.Models.BillingClasses;
using TheSocialCebu_Capstone.Models.OrderClasses;

namespace TheSocialCebu_Capstone.Models.TableClasses;

public partial class Table
{
    public string TableId { get; set; } = null!;

    public string TableNumber { get; set; } = null!;

    public byte[]? QrcodeImage { get; set; }

    public string Status { get; set; } = null!;

    public string LocationId { get; set; } = null!;

    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();

    public virtual Location Location { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
