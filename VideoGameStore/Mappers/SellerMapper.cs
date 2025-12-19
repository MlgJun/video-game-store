using VideoGameStore.Dtos;
using VideoGameStore.Entities;

namespace VideoGameStore.Mappers
{
    public class SellerMapper
    {
        private readonly GameMapper _gameMapper;

        public SellerMapper(GameMapper gameMapper)
        {
            _gameMapper = gameMapper;
        }

        public SellerResponse ToResponse(Seller seller, AspNetUser user, Dictionary<Game, string> games)
        {
            return new SellerResponse(seller.Id, user.UserName, user.Email, _gameMapper.ToResponseList(games));
        }

        public Seller ToEntity(UserRequest sellerRequest)
        {
            var seller = new Seller
            {
                CreatedAt = DateTime.Now,
            };

            return seller;
        }
    }
}
