namespace PubQuizBackend.Util.Helpers
{
    public static class ImageHelper
    {
        public static async Task<string> SaveProfileImageAsync(IFormFile image, string folderPath, string previousFileName)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("Invalid image file.");

            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Unsupported file type.");

            Directory.CreateDirectory(folderPath);

            if (!string.IsNullOrWhiteSpace(previousFileName) && previousFileName != "default.jpg")
            {
                var oldFilePath = Path.Combine(folderPath, previousFileName);
                if (File.Exists(oldFilePath))
                    File.Delete(oldFilePath);
            }

            var newFileName = Guid.NewGuid() + extension;
            var newFilePath = Path.Combine(folderPath, newFileName);

            using var stream = new FileStream(newFilePath, FileMode.Create);
            await image.CopyToAsync(stream);

            return newFileName;
        }
    }
}
