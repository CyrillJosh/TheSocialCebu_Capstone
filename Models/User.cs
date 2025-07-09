using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int? Age { get; set; }

    public string? Gender { get; set; }

    public DateOnly BirthDate { get; set; }

    public DateOnly HiredDate { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Role Role { get; set; } = null!;
}
