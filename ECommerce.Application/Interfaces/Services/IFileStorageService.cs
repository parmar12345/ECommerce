namespace ECommerce.Application.Interfaces.Services;

public interface IFileStorageService
{
    Task<string> SaveAsync(
        Stream stream,
        string fileName,
        string contentType);

    Task DeleteAsync(string fileUrl);
}