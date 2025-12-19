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

        public GameResponse ToResponse(Game game, string imageUrl)
        {
            return new GameResponse(game.Id, game.PublisherTitle, game.DeveloperTitle, game.Price, game.Title, game.Description, 
                DateTime.Now, _genreMapper.ToResponseList(game.Genres), game.Keys.Count, imageUrl);
        }

        public SellerGameResponse ToResponseForSeller(Game game, string imageUrl)
        {
            return new SellerGameResponse(game.Id, game.Title, game.Price, game.Keys.Select(k => k.Value).ToList(), imageUrl);
        }

        public Game ToEntity(GameWithKeysRequest gameRequest, Seller seller, string imageUrl)
        {
            var game = new Game();

            game.PublisherTitle = gameRequest.PublisherTitle;
            game.DeveloperTitle = gameRequest.DeveloperTitle;
            game.Price = gameRequest.Price;
            game.Title = gameRequest.Title;
            game.Description = gameRequest.Description;
            game.Genres = _genreMapper.ToEntityList(gameRequest.Genres);
            game.Seller = seller;
            game.ImageUrl = imageUrl;

            return game;
        }

        public List<GameResponse> ToResponseList(Dictionary<Game, string> games)
        {
            return games.Select(game => ToResponse(game.Key, game.Value)).ToList();
        }

        public Game Update(GameRequest request, Game game, string imageUrl)
        {
            game.Title = request.Title;
            game.Description = request.Description;
            game.Price = request.Price;
            game.DeveloperTitle = request.DeveloperTitle;
            game.PublisherTitle = request.PublisherTitle;
            game.Genres = _genreMapper.ToEntityList(request.Genres);
            game.ImageUrl= imageUrl;

            return game;
        }
    }
}
