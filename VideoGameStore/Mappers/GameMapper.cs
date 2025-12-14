using VideoGameStore.Controllers;
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
                                    game.Title, game.Description, DateTime.Now, _genreMapper.ToResponseList(game.Genres), game.Keys.Count);
        }

        public SellerGameResponse ToResponseForSeller(Game game)
        {
            return new SellerGameResponse(game.Id, game.Title, game.Price, game.Keys.Select(k => k.Value).ToList());
        }

        public Game ToEntity(GameWithKeysRequest gameRequest)
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

        public Game Update(GameRequest request, Game game)
        {
            game.Title = request.Title;
            game.Description = request.Description;
            game.Price = request.Price;
            game.DeveloperTitle = request.DeveloperTitle;
            game.PublisherTitle = request.PublisherTitle;
            game.Genres = _genreMapper.ToEntityList(request.Genres);

            return game;
        }
    }
}
