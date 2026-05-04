using Microsoft.AspNetCore.Http;

namespace Photo.Services;

public interface IPhotoService
{
    // Retorna o caminho onde a foto foi salva
    Task<string?> UploadAsync(IFormFile file);

    // Retorna os bytes da foto para exibir na API
    byte[] GetImageBytes(string filePath);
}