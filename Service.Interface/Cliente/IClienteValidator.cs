using Microsoft.AspNetCore.Http;

namespace Service.Interface.Cliente;

public interface IClienteValidator
{
    void ValidarArquivoExcel(IFormFile arquivoExcel);

    void ValidarPlanilha(MemoryStream memoryStream);

    void ValidarCliente(Database.Models.Cliente cliente);
}
