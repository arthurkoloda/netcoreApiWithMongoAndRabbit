using RabbitMQ.Client;
using Service.Interface.Fila;
using Newtonsoft.Json;
using System.Text;
using Service.DTO;

namespace Service.Fila;

internal class ClienteRabbitMQ : IClienteRabbitMQ
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string nomeFilaDocumentos = "importacao-documentos-queue";

    public ClienteRabbitMQ()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost"
        };

        _connection = factory.CreateConnection();

        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: nomeFilaDocumentos, exclusive: false);
    }

    public void ArmazenarEmFila(MemoryStream planilha, string documentoId)
    {
        var dadosFila = new PlanilhaFilaDTO()
        {
            DocumentoId = documentoId,
            Arquivo = planilha.ToArray()
        };

        var dadosFilaJSON = JsonConvert.SerializeObject(dadosFila);

        _channel.BasicPublish(string.Empty, nomeFilaDocumentos, null, Encoding.UTF8.GetBytes(dadosFilaJSON));
    }

    public PlanilhaFilaDTO? LerDocumentoFila()
    {
        BasicGetResult? data = _channel.BasicGet(nomeFilaDocumentos, true);

        if(data != null)
        {
            var dadosFilaJSON = Encoding.UTF8.GetString(data.Body.ToArray());

            var dadosFila = JsonConvert.DeserializeObject<PlanilhaFilaDTO>(dadosFilaJSON);

            if(dadosFila != null)
            {
                return dadosFila;
            }
        }

        return null;
    }
}
