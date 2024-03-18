using Domain.Interfaces;
using Domain.Models;
using OfficeOpenXml;
using Service.Interface.Documento;
using System.Net;
using System.Net.Mail;

namespace Domain.Services;

public class RelatorioService : IRelatorioService
{
    private readonly string[] _destinatarios = ["arthur.koloda@hotmail.com"];
    private readonly IClienteService _clienteService;
    private readonly SmtpClient _smtpClient;
    private readonly string _smtpUsername;

    public RelatorioService(IClienteService clienteService)
    {
        _clienteService = clienteService;
        _smtpUsername = "fakeUserName@mail.com";

        _smtpClient = new SmtpClient("mail.fakesmtp.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(_smtpUsername, "fakePassword"),
            EnableSsl = true
        };
    }

    public async Task GerarRelatorioDiario()
    {
        string documentosProcessados = "";

        var documentos = await _clienteService.ListarDocumentosProcessadosPorData(DateTime.Today);

        var clientes = new List<ClienteResponse>();

        if(documentos != null && documentos.Any())
        {
            foreach(var documento in documentos)
            {
                documentosProcessados += $"Documento: {documento.Nome} -- Data de Processamento: {documento.DataProcessamento!.Value.ToShortDateString()} -- Situação: {documento.Situacao} <br>";

                var clientesDb = await _clienteService.ListarClientesDocumento(documento.Id);

                if(clientesDb != null && clientesDb.Any())
                {
                    clientes = clientes.Union(clientesDb).ToList();
                }
            }
        }

        var planilha = GerarPlanilhaClientes(clientes);

        try
        {
            await EnviarEmailRelatorio(planilha, documentosProcessados);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao enviar e-mail com relatório: {ex.Message}");
        }
    }

    private byte[] GerarPlanilhaClientes(IList<ClienteResponse> clientes)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var excelPackage = new ExcelPackage();
        var planilha = excelPackage.Workbook.Worksheets.Add($"Documentos Processados - {DateTime.Today.ToShortDateString()}");

        PreencherCabecalhoPlanilhaClientes(planilha);

        for(int i = 2; i <= (clientes.Count + 1); i++)
        {
            var cliente = clientes[i - 2];

            planilha.Cells[i, 1].Value = cliente.Nome;
            planilha.Cells[i, 2].Value = cliente.Cpf;
            planilha.Cells[i, 3].Value = cliente.Endereco;
            planilha.Cells[i, 4].Value = cliente.Cidade;
            planilha.Cells[i, 5].Value = cliente.Estado;
            planilha.Cells[i, 6].Value = cliente.Ddd;
            planilha.Cells[i, 7].Value = cliente.Telefone;
        }

        return excelPackage.GetAsByteArray();
    }

    private void PreencherCabecalhoPlanilhaClientes(ExcelWorksheet planilha)
    {
        planilha.Cells[1, 1].Value = "Nome";
        planilha.Cells[1, 2].Value = "CPF";
        planilha.Cells[1, 3].Value = "Endereço";
        planilha.Cells[1, 4].Value = "Cidade";
        planilha.Cells[1, 5].Value = "Estado";
        planilha.Cells[1, 6].Value = "DDD";
        planilha.Cells[1, 7].Value = "Telefone";
    }

    private async Task EnviarEmailRelatorio(byte[] planilha, string documentosProcessados)
    {
        var planilhaStream = new MemoryStream(planilha);

        var mailMessage = new MailMessage()
        {
            From = new MailAddress(_smtpUsername),
            Subject = $"Relatório de Documentos Processados - {DateTime.Today.ToShortDateString()}",
            Body = documentosProcessados,
            IsBodyHtml = true,
            Attachments =
            {
                new Attachment(planilhaStream, $"Planilha Documentos Processados - {DateTime.Today.ToShortDateString()}")
            }
        };

        foreach (var destinatario in _destinatarios)
        {
            mailMessage.To.Add(destinatario);
        }

        //Desabilitei o envio de e-mail, pois para que ele funcione precisa de credenciais válidas, do contrário irá disparar Exception
        
        //await _smtpClient.SendMailAsync(mailMessage);
        
    }
}
