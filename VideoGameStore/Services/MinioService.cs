using Minio;
using Minio.DataModel.Args;

namespace VideoGameStore.Services
{
    public class MinioService : IFileStorage
    {
        private readonly IMinioClient _minio;
        private const string BUCKET_NAME = "games-files";

        public MinioService(IMinioClient minio)
        {
            _minio = minio;
        }

        public async Task DeletePhotoAsync(string objectName, CancellationToken ct = default)
        {
            var removeArgs = new RemoveObjectArgs()
            .WithBucket(BUCKET_NAME)
            .WithObject(objectName);

            await _minio.RemoveObjectAsync(removeArgs, ct);
        }

        public async Task UploadPhotoAsync(string objectName, IFormFile image, CancellationToken ct = default)
        {
            await EnsureBucketExistsAsync(ct);

            var putArgs = new PutObjectArgs()
                .WithBucket(BUCKET_NAME)
                .WithObject(objectName)
                .WithStreamData(image.OpenReadStream())
                .WithObjectSize(image.Length)
                .WithContentType(image.ContentType);

            await _minio.PutObjectAsync(putArgs, ct);
        }

        public string Url(string objectName)
        {
            return $"http://localhost:9000/{BUCKET_NAME}/{objectName}";
        }

        private async Task EnsureBucketExistsAsync(CancellationToken ct = default)
        {
            var existsArgs = new BucketExistsArgs()
                .WithBucket(BUCKET_NAME);

            var exists = await _minio.BucketExistsAsync(existsArgs, ct);

            if (!exists)
            {
                var makeArgs = new MakeBucketArgs()
                    .WithBucket(BUCKET_NAME);

                await _minio.MakeBucketAsync(makeArgs, ct);
                string policy = $@"{{
                    ""Version"": ""2012-10-17"",
                    ""Statement"": [
                        {{
                            ""Effect"": ""Allow"",
                            ""Principal"": {{""AWS"": ""*""}},
                            ""Action"": [""s3:GetObject""],
                            ""Resource"": ""arn:aws:s3:::{BUCKET_NAME}/*""
                        }}
                    ]
                }}";

                await _minio.SetPolicyAsync(new SetPolicyArgs()
                    .WithBucket(BUCKET_NAME)
                    .WithPolicy(policy));
            }
        }
    }
}
