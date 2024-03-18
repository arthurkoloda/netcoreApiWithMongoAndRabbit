using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface.Documento;

namespace Exemplo_de_API_Net_Core_com_MongoDB.Controllers;

/// <summary>
/// Controlador responsável pelas operações envolvendo clientes
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class ClienteController : ControllerBase
{
    private readonly ILogger<ClienteController> _logger;
    private readonly IClienteService _clienteService;

    public ClienteController(ILogger<ClienteController> logger, IClienteService clienteService)
    {
        _logger = logger;
        _clienteService = clienteService;
    }

    /// <summary>
    /// Realiza a importação de um arquivo Excel contendo as informações dos clientes
    /// </summary>
    [HttpPost("importar-excel")]
    public async Task<IActionResult> Post_ImportarExcel(IFormFile arquivo)
    {
        try
        {
            await _clienteService.ProcessarExcel(arquivo);

            _logger.Log(LogLevel.Information, "Arquivo Excel com documentos recebido com sucesso, em breve ele será processado!");
            
            return Ok("Arquivo Excel com documentos recebido com sucesso, em breve ele será processado!");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, $"Erro ao importar arquivo Excel com documentos: {ex.Message}");

            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retorna os documentos que foram importados no sistema, bem como suas informações
    /// </summary>
    [HttpGet("documentos")]
    public async Task<IActionResult> Get_Documentos()
    {
        try
        {
            var documentos = await _clienteService.ListarDocumentos();

            return Ok(documentos);
        }
        catch (Exception ex)
        {
            return BadRequest($"Não foi possível obter os documentos: {ex.Message}");
        }
    }

    /// <summary>
    /// Retorna os documentos que ainda estão pendentes de processamento ou sendo processados
    /// </summary>
    [HttpGet("documentos-pendentes")]
    public async Task<IActionResult> Get_DocumentosPendentes()
    {
        try
        {
            var documentos = await _clienteService.ListarDocumentosPendentes();

            return Ok(documentos);
        }
        catch (Exception ex)
        {
            return BadRequest($"Não foi possível obter os documentos: {ex.Message}");
        }
    }

    /// <summary>
    /// Retorna os documentos que já foram processados pelo sistema, independente do resultado do processamento
    /// </summary>
    [HttpGet("documentos-processados")]
    public async Task<IActionResult> Get_DocumentosProcessados()
    {
        try
        {
            var documentos = await _clienteService.ListarDocumentosProcessados();

            return Ok(documentos);
        }
        catch (Exception ex)
        {
            return BadRequest($"Não foi possível obter os documentos: {ex.Message}");
        }
    }
}