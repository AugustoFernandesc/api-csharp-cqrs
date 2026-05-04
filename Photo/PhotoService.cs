using Microsoft.AspNetCore.Http;

namespace Photo.Services;


/// Servico responsavel por salvar fotos no disco e recuperar fotos como bytes.
/// Ele grava o arquivo na pasta Storage e devolve o caminho fisico para ser salvo no banco.

public class PhotoService : IPhotoService
{
    // Define onde as fotos serao salvas: pasta Storage na raiz do processo da API.
    private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");

    public async Task<string?> UploadAsync(IFormFile file)
    {
        // Se nenhum arquivo foi enviado, nao existe caminho para salvar no banco.
        if (file == null || file.Length == 0) return null;

        // Cria a pasta Storage quando ela ainda nao existe.
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }

        // Gera um nome unico para nao sobrescrever fotos com o mesmo nome.
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(_storagePath, fileName);

        // Copia o conteudo recebido pelo formulario para o arquivo fisico.
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Caminho completo que sera salvo no banco e usado depois para PDF/foto.
        return filePath;
    }

    public byte[] GetImageBytes(string filePath)
    {
        // Garante que a foto existe antes de tentar ler o arquivo.
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            throw new FileNotFoundException("A foto não foi encontrada no servidor.");
        }

        // Transforma o arquivo de imagem em bytes para o controller retornar como File(...).
        return File.ReadAllBytes(filePath);
    }
}
