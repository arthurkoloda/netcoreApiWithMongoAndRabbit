using Database.Models;

namespace Database.Interface;

/// <summary>
/// Responsável por gerenciar as informações dos documentos (planilhas) no banco de dados
/// </summary>
public interface IDocumentoRepository
{
    /// <summary>
    /// Insere um novo documento no banco de dados
    /// </summary>
    Task InsertAsync(Documento documento);

    /// <summary>
    /// Busca um documento através do seu Id
    /// </summary>
    Task<Documento?> GetAsync(string id);

    /// <summary>
    /// Atualiza um documento no banco de dados, buscando-o pelo seu Id
    /// </summary>
    Task UpdateAsync(string id, Documento documento);

    /// <summary>
    /// Lista todos os documentos disponíveis no banco de dados
    /// </summary>
    Task<IList<Documento>?> ListAllAsync();

    /// <summary>
    /// Lista os documentos que estão pendentes de processamento ou que estão sendo processados no momento
    /// </summary>
    Task<IList<Documento>?> ListPending();

    /// <summary>
    /// Lista os documentos que já foram processados pelo sistema, independente do resultado do processamento
    /// </summary>
    Task<IList<Documento>?> ListProcessed();

    /// <summary>
    /// Lista os documentos que foram processados durante a data repassada
    /// </summary>
    Task<IList<Documento>?> ListProcessedDaily(DateTime dataProcessamento);
}
