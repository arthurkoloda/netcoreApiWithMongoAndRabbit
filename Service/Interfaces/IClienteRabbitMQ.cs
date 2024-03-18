using Service.DTO;

namespace Service.Interface.Fila;

public interface IClienteRabbitMQ
{
    /// <summary>
    /// Recebe a planilha importada e o Id do documento armazenado no banco de dados e envia para a fila correspondente
    /// </summary>
    void ArmazenarEmFila(MemoryStream planilha, string documentoId);

    /// <summary>
    /// Lê o primeiro documento disposto na fila e o devolve, caso houver
    /// </summary>
    PlanilhaFilaDTO? LerDocumentoFila();
}
