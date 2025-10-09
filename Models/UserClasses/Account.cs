using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class Account
{
    public string AccountId { get; set; } = null!;

    public string PersonId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public DateTime DateUpdated { get; set; }

    public virtual Person Person { get; set; } = null!;
}
