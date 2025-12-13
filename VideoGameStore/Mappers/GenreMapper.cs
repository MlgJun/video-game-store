using VideoGameStore.Dtos;
using VideoGameStore.Entities;

namespace VideoGameStore.Mappers
{
    public class GenreMapper
    {
        public GenreResponse ToResponse(Genre genre)
        {
            return new GenreResponse(genre.Title);
        }

        public Genre ToEntity(GenreRequest genre)
        {
            return new Genre { Title = genre.Title };
        }

        public List<GenreResponse> ToResponseList(List<Genre> genres)
        {
            return genres.Select(g => ToResponse(g)).ToList();
        }

        public List<Genre> ToEntityList(List<GenreRequest> genres)
        {
            return genres.Select(g => ToEntity(g)).ToList();
        }
    }
}
