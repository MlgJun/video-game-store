using System.ComponentModel.DataAnnotations;

namespace VideoGameStore.Dtos
{
    public record class Pageable([Range(0, int.MaxValue)] int Page = 1, [Range(1, 100)] int PageSize = 20);
}
