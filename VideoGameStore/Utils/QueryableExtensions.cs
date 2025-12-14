using Microsoft.EntityFrameworkCore;
using VideoGameStore.Dtos;
using VideoGameStore.Entities;

namespace VideoGameStore.Utils
{
    public static class QueryableExtensions
    {
        public static async Task<Page<Dto>> ToPageAsync<Entity, Dto>(this IQueryable<Entity> source, Pageable pageable, 
            Func<Entity, Dto> map, List<Predicate<Entity>>? predicates) where Entity : BaseEntity
        {
            int total = await source.CountAsync();

            if(predicates != null) 
                foreach(var p in predicates)
                    source.Where(e => p.Invoke(e));

            List<Entity> items = await source
                .OrderBy(i => i.Id)
                .Skip((pageable.Page - 1) * pageable.PageSize)
                .Take(pageable.PageSize)
                .ToListAsync();

            List<Dto> result = items.Select(i => map.Invoke(i)).ToList();

            return new Page<Dto>(result, pageable.Page, pageable.PageSize, total, (total + pageable.PageSize - 1) / pageable.PageSize);
        }
    }
}
