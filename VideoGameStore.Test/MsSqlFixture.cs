using DotNet.Testcontainers.Builders;
using Minio;
using Minio.DataModel.Args;
using Testcontainers.Minio;
using Testcontainers.MsSql;

namespace VideoGameStore.Tests
{
    public class MsSqlFixture : IAsyncLifetime
    {
        public MsSqlContainer MsSqlContainer { get; private set; } = null!;
        public MinioContainer MinioContainer { get; private set; } = null!;
        public IMinioClient MinioClient { get; private set; } = null!;
        public string ConnectionString { get; private set; } = null!;
        private const string BUCKET_NAME = "game-files-test";


        public async Task InitializeAsync()
        {
            MsSqlContainer = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("Password1234@!")
                .WithCleanUp(true)
                .WithReuse(true)
                .Build();

            MinioContainer = new MinioBuilder()
                .WithImage("minio/minio:RELEASE.2023-01-31T02-24-19Z")
                .WithPortBinding(9000)
                .WithUsername("minioadmin")
                .WithPassword("minioadmin")
                .WithCleanUp(true)
                .WithReuse(true)
                .Build();

            await MinioContainer.StartAsync();

            await MsSqlContainer.StartAsync();

            ConnectionString = MsSqlContainer.GetConnectionString();

            MinioClient = new MinioClient()
                .WithEndpoint("localhost", MinioContainer.GetMappedPublicPort(9000))
                .WithCredentials("minioadmin", "minioadmin")
                .Build();

            bool bucketExists = await MinioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(BUCKET_NAME));

            if (!bucketExists)
            {
                await MinioClient.MakeBucketAsync(new MakeBucketArgs()
                    .WithBucket(BUCKET_NAME));
            }

            await WaitForMinioReady();
        }

        public async Task DisposeAsync()
        {
            await Task.CompletedTask;
        }

        //public async Task DisposeAsync()
        //{
        //    if (MsSqlContainer != null)
        //        await MsSqlContainer.DisposeAsync();
        //    if (MinioContainer != null)
        //        await MinioContainer.DisposeAsync();
        //}

        public string GetMinioEndpoint() => $"http://localhost:{MinioContainer.GetMappedPublicPort(9000)}";

        public MinioConfig GetMinioConfig() => new()
        {
            Endpoint = GetMinioEndpoint(),
            AccessKey = "minioadmin",
            SecretKey = "minioadmin",
            BucketName = BUCKET_NAME
        };

        private async Task WaitForMinioReady()
        {
            var attempts = 0;
            while (attempts < 30)
            {
                try
                {
                    await MinioClient.BucketExistsAsync(new BucketExistsArgs()
                        .WithBucket(BUCKET_NAME));
                    return;
                }
                catch
                {
                    await Task.Delay(1000);
                    attempts++;
                }
            }
        }

        public record MinioConfig
        {
            public string Endpoint { get; init; } = null!;
            public string AccessKey { get; init; } = null!;
            public string SecretKey { get; init; } = null!;
            public string BucketName { get; init; } = null!;
        }
    }
}
