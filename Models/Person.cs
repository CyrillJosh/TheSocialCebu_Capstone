using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class Person
{
    public string UserId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public byte? Age { get; set; }

    public string? Gender { get; set; }

    public DateOnly BirthDate { get; set; }

    public DateOnly HiredDate { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
