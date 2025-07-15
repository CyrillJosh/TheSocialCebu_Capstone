using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class User
{
    public string AccountId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual Person Person { get; set; } = null!;
}
