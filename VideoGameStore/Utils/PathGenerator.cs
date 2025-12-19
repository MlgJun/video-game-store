using Microsoft.IdentityModel.Tokens;

namespace VideoGameStore.Utils
{
    public static class PathGenerator
    {
        public static string GenerateFilePath(IFormFile file)
        {
            string extension = GetFileExtension(file.FileName);

            return $"games/{Guid.NewGuid()}{extension}";
        }

        private static string GetFileExtension(string filename)
        {
            if (filename == null || filename.IsNullOrEmpty())
            {
                return "";
            }

            int lastDotIndex = filename.LastIndexOf('.');
            return lastDotIndex > 0 ? filename.Substring(lastDotIndex) : "";
        }

    }
}
