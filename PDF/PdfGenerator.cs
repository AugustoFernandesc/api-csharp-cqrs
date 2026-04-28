using QuestPDF.Fluent;
using QuestPDF.Helpers;
using MinhaApiCQRS.Application.Interfaces;


namespace MinhaApiCQRS.PDF;

public class PdfGenerator : IPdfService
{
    public byte[] GenerateEmployeePdf(string name, int age, string? photo)
    {

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Header().Text("Relatorio de Funcionario").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Spacing(5);
                    col.Item().Text($"Nome: {name}");
                    col.Item().Text($"Idade: {age} anos");

                    if (!string.IsNullOrEmpty(photo) && File.Exists(photo))
                    {
                        col.Item().PaddingTop(10).Image(photo);
                    }
                });
            });
        }).GeneratePdf();
    }




}
