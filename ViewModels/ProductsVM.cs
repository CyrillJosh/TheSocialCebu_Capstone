using System.ComponentModel.DataAnnotations;
using TheSocialCebu_Capstone.Models;

namespace TheSocialCebu_Capstone.ViewModels
{
    public class ProductVM
    {
        public string? ProdId { get; set; }

        [Required(ErrorMessage = "Please enter a name")]
        public string ProdName { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Please enter a price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public string CategoryId { get; set; }

        [Required(ErrorMessage = "Please select a subcategory")]
        public string SubcategoryId { get; set; }

        public bool Availability { get; set; } = true;

        public IFormFile? UploadImage { get; set; }

        public List<Category>? Categories { get; set; }
        public List<SubCategory>? Subcategories { get; set; }

        public byte[]? ExistingImage { get; set; }
    }
}
