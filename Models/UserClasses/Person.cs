using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models.UserClasses;

public partial class Person
{
    public string PersonId { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public DateOnly HiredDate { get; set; }

    public bool Status { get; set; }

    public string Gender { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();

    public virtual Role Role { get; set; } = null!;
}
