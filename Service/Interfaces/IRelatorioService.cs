namespace Domain.Interfaces;

public interface IRelatorioService
{
    /// <summary>
    /// Gera o relatório diário de documentos processados e envia via e-mail
    /// </summary>
    Task GerarRelatorioDiario();
}
