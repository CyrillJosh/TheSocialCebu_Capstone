using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TheSocialCebu_Capstone.ViewModels
{
    public class TableVM
    {
        public string Id { get; set; }

        [Required]
        public string TableNumber { get; set; }

        [Required]
        public string LocationId { get; set; }

        public IFormFile QRCodeImageFile { get; set; }

        public SelectList LocationList { get; set; }
    }
}
