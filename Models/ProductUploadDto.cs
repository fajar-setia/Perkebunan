namespace Perkebunan.Models
{
    public class ProductUploadDto
    {
        public string NameProduct { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public IFormFile Image { get; set; } = null!;
    }
}
