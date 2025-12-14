using Microsoft.EntityFrameworkCore;

namespace VideoGameStore.Dtos
{
    public record class Page<T>(
            IReadOnlyList<T> Content,
            int PageNumber,
            int PageSize,
            long TotalElements,
            int TotalPages
    );  
}
