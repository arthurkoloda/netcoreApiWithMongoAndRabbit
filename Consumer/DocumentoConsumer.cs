using Microsoft.Extensions.Hosting;
using Service.Interface.Documento;

namespace Consumer;

/// <summary>
/// Classe responsável por processar a fila de documentos de tempos em tempos
/// </summary>
internal class DocumentoConsumer : IHostedService
{
    //Segundos entre um processamento e outro da fila
    private readonly int segundosParaReprocessar = 30;

    private readonly IClienteService _clienteService;

    public DocumentoConsumer(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    //Retorna data e hora atual para utilização nos Console.WriteLine
    private string GetDataHoraString()
    {
        return $"[{DateTime.UtcNow.ToLocalTime().ToShortDateString()} - {DateTime.UtcNow.ToLocalTime().ToLongTimeString()}] - ";
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while(!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine($"{GetDataHoraString()}Iniciando processamento da fila de documentos.");

            try
            {
                await _clienteService.ProcessarFila();

                Console.WriteLine($"{GetDataHoraString()}Fila processada com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{GetDataHoraString}Ocorreu um erro ao processar a fila: {ex.Message}.");
            }

            await Task.Delay(segundosParaReprocessar * 1000);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
