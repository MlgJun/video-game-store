using System.ComponentModel.DataAnnotations;

namespace VideoGameStore.Dtos
{
    public record class GenreRequest([MinLength(1)]string Title);
}
