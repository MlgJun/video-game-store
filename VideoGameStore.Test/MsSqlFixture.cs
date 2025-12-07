using Testcontainers.MsSql;
using Xunit;

namespace VideoGameStore.Tests
{
    public class MsSqlFixture : IAsyncLifetime
    {
        public MsSqlContainer MsSqlContainer { get; private set; } = null!;
        public string ConnectionString { get => MsSqlContainer.GetConnectionString();}

        public async Task InitializeAsync()
        {
            MsSqlContainer = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("Password1234@!")
                .WithCleanUp(true)
                .Build();

            await MsSqlContainer.StartAsync();

            await MsSqlContainer.ExecAsync(new List<string>
            {
                "/opt/mssql-tools18/bin/sqlcmd",
                "-S", "localhost",
                "-U", "sa",
                "-P", "Password1234@!",
                "-Q", "CREATE DATABASE GameStoreTest"
            });
        }

        public async Task DisposeAsync()
        {
            await MsSqlContainer.DisposeAsync().AsTask();
        }
    }
}
