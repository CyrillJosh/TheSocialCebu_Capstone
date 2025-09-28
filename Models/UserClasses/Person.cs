using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class Person
{
    public string PersonId { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public DateTime HiredDate { get; set; }

    public bool Status { get; set; }

    public string Gender { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
