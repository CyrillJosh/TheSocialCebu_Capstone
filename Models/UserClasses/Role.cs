using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models.UserClasses;

public partial class Role
{
    public string RoleId { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public virtual ICollection<Person> People { get; set; } = new List<Person>();
}
