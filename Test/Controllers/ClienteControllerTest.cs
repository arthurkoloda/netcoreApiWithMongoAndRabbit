using Amazon.Runtime.Internal.Util;
using Exemplo_de_API_Net_Core_com_MongoDB.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Service.Interface.Documento;

namespace Test.Controllers;

public class ClienteControllerTest
{
    private readonly ClienteController _controller;
    private readonly Mock<ILogger<ClienteController>> _logger;
    private readonly Mock<IClienteService> _service;

    public ClienteControllerTest()
    {
        _logger = new Mock<ILogger<ClienteController>>();
        _service = new Mock<IClienteService>();

        _controller = new ClienteController(_logger.Object, _service.Object);
    }

    [Fact]
    public async Task ImportarExcel()
    {
        _service.Setup(x => x.ProcessarExcel(It.IsAny<IFormFile>()));

        var result = await _controller.Post_ImportarExcel(It.IsAny<IFormFile>());

        Assert.NotNull(result);
        Assert.Equal(result.GetType(), typeof(OkObjectResult));

        _service.Verify(x => x.ProcessarExcel(It.IsAny<IFormFile>()), Times.Once);
    }

    [Fact]
    public async Task ImportarExcel_Exception()
    {
        _service.Setup(x => x.ProcessarExcel(It.IsAny<IFormFile>())).ThrowsAsync(new Exception("teste-exception"));

        var result = await _controller.Post_ImportarExcel(It.IsAny<IFormFile>());

        Assert.NotNull(result);
        Assert.Equal(result.GetType(), typeof(BadRequestObjectResult));

        _service.Verify(x => x.ProcessarExcel(It.IsAny<IFormFile>()), Times.Once);
    }

    [Fact]
    public async Task Documentos()
    {
        _service.Setup(x => x.ListarDocumentos());

        var result = await _controller.Get_Documentos();

        Assert.NotNull(result);
        Assert.Equal(result.GetType(), typeof(OkObjectResult));

        _service.Verify(x => x.ListarDocumentos(), Times.Once);
    }

    [Fact]
    public async Task Documentos_Exception()
    {
        _service.Setup(x => x.ListarDocumentos()).ThrowsAsync(new Exception("teste-exception"));

        var result = await _controller.Get_Documentos();

        Assert.NotNull(result);
        Assert.Equal(result.GetType(), typeof(BadRequestObjectResult));

        _service.Verify(x => x.ListarDocumentos(), Times.Once);
    }

    [Fact]
    public async Task Documentos_Pendentes()
    {
        _service.Setup(x => x.ListarDocumentosPendentes());

        var result = await _controller.Get_DocumentosPendentes();

        Assert.NotNull(result);
        Assert.Equal(result.GetType(), typeof(OkObjectResult));

        _service.Verify(x => x.ListarDocumentosPendentes(), Times.Once);
    }

    [Fact]
    public async Task DocumentosPendentes_Exception()
    {
        _service.Setup(x => x.ListarDocumentosPendentes()).ThrowsAsync(new Exception("teste-exception"));

        var result = await _controller.Get_DocumentosPendentes();

        Assert.NotNull(result);
        Assert.Equal(result.GetType(), typeof(BadRequestObjectResult));

        _service.Verify(x => x.ListarDocumentosPendentes(), Times.Once);
    }

    [Fact]
    public async Task Documentos_Processados()
    {
        _service.Setup(x => x.ListarDocumentosProcessados());

        var result = await _controller.Get_DocumentosProcessados();

        Assert.NotNull(result);
        Assert.Equal(result.GetType(), typeof(OkObjectResult));

        _service.Verify(x => x.ListarDocumentosProcessados(), Times.Once);
    }

    [Fact]
    public async Task DocumentosProcessados_Exception()
    {
        _service.Setup(x => x.ListarDocumentosProcessados()).ThrowsAsync(new Exception("teste-exception"));

        var result = await _controller.Get_DocumentosProcessados();

        Assert.NotNull(result);
        Assert.Equal(result.GetType(), typeof(BadRequestObjectResult));

        _service.Verify(x => x.ListarDocumentosProcessados(), Times.Once);
    }
}
