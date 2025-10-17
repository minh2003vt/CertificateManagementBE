using Application.IServices;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                throw new InvalidOperationException("Cloudinary configuration is missing.");
            }

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
        {
            try
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, imageStream),
                    Folder = "certificate_management/certificates",
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = false
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception($"Cloudinary upload error: {uploadResult.Error.Message}");
                }

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to upload image to Cloudinary: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteImageAsync(string publicIdOrUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(publicIdOrUrl))
                {
                    return true; // nothing to delete
                }

                // Accept either a Cloudinary public id or a full URL; if URL, derive public id by stripping version and extension
                string publicId = publicIdOrUrl;
                if (publicIdOrUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || publicIdOrUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    // URL format: https://res.cloudinary.com/<cloud>/image/upload/v<version>/<folder>/<name>.<ext>
                    // Extract part after '/upload/'
                    var marker = "/upload/";
                    var idx = publicIdOrUrl.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                    if (idx >= 0)
                    {
                        var after = publicIdOrUrl.Substring(idx + marker.Length);
                        // remove leading version segment like v1691234567/
                        if (after.StartsWith("v"))
                        {
                            var slashIdx = after.IndexOf('/');
                            if (slashIdx > 0)
                            {
                                after = after.Substring(slashIdx + 1);
                            }
                        }
                        // drop extension (last dot segment)
                        var lastDot = after.LastIndexOf('.');
                        publicId = lastDot > 0 ? after.Substring(0, lastDot) : after;
                    }
                }

                var deletionParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Image
                };
                var result = await _cloudinary.DestroyAsync(deletionParams);
                return string.Equals(result.Result, "ok", StringComparison.OrdinalIgnoreCase) || string.Equals(result.Result, "not found", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                // swallow and report failure; callers decide how to proceed
                return false;
            }
        }
    }
}

