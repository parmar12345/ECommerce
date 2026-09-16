using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;

namespace ECommerce.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;

    private const string ProductFolder =
        "uploads/products";

    public LocalFileStorageService(
        IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveAsync(
        Stream stream,
        string fileName,
        string contentType)
    {
        var webRootPath =
            _environment.WebRootPath;

        if (string.IsNullOrWhiteSpace(webRootPath))
        {
            webRootPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot");
        }

        var folderPath = Path.Combine(
            webRootPath,
            ProductFolder);

        Directory.CreateDirectory(folderPath);

        var extension =
            Path.GetExtension(fileName);

        var uniqueFileName =
            $"{Guid.NewGuid()}{extension}";

        var filePath = Path.Combine(
            folderPath,
            uniqueFileName);

        await using var fileStream =
            new FileStream(
                filePath,
                FileMode.Create);

        await stream.CopyToAsync(fileStream);

        return $"/{ProductFolder}/{uniqueFileName}"
            .Replace("\\", "/");
    }

    public Task DeleteAsync(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return Task.CompletedTask;
        }

        var relativePath =
            fileUrl.TrimStart('/')
                .Replace("/", Path.DirectorySeparatorChar.ToString());

        var webRootPath =
            _environment.WebRootPath;

        if (string.IsNullOrWhiteSpace(webRootPath))
        {
            webRootPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot");
        }

        var filePath =
            Path.Combine(webRootPath, relativePath);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}