using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Service.Interface.Documento;

public interface IClienteService
{
    /// <summary>
    /// Recebe a planilha importada e prossegue para as etapas subsequentes do armazenamento de informações
    /// </summary>
    Task ProcessarExcel(IFormFile arquivoExcel);

    /// <summary>
    /// Lê o próximo item da fila de documentos, processa e armazena as informações, caso houver
    /// </summary>
    Task ProcessarFila();

    /// <summary>
    /// Lista todos os documentos que foram importados, independente da situação
    /// </summary>
    Task<IList<DocumentoResponse>?> ListarDocumentos();

    /// <summary>
    /// Lista somente os documentos que já foram processados pela fila
    /// </summary>
    Task<IList<DocumentoResponse>?> ListarDocumentosProcessados();

    /// <summary>
    /// Lista somente os documentos que estão aguardando processamento ou estão em fase de processando na fila
    /// </summary>
    /// <returns></returns>
    Task<IList<DocumentoResponse>?> ListarDocumentosPendentes();

    /// <summary>
    /// Lista os documentos processados em uma data específica
    /// </summary>
    Task<IList<Database.Models.Documento>?> ListarDocumentosProcessadosPorData(DateTime data);

    /// <summary>
    /// Lista todos os clientes que foram importados através de um documento específico
    /// </summary>
    Task<IList<ClienteResponse>?> ListarClientesDocumento(string documentoId);
}
