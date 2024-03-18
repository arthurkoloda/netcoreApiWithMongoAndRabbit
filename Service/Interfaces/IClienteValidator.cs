using Microsoft.AspNetCore.Http;

namespace Service.Interface.Cliente;

public interface IClienteValidator
{
    /// <summary>
    /// Faz uma série de validações no arquivo que está sendo importado
    /// </summary>
    void ValidarArquivoExcel(IFormFile arquivoExcel);

    /// <summary>
    /// Verifica se a planilha que está sendo importada segue os padrões esperados pelo sistema
    /// </summary>
    void ValidarPlanilha(MemoryStream memoryStream);

    /// <summary>
    /// Valida os dados de um cliente que está contido na planilha em processamento
    /// </summary>
    void ValidarCliente(Database.Models.Cliente cliente);
}
