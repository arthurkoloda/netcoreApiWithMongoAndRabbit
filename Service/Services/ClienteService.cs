using AutoMapper;
using Database.Interface;
using Database.Models;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Service.Interface.Cliente;
using Service.Interface.Documento;
using Service.Interface.Fila;

namespace Domain.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRabbitMQ _clienteRabbitMQ;
    private readonly IClienteValidator _clienteValidator;
    private readonly IClienteRepository _clienteRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly IMapper _mapper;

    public ClienteService(IClienteRabbitMQ clienteRabbitMQ, IClienteValidator clienteValidator, IClienteRepository clienteRepository, IDocumentoRepository documentoRepository, IMapper mapper)
    {
        _clienteRabbitMQ = clienteRabbitMQ;
        _clienteValidator = clienteValidator;
        _clienteRepository = clienteRepository;
        _documentoRepository = documentoRepository;
        _mapper = mapper;
    }

    public async Task ProcessarExcel(IFormFile arquivoExcel)
    {
        _clienteValidator.ValidarArquivoExcel(arquivoExcel);

        using (var memoryStream = new MemoryStream())
        {
            await arquivoExcel.CopyToAsync(memoryStream);

            _clienteValidator.ValidarPlanilha(memoryStream);

            var documento = new Documento()
            {
                DataImportacao = DateTime.UtcNow.ToLocalTime(),
                Nome = arquivoExcel.FileName
            };

            await _documentoRepository.InsertAsync(documento);

            _clienteRabbitMQ.ArmazenarEmFila(memoryStream, documento.Id);
        }
    }

    public async Task ProcessarFila()
    {
        var dadosFila = _clienteRabbitMQ.LerDocumentoFila();

        if(dadosFila == null)
        {
            return;
        }

        var documento = await _documentoRepository.GetAsync(dadosFila.DocumentoId);

        if(documento == null)
        {
            return;
        }

        documento.Situacao = Database.Enums.ESituacaoDocumento.Processando;

        await _documentoRepository.UpdateAsync(dadosFila.DocumentoId, documento);

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        ExcelPackage? package = null;

        using (var memoryStream = new MemoryStream(dadosFila.Arquivo))
        {
            package = new ExcelPackage(memoryStream);
        }

        if (package != null)
        {
            //Obtém a planilha que está contida dentro do arquivo (package)
            var planilha = package.Workbook.Worksheets[0];

            //Para cada linha da planilha, irá validar e processar o dado
            for (int i = 2; i <= planilha.Rows.Count(); i++)
            {
                var cliente = new Cliente()
                {
                    Nome = planilha.Cells[i, 1]?.Value?.ToString() ?? string.Empty,
                    Cpf = planilha.Cells[i, 2]?.Value?.ToString() ?? string.Empty,
                    Endereco = planilha.Cells[i, 3]?.Value?.ToString() ?? string.Empty,
                    Cidade = planilha.Cells[i, 4]?.Value?.ToString() ?? string.Empty,
                    Estado = planilha.Cells[i, 5]?.Value?.ToString() ?? string.Empty,
                    Ddd = planilha.Cells[i, 6]?.Value?.ToString() ?? string.Empty,
                    Telefone = planilha.Cells[i, 7]?.Value?.ToString() ?? string.Empty
                };

                try
                {
                    _clienteValidator.ValidarCliente(cliente);

                    cliente.DocumentoId = dadosFila.DocumentoId;

                    await _clienteRepository.InsertAsync(cliente);

                    InserirLogInformativo(documento, $"Cliente {cliente.Nome} foi inserido com sucesso. Linha: {i}. Arquivo: {documento.Nome}");
                }
                catch (InvalidDataException ex)
                {
                    InserirLogErro(documento, $"{ex.Message}. Linha: {i}. Arquivo: {documento.Nome}.");
                }
                catch (Exception ex)
                {
                    InserirLogErro(documento, $"{ex.Message}. Linha: {i}. Arquivo: {documento.Nome}.");
                }
            }
        }

        documento.DataProcessamento = DateTime.UtcNow.ToLocalTime();

        documento.Situacao = 
                    documento.ErrorLog.Any() && !documento.InformationLog.Any() ? Database.Enums.ESituacaoDocumento.Falha :
                    !documento.ErrorLog.Any() && !documento.InformationLog.Any() ? Database.Enums.ESituacaoDocumento.Falha :
                    documento.ErrorLog.Any() && documento.InformationLog.Any() ? Database.Enums.ESituacaoDocumento.ProcessadoComErro : 
                    Database.Enums.ESituacaoDocumento.Processado;

        await _documentoRepository.UpdateAsync(dadosFila.DocumentoId, documento);   
    }

    public async Task<IList<DocumentoResponse>?> ListarDocumentos()
    {
        var documentosEntity = await _documentoRepository.ListAllAsync();

        if(documentosEntity != null && documentosEntity.Any())
        {
            var documentos = _mapper.Map<IList<DocumentoResponse>>(documentosEntity);
            return documentos;
        }

        return null;
    }

    public async Task<IList<DocumentoResponse>?> ListarDocumentosPendentes()
    {
        var documentosEntity = await _documentoRepository.ListPending();

        if (documentosEntity != null && documentosEntity.Any())
        {
            return _mapper.Map<IList<DocumentoResponse>>(documentosEntity);
        }

        return null;
    }

    public async Task<IList<DocumentoResponse>?> ListarDocumentosProcessados()
    {
        var documentosEntity = await _documentoRepository.ListProcessed();

        if (documentosEntity != null && documentosEntity.Any())
        {
            return _mapper.Map<IList<DocumentoResponse>>(documentosEntity);
        }

        return null;
    }

    private void InserirLogInformativo(Documento documento, string mensagemLog)
    {
        documento.InformationLog.Add(mensagemLog);
    }

    private void InserirLogErro(Documento documento, string mensagemLog)
    {
        documento.ErrorLog.Add(mensagemLog);
    }

    public async Task<IList<Documento>?> ListarDocumentosProcessadosPorData(DateTime data)
    {
        return await _documentoRepository.ListProcessedDaily(data);
    }

    public async Task<IList<ClienteResponse>?> ListarClientesDocumento(string documentoId)
    {
        var clientesDb = await _clienteRepository.ListByDocumentoId(documentoId);

        if (clientesDb != null && clientesDb.Any())
        {
            return _mapper.Map<List<ClienteResponse>>(clientesDb);
        }

        return null;
    }
}
