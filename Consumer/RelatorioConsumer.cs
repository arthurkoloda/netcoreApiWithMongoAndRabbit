using Domain.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Consumer
{
    /// <summary>
    /// Classe responsável enviar um relatório com os documentos diariamente às 18h
    /// </summary>
    internal class RelatorioConsumer : IHostedService, IDisposable
    {
        private readonly IRelatorioService _relatorioService;
        private Timer _timer;

        public RelatorioConsumer(IRelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        // Método para calcular o tempo restante até às 18h
        private TimeSpan TempoAteAs18h()
        {
            DateTime agora = DateTime.Now;
            DateTime amanha18h = agora.Date.AddDays(1).AddHours(18);
            return amanha18h - agora;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            TimeSpan aguardoInicial = TempoAteAs18h();

            _timer = new Timer(async state =>
            {
                try
                {
                    await _relatorioService.GerarRelatorioDiario();

                    // Reiniciar o temporizador para o próximo dia
                    _timer.Change(TimeSpan.FromDays(1), Timeout.InfiniteTimeSpan);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao gerar o relatório: {ex.Message}");
                }
            }, null, aguardoInicial, Timeout.InfiniteTimeSpan);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
