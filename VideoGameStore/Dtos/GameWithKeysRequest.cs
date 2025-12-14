using System.ComponentModel.DataAnnotations;

namespace VideoGameStore.Dtos
{
    public record class GameWithKeysRequest([MinLength(2)][MaxLength(100)] string DeveloperTitle,
         [MinLength(2)][MaxLength(100)] string PublisherTitle,
         [Range(0.1, double.MaxValue)] decimal Price,
         [MinLength(2)][MaxLength(100)] string Title,
         [MaxLength(1000)] string? Description,
         [MinLength(1)] List<GenreRequest> Genres,
         [Required] IFormFile Keys);
}
