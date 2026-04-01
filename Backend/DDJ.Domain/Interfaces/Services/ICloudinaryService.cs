namespace DDJ.Domain.Interfaces.Services;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(Stream fileStream, string fileName, string folder, CancellationToken ct = default);
    Task<bool> DeleteFileAsync(string publicId, CancellationToken ct = default);
}
