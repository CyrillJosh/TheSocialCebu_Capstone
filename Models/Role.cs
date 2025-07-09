using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class Role
{
    public string RoleId { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
