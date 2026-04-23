using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PortfolioApi.Infrastructure.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder = "portfolio");
        Task<string> UploadCVAsync(IFormFile file);
        Task<bool> DeleteFileAsync(string publicId);
        string GetImageUrl(string publicId);
    }

    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(IConfiguration configuration, ILogger<CloudinaryService> logger)
        {
            _logger = logger;
            
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                _logger.LogWarning("Cloudinary credentials not configured. File uploads will fail.");
                _cloudinary = null;
                return;
            }

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder = "portfolio")
        {
            if (_cloudinary == null)
            {
                _logger.LogError("Cloudinary not configured");
                throw new InvalidOperationException("Cloudinary service not configured");
            }

            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty");
            }

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                throw new InvalidOperationException("Only image files (JPG, PNG, GIF, WebP) are allowed");
            }

            // Validate file size (5MB max)
            if (file.Length > 5 * 1024 * 1024)
            {
                throw new InvalidOperationException("File size exceeds 5MB limit");
            }

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                Transformation = new Transformation()
                    .Quality("auto")
                    .FetchFormat("auto"),
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogError("Cloudinary upload failed: {Error}", uploadResult.Error?.Message);
                throw new InvalidOperationException($"Upload failed: {uploadResult.Error?.Message}");
            }

            _logger.LogInformation("Image uploaded successfully: {PublicId}", uploadResult.PublicId);
            return uploadResult.SecureUrl?.ToString() ?? uploadResult.Url?.ToString();
        }

        public async Task<string> UploadCVAsync(IFormFile file)
        {
            if (_cloudinary == null)
            {
                _logger.LogError("Cloudinary not configured");
                throw new InvalidOperationException("Cloudinary service not configured");
            }

            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty");
            }

            // Validate file type
            if (file.ContentType.ToLower() != "application/pdf" && 
                !file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Only PDF files are allowed for CV upload");
            }

            // Validate file size (10MB max)
            if (file.Length > 10 * 1024 * 1024)
            {
                throw new InvalidOperationException("File size exceeds 10MB limit");
            }

            using var stream = file.OpenReadStream();
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "cv",
                PublicId = $"Abdullah_Mohammed_CV_{DateTime.UtcNow:yyyyMMdd}",
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogError("Cloudinary CV upload failed: {Error}", uploadResult.Error?.Message);
                throw new InvalidOperationException($"Upload failed: {uploadResult.Error?.Message}");
            }

            _logger.LogInformation("CV uploaded successfully: {PublicId}", uploadResult.PublicId);
            return uploadResult.SecureUrl?.ToString() ?? uploadResult.Url?.ToString();
        }

        public async Task<bool> DeleteFileAsync(string publicId)
        {
            if (_cloudinary == null)
            {
                _logger.LogError("Cloudinary not configured");
                return false;
            }

            if (string.IsNullOrEmpty(publicId))
            {
                return false;
            }

            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);

            var success = result.Result == "ok";
            if (success)
            {
                _logger.LogInformation("File deleted successfully: {PublicId}", publicId);
            }
            else
            {
                _logger.LogWarning("Failed to delete file: {PublicId}, Result: {Result}", publicId, result.Result);
            }

            return success;
        }

        public string GetImageUrl(string publicId)
        {
            if (_cloudinary == null || string.IsNullOrEmpty(publicId))
            {
                return null;
            }

            return _cloudinary.Api.UrlImgUp.BuildUrl(publicId);
        }
    }
}
