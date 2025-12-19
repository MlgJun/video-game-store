namespace VideoGameStore.Services
{
    public interface IFileStorage
    {
        public Task UploadPhotoAsync(string objectName, IFormFile image, CancellationToken ct = default);
        public Task DeletePhotoAsync(string objectName, CancellationToken ct = default);
        public string Url(string objectName);
    }
}
