using VideoGameStore.Dtos;
using VideoGameStore.Entities;

namespace VideoGameStore.Mappers
{
    public class GameMapper
    {
        public GameResponse ToResponse(Game game)
        {
            return new GameResponse(game.Id, game.PublisherTitle, game.DeveloperTitle, game.Price, game.Title ,game.Description);
        }
        
        public Game ToEntity(GameRequest gameRequest)
        {
            var game = new Game();

            game.PublisherTitle = gameRequest.PublisherTitle;
            game.DeveloperTitle = gameRequest.DeveloperTitle;
            game.Price = gameRequest.Price;
            game.Title = gameRequest.Title;
            game.Description = gameRequest.Description;

            return game;
        }
        public List<GameResponse> ToResponseList(List<Game> games)
        {
            List<GameResponse> orederItems = [];

            foreach (var i in games)
            {
                orederItems.Add(ToResponse(i));
            }

            return orederItems;
        }

    }
}
