using OfficeOpenXml;

namespace Service.Interface.Fila;

public interface IClienteRabbitMQ
{
    void ArmazenarEmFila(MemoryStream planilha, string documentoId);

    ExcelPackage? LerDocumentoFila();

    int ConsultarQuantidadeDocumentosEmFila();
}
