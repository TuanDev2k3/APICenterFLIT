using System.ComponentModel.DataAnnotations.Schema;

namespace WebTinTucFLIC.Models
{
    public class ImageDTO
    {
        public int Id { get; set; }

        public string? Description { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;


    }
}
