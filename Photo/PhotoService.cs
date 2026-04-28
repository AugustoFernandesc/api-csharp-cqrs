using Microsoft.AspNetCore.Http;

namespace Photo.Services;

public class PhotoService : IPhotoService
{
    // Define onde as fotos serão salvas (Pasta Storage na raiz da API)
    private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");

    public async Task<string> UploadAsync(IFormFile file)
    {
        if (file == null || file.Length == 0) return null!;

        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }

        // Gera um nome único para não sobrescrever fotos com o mesmo nome
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(_storagePath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return filePath; // Caminho completo que irá para o banco
    }

    public byte[] GetImageBytes(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            throw new FileNotFoundException("A foto não foi encontrada no servidor.");
        }

        return File.ReadAllBytes(filePath);
    }
}