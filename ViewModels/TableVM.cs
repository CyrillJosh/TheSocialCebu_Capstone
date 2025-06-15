using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TheSocialCebu_Capstone.ViewModels
{
    public class TableVM
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Please enter a table name")]
        public string TableNumber { get; set; }

        [Required(ErrorMessage = "Please select a location")]
        public string LocationId { get; set; }

        public bool Status { get; set; }

        public IFormFile? QRCodeImageFile { get; set; }

        public string? QRCodeBase64 { get; set; }

        public byte[]? ExistingQRCodeImage { get; set; }

        public IEnumerable<SelectListItem>? LocationList { get; set; }
    }
}
