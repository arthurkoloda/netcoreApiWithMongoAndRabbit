namespace Domain.Models;

public class DocumentoResponse
{
    public string Nome { get; set; } = string.Empty;

    public DateTime? DataImportacao { get; set; }

    public string Situacao { get; set; } = string.Empty;
}
