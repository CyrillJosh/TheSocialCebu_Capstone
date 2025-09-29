
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TheSocialCebu_Capstone.Models;

namespace TheSocialCebu_Capstone.ViewModels
{
    public class PersonUpdateVM
    {
        public string PersonId { get; set; } = null!;
        public string RoleId { get; set; } = null!;

        [Required(ErrorMessage = "Birth date is required")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? HiredDate { get; set; }

        public bool Status { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; } = null!;

        [AllowNull]
        [BindNever]
        public IEnumerable<Role>? Roles { get; set; }
    }

}
