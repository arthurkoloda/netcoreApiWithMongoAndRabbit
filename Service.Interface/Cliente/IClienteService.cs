using Microsoft.AspNetCore.Http;

namespace Service.Interface.Documento;

public interface IClienteService
{
    Task ProcessarExcel(IFormFile arquivoExcel);

    Task ProcessarFila();
}
