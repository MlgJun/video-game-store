using System.ComponentModel.DataAnnotations;

namespace VideoGameStore.Dtos
{
    public record class FilterRequest([Range(0, double.MaxValue)] decimal? MinPrice,
        [Range(0, double.MaxValue)] decimal? MaxPrice,
        [MaxLength(100)] string? GameTitle,
        List<string>? Genres);
}
