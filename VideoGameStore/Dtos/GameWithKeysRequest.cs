using System.ComponentModel.DataAnnotations;

namespace VideoGameStore.Dtos
{
    public class GameWithKeysRequest
    {
        [MinLength(2), MaxLength(100)]
        public string DeveloperTitle { get; set; }

        [MinLength(2), MaxLength(100)]
        public string PublisherTitle { get; set; }

        [Range(0.1, double.MaxValue)]
        public decimal Price { get; set; }

        [MinLength(2), MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MinLength(1)]
        public List<GenreRequest> Genres { get; set; }
        [Required]
        public IFormFile Keys { get; set; }
        [Required]
        public IFormFile Image { get; set; }
    }
}

