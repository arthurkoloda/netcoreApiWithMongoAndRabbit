namespace Service.DTO;

/// <summary>
/// Armazena o conteúdo da planilha, bem como o Id do documento gravado no banco de dados, para envio à fila
/// </summary>
public class PlanilhaFilaDTO
{
    /// <summary>
    /// Id do documento gravado no banco de dados
    /// </summary>
    public string DocumentoId { get; set; }

    /// <summary>
    /// Planilha que foi importada, convertida em um Byte Array
    /// </summary>
    public byte[] Arquivo { get; set; }
}
