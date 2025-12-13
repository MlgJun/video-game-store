using System.ComponentModel.DataAnnotations;

namespace VideoGameStore.Dtos
{
    public record class FilterRequest(decimal? MinPrice, decimal? MaxPrice, [MaxLength(100)] string? GameTitle, List<string>? Genres);
}
