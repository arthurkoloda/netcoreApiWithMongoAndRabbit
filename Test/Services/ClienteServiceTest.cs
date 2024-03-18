using Database.Enums;
using Database.Interface;
using Database.Models;
using Domain.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Service.DTO;
using Service.Interface.Cliente;
using Service.Interface.Documento;
using Service.Interface.Fila;
using Test.Fixtures;
using Test.Helper;

namespace Test.Services;

public class ClienteServiceTest
{
    private readonly IClienteService _clienteService;
    private readonly Mock<IClienteRepository> _repository;
    private readonly Mock<IDocumentoRepository> _documentoRepository;
    private readonly Mock<IClienteValidator> _clienteValidator;
    private readonly MapperFixture _mapperFixture;
    private readonly Mock<IClienteRabbitMQ> _clienteRabbitMQ;

    public ClienteServiceTest()
    {
        _clienteRabbitMQ = new Mock<IClienteRabbitMQ>();
        _clienteValidator = new Mock<IClienteValidator>();
        _repository = new Mock<IClienteRepository>();
        _documentoRepository = new Mock<IDocumentoRepository>();
        _mapperFixture = new MapperFixture();

        _clienteService = new ClienteService(_clienteRabbitMQ.Object, _clienteValidator.Object, _repository.Object, _documentoRepository.Object, _mapperFixture.Mapper);
    }

    [Fact]
    public async Task ListarDocumentos_Null()
    {
        IList<Documento> documentos = null;

        _documentoRepository.Setup(c => c.ListAllAsync()).ReturnsAsync(documentos);

        var documentosRetornados = await _clienteService.ListarDocumentos();

        Assert.Null(documentosRetornados);

        _documentoRepository.Verify(c => c.ListAllAsync());
    }

    [Fact]
    public async Task ListarDocumentos_Empty()
    {
        IList<Documento> documentos = new List<Documento>();

        _documentoRepository.Setup(c => c.ListAllAsync()).ReturnsAsync(documentos);

        var documentosRetornados = await _clienteService.ListarDocumentos();

        Assert.Null(documentosRetornados);

        _documentoRepository.Verify(c => c.ListAllAsync());
    }

    [Fact]
    public async Task ListarDocumentos()
    {
        IList<Documento> documentos = new List<Documento>()
        {
            new Documento()
            {
                DataImportacao = DateTime.Now,
                DataProcessamento = DateTime.Now,
                Nome = "Documento teste",
                Situacao = ESituacaoDocumento.Processado}
        };

        _documentoRepository.Setup(c => c.ListAllAsync()).ReturnsAsync(documentos);

        var documentosRetornados = await _clienteService.ListarDocumentos();

        Assert.NotNull(documentosRetornados);
        Assert.NotEmpty(documentosRetornados);
        Assert.Single(documentosRetornados);

        _documentoRepository.Verify(c => c.ListAllAsync());
    }

    [Fact]
    public async Task ListarDocumentosPendentes_Null()
    {
        IList<Documento> documentos = null;

        _documentoRepository.Setup(c => c.ListPending()).ReturnsAsync(documentos);

        var documentosRetornados = await _clienteService.ListarDocumentosPendentes();

        Assert.Null(documentosRetornados);

        _documentoRepository.Verify(c => c.ListPending());
    }

    [Fact]
    public async Task ListarDocumentosPendentes_Empty()
    {
        IList<Documento> documentos = new List<Documento>();

        _documentoRepository.Setup(c => c.ListPending()).ReturnsAsync(documentos);

        var documentosRetornados = await _clienteService.ListarDocumentosPendentes();

        Assert.Null(documentosRetornados);

        _documentoRepository.Verify(c => c.ListPending());
    }

    [Fact]
    public async Task ListarDocumentosPendentes()
    {
        IList<Documento> documentos = new List<Documento>()
        {
            new Documento()
            {
                DataImportacao = DateTime.Now,
                DataProcessamento = DateTime.Now,
                Nome = "Documento teste",
                Situacao = ESituacaoDocumento.Processado}
        };

        _documentoRepository.Setup(c => c.ListPending()).ReturnsAsync(documentos);

        var documentosRetornados = await _clienteService.ListarDocumentosPendentes();

        Assert.NotNull(documentosRetornados);
        Assert.NotEmpty(documentosRetornados);
        Assert.Single(documentosRetornados);

        _documentoRepository.Verify(c => c.ListPending());
    }

    [Fact]
    public async Task ListarDocumentosProcessados_Null()
    {
        IList<Documento> documentos = null;

        _documentoRepository.Setup(c => c.ListProcessed()).ReturnsAsync(documentos);

        var documentosRetornados = await _clienteService.ListarDocumentosProcessados();

        Assert.Null(documentosRetornados);

        _documentoRepository.Verify(c => c.ListProcessed());
    }

    [Fact]
    public async Task ListarDocumentosProcessados_Empty()
    {
        IList<Documento> documentos = new List<Documento>();

        _documentoRepository.Setup(c => c.ListProcessed()).ReturnsAsync(documentos);

        var documentosRetornados = await _clienteService.ListarDocumentosProcessados();

        Assert.Null(documentosRetornados);

        _documentoRepository.Verify(c => c.ListProcessed());
    }

    [Fact]
    public async Task ListarDocumentosProcessados()
    {
        IList<Documento> documentos = new List<Documento>()
        {
            new Documento()
            {
                DataImportacao = DateTime.Now,
                DataProcessamento = DateTime.Now,
                Nome = "Documento teste",
                Situacao = ESituacaoDocumento.Processado}
        };

        _documentoRepository.Setup(c => c.ListProcessed()).ReturnsAsync(documentos);

        var documentosRetornados = await _clienteService.ListarDocumentosProcessados();

        Assert.NotNull(documentosRetornados);
        Assert.NotEmpty(documentosRetornados);
        Assert.Single(documentosRetornados);

        _documentoRepository.Verify(c => c.ListProcessed());
    }

    [Fact]
    public async Task ProcessarExcel()
    {
        var arquivoMock = new Mock<IFormFile>();
        arquivoMock.Setup(x => x.FileName).Returns("arquivo.xlsx");

        _clienteValidator.Setup(x => x.ValidarArquivoExcel(It.IsAny<IFormFile>())).Verifiable();
        _clienteValidator.Setup(x => x.ValidarPlanilha(It.IsAny<MemoryStream>())).Verifiable();

        _documentoRepository.Setup(x => x.InsertAsync(It.IsAny<Documento>())).Returns(Task.CompletedTask);

        // Act
        await _clienteService.ProcessarExcel(arquivoMock.Object);

        // Assert
        _clienteValidator.Verify();
        _documentoRepository.Verify(x => x.InsertAsync(It.Is<Documento>(d => d.Nome == "arquivo.xlsx" && d.DataImportacao <= DateTime.Now && d.DataImportacao > DateTime.Now.AddSeconds(-5))), Times.Once);
        _clienteRabbitMQ.Verify(x => x.ArmazenarEmFila(It.IsAny<MemoryStream>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ProcessarFila_SemDadosNaFila()
    {
        PlanilhaFilaDTO? dadosFila = null;
        
        _clienteRabbitMQ.Setup(x => x.LerDocumentoFila()).Returns(dadosFila);

        await _clienteService.ProcessarFila();

        _documentoRepository.VerifyNoOtherCalls();
        _clienteValidator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ProcessarFila_DadosNaFila_SemDocumentoCorrespondente()
    {
        Documento? documento = null;

        var dadosFila = new PlanilhaFilaDTO 
        { 
            DocumentoId = Guid.NewGuid().ToString(), 
            Arquivo = new byte[] { } 
        };

        _clienteRabbitMQ.Setup(x => x.LerDocumentoFila()).Returns(dadosFila);
        _documentoRepository.Setup(x => x.GetAsync(dadosFila.DocumentoId)).ReturnsAsync(documento);

        await _clienteService.ProcessarFila();

        _documentoRepository.Verify(x => x.GetAsync(dadosFila.DocumentoId), Times.Once);
        _clienteValidator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ProcessarFila()
    {
        var documentoId = Guid.NewGuid().ToString();

        var documento = new Documento { Id = documentoId, Nome = "Documento de Teste" };

        var dadosFila = new PlanilhaFilaDTO { DocumentoId = documentoId, Arquivo = PlanilhaHelper.ObterPlanilhaEmByteArray() };

        _clienteRabbitMQ.Setup(x => x.LerDocumentoFila()).Returns(dadosFila);
        _documentoRepository.Setup(x => x.GetAsync(dadosFila.DocumentoId)).ReturnsAsync(documento);

        await _clienteService.ProcessarFila();

        _documentoRepository.Verify(x => x.UpdateAsync(documentoId, It.IsAny<Documento>()), Times.AtLeast(2));
        _clienteValidator.Verify();
    }

    [Fact]
    public async Task ProcessarFila_Exeption()
    {
        var documentoId = Guid.NewGuid().ToString();

        var documento = new Documento { Id = documentoId, Nome = "Documento de Teste" };

        var dadosFila = new PlanilhaFilaDTO { DocumentoId = documentoId, Arquivo = PlanilhaHelper.ObterPlanilhaEmByteArray() };

        _clienteRabbitMQ.Setup(x => x.LerDocumentoFila()).Returns(dadosFila);
        _documentoRepository.Setup(x => x.GetAsync(dadosFila.DocumentoId)).ReturnsAsync(documento);
        _repository.Setup(x => x.InsertAsync(It.IsAny<Cliente>())).ThrowsAsync(new Exception("teste-exception"));

        await _clienteService.ProcessarFila();

        _repository.Verify(x => x.InsertAsync(It.IsAny<Cliente>()), "teste-exception");
        _documentoRepository.Verify(x => x.UpdateAsync(documentoId, It.IsAny<Documento>()), Times.AtLeast(2));
        _clienteValidator.Verify();
    }

    [Fact]
    public async Task ProcessarFila_InvalidDataExeption()
    {
        var documentoId = Guid.NewGuid().ToString();

        var documento = new Documento { Id = documentoId, Nome = "Documento de Teste" };

        var dadosFila = new PlanilhaFilaDTO { DocumentoId = documentoId, Arquivo = PlanilhaHelper.ObterPlanilhaEmByteArray() };

        _clienteRabbitMQ.Setup(x => x.LerDocumentoFila()).Returns(dadosFila);
        _documentoRepository.Setup(x => x.GetAsync(dadosFila.DocumentoId)).ReturnsAsync(documento);
        _repository.Setup(x => x.InsertAsync(It.IsAny<Cliente>())).ThrowsAsync(new InvalidDataException("teste-exception"));

        await _clienteService.ProcessarFila();

        _repository.Verify(x => x.InsertAsync(It.IsAny<Cliente>()), "teste-exception");
        _documentoRepository.Verify(x => x.UpdateAsync(documentoId, It.IsAny<Documento>()), Times.AtLeast(2));
        _clienteValidator.Verify();
    }
}
