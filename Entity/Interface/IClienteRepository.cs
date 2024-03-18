using Database.Models;

namespace Database.Interface;

/// <summary>
/// Responsável por gerenciar as informações dos clientes no banco de dados
/// </summary>
public interface IClienteRepository
{
    /// <summary>
    /// Insere um novo cliente no banco de dados
    /// </summary>
    Task InsertAsync(Cliente entity);

    /// <summary>
    /// Lista os cliente que foram importados a partir de um documento em específico
    /// </summary>
    Task<IList<Cliente>?> ListByDocumentoId(string documentoId);
}
