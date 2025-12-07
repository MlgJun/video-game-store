using VideoGameStore.Dtos;

namespace VideoGameStore.Services
{
    public interface IGameService
    {
        public GameResponse FindById(long gameId);
        public Page<GameResponse> FindAll();
        /// <summary>
        /// Найти все игры продавца
        /// </summary>
        /// <param name="sellerId"></param>
        /// <returns></returns>
        public Page<GameResponse> FindAllBySellerId(long sellerId);
        public GameResponse Create(GameRequest request);
        public GameResponse Update(long gameId, GameRequest request);
        public bool Delete(long gameId);
    }
}
