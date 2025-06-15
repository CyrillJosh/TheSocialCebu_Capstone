using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class Table
{
    public string Id { get; set; } = null!;

    public string TableNumber { get; set; } = null!;

    public byte[]? QrcodeImage { get; set; }

    public bool Status { get; set; }

    public string LocationId { get; set; } = null!;

    public virtual Location Location { get; set; } = null!;
}
