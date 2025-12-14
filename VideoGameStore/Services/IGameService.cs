using VideoGameStore.Controllers;
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
        public Task<Page<SellerGameResponse>> FindAllBySellerId(long sellerId, Pageable pageable);
        public Task<GameResponse> Create(GameWithKeysRequest request);
        public Task<GameResponse> AddKeys(long gameId, IFormFile keys);
        public Task<GameResponse> Update(long gameId, long sellerId, GameRequest request);
        public Task<bool> Delete(long gameId, long sellerId);
    }
}
