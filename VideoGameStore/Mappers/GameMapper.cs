using VideoGameStore.Dtos;
using VideoGameStore.Entities;

namespace VideoGameStore.Mappers
{
    public class GameMapper
    {
        private readonly GenreMapper _genreMapper;

        public GameMapper(GenreMapper genreMapper)
        {
            _genreMapper = genreMapper;
        }

        public GameResponse ToResponse(Game game)
        {
            return new GameResponse(game.Id, game.PublisherTitle, game.DeveloperTitle, game.Price,
                                    game.Title, game.Description, DateTime.Now, _genreMapper.ToResponseList(game.Genres));
        }

        public Game ToEntity(GameRequest gameRequest)
        {
            var game = new Game();

            game.PublisherTitle = gameRequest.PublisherTitle;
            game.DeveloperTitle = gameRequest.DeveloperTitle;
            game.Price = gameRequest.Price;
            game.Title = gameRequest.Title;
            game.Description = gameRequest.Description;
            game.Genres = _genreMapper.ToEntityList(gameRequest.Genres);

            return game;
        }
        public List<GameResponse> ToResponseList(List<Game> games)
        {
            return games.Select(game => ToResponse(game)).ToList();
        }

    }
}
