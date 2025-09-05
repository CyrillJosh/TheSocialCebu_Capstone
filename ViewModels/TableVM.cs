using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TheSocialCebu_Capstone.Models.TableClasses;

namespace TheSocialCebu_Capstone.ViewModels
{
    public class TableVM
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Please enter a table name")]
        public string TableNumber { get; set; }

        [Required(ErrorMessage = "Please select a location")]
        public int LocationId { get; set; }

        public int StatusId { get; set; }

        public IFormFile? QRCodeImageFile { get; set; }

        public string? QRCodeBase64 { get; set; }

        public byte[]? ExistingQRCodeImage { get; set; }
        [BindNever]
        public IEnumerable<SelectListItem>? LocationList { get; set; }

        [BindNever]
        public List<TableStatus>? StatusList { get; set; }
    }
}
