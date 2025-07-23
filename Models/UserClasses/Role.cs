using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models.UserClasses;

public partial class Role
{
    public string RoleId { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
