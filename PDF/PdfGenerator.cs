using QuestPDF.Fluent;
using QuestPDF.Helpers;
using MinhaApiCQRS.Application.Interfaces;


namespace MinhaApiCQRS.PDF;


/// Servico responsavel por gerar o relatorio PDF do funcionario.
/// Ele transforma nome, idade e foto em um arquivo PDF representado por byte[].

public class PdfGenerator : IPdfService
{
    public byte[] GenerateEmployeePdf(string name, int age, string? photo)
    {
        // Cria o documento PDF em memoria usando QuestPDF.
        return Document.Create(container =>
        {
            // Configura uma pagina com margem, cabecalho e conteudo.
            container.Page(page =>
            {
                page.Margin(50);

                // Cabecalho do relatorio.
                page.Header().Text("Relatorio de Funcionario").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                // Corpo do PDF com as informacoes solicitadas na atividade.
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Spacing(5);
                    col.Item().Text($"Nome: {name}");
                    col.Item().Text($"Idade: {age} anos");

                    // A foto entra no PDF apenas quando o arquivo salvo no servidor ainda existe.
                    if (!string.IsNullOrEmpty(photo) && File.Exists(photo))
                    {
                        // Insere a imagem no documento respeitando o limite de tamanho.
                        col.Item().PaddingTop(10).MaxWidth(200).MaxHeight(200).Image(photo).FitArea();
                    }
                });
            });
        }).GeneratePdf();
    }




}
