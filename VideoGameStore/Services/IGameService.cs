using VideoGameStore.Dtos;

namespace VideoGameStore.Services
{
    public interface IGameService
    {
        public Task<GameResponse> FindById(long gameId);
        public Task<Page<GameResponse>> FindAll(Pageable pageable);
        public Task<Page<GameResponse>> FindAllByFilter(Pageable pageable, FilterRequest filter);
        /// <summary>
        /// Найти все игры продавца
        /// </summary>
        /// <param name="sellerId"></param>
        /// <returns></returns>
        public Task<Page<GameResponse>> FindAllBySellerId(long sellerId, Pageable pageable);
        public Task<GameResponse> Create(GameRequest request);
        public Task<GameResponse> Update(long gameId, GameRequest request);
        public Task<bool> Delete(long gameId);
    }
}
