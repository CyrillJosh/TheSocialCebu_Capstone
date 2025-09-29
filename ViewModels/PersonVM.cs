using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TheSocialCebu_Capstone.Models;

namespace TheSocialCebu_Capstone.ViewModels
{
    public class PersonVM
    {
        public string? PersonId { get; set; } = null;

        public string RoleId { get; set; } = null!;

        [Required(ErrorMessage = "Birth date is required")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        public DateTime HiredDate { get; set; }

        public bool Status { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; } = null!;

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required", AllowEmptyStrings = false)]
        public string? Password { get; set; }

        [AllowNull]
        [BindNever]
        public IEnumerable<Role>? Roles { get; set; }
    }
}
