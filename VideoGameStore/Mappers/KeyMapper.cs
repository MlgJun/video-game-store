using VideoGameStore.Entities;

namespace VideoGameStore.Mappers
{
    public class KeyMapper
    {
        public List<Key> ToEntityList(string[] keys, Game game)
        {
            List<Key> keysList = keys.Select(k => new Key { Value = k, Game = game, GameId = game.Id }).ToList();

            game.Keys.AddRange(keysList);

            return keysList;
        }
    }
}
