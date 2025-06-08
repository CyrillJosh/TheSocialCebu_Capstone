using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TheSocialCebu_Capstone.ViewModels
{
    public class ProductVM
    {
        public Guid? ProId { get; set; }

        [Required(ErrorMessage = "Please enter a name")]
        public string ProdName { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Please enter a price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Please select a subcategory")]
        public Guid SubcategoryId { get; set; }

        public bool Availability { get; set; } = true;

        public IFormFile? UploadImage { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }
        public IEnumerable<SelectListItem>? Subcategories { get; set; }

        public byte[]? ExistingImage { get; set; }
    }

}
