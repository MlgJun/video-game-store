using VideoGameStore.Tests;

namespace VideoGameStore.Test
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<MsSqlFixture>
    {
    }
}
