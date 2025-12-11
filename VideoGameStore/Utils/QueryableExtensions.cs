using Microsoft.EntityFrameworkCore;
using VideoGameStore.Dtos;

namespace VideoGameStore.Utils
{
    public static class QueryableExtensions
    {
        public static async Task<Page<Dto>> ToPageAsync<Entity, Dto>(this IQueryable<Entity> source, Pageable pageable, 
            Func<Entity, Dto> map, Predicate<Entity>? predicate)
        {
            int total = await source.CountAsync();

            if(predicate != null) 
                source.Where(e => predicate.Invoke(e));

            List<Entity> items = await source
                .Skip((pageable.Page - 1) * pageable.PageSize)
                .Take(pageable.PageSize)
                .ToListAsync();

            List<Dto> result = items.Select(i => map.Invoke(i)).ToList();

            return new Page<Dto>(result, pageable.Page, pageable.PageSize, total, (total + pageable.PageSize - 1) / pageable.PageSize);
        }
    }
}
