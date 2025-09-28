using TheSocialCebu_Capstone.Models;

namespace TheSocialCebu_Capstone.ViewModels
{
    public class PersonVM
    {
        public string PersonId { get; set; } = null!;

        public string RoleId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public DateOnly BirthDate { get; set; }

        public DateOnly HiredDate { get; set; }

        public bool Status { get; set; }

        public string Gender { get; set; } = null!;

        public virtual Account Account { get; set; } = null!;
        public IEnumerable<Role> Roles { get; set; }
    }
}
