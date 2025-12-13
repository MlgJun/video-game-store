using Microsoft.EntityFrameworkCore;
using VideoGameStore.Context;
using VideoGameStore.Dtos;
using VideoGameStore.Mappers;

namespace VideoGameStore.Services
{
    public class GenreService : IGenreService
    {
        private readonly AppDbContext _context;
        private GenreMapper _mapper;

        public GenreService(AppDbContext context, GenreMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<GenreResponse>> FindAll()
        {
            return _mapper.ToResponseList(await _context.Genres.ToListAsync());
        }
    }
}
