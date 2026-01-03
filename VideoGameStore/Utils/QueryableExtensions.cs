using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VideoGameStore.Dtos;
using VideoGameStore.Entities;

namespace VideoGameStore.Utils
{
    public static class QueryableExtensions
    {
        public static async Task<Page<Dto>> ToPageAsync<Entity, Dto>(this IQueryable<Entity> source, Pageable pageable, 
            Func<Entity, Dto> map, List<Expression<Func<Entity, bool>>>? predicates) where Entity : BaseEntity
        {
            if(predicates != null)
            {
                foreach (var predicate in predicates)
                {
                    source = source.Where(predicate);
                }
            }

            int totalElements = await source.CountAsync();

            List<Entity> items = await source
                .OrderBy(i => i.Id)
                .Skip((pageable.Page - 1) * pageable.PageSize)
                .Take(pageable.PageSize)
                .ToListAsync();

            List<Dto> result = items.Select(i => map.Invoke(i)).ToList();

            return new Page<Dto>(result, pageable.Page, pageable.PageSize, totalElements, (totalElements + pageable.PageSize - 1) / pageable.PageSize);
        }
    }
}
