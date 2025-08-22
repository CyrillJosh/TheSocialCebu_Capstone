using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models.UserCLasses;

public partial class Account
{
    public string AccountId { get; set; } = null!;

    public string PersonId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual Person Person { get; set; } = null!;
}
