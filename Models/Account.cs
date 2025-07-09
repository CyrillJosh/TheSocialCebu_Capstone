using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class Account
{
    public string AccountId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool? Status { get; set; }

    public virtual User User { get; set; } = null!;
}
