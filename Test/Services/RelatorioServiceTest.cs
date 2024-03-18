using Database.Models;
using Domain.Models;
using Domain.Services;
using Moq;
using Service.Interface.Documento;

namespace Test.Services;

public class RelatorioServiceTest
{
    private readonly RelatorioService _service;
    private Mock<IClienteService> _clienteService;

    public RelatorioServiceTest()
    {
        _clienteService = new Mock<IClienteService>();

        _service = new RelatorioService(_clienteService.Object);
    }

    [Fact] 
    public async Task EnviarEmail()
    {
        var documentos = new List<Documento>()
        {
            new Documento()
            {
                DataImportacao = DateTime.Now.AddMinutes(-25),
                DataProcessamento = DateTime.Now.AddMinutes(-10),
                Nome = "Documento teste",
                Id = Guid.NewGuid().ToString(),
                Situacao = Database.Enums.ESituacaoDocumento.Processado
            }
        };

        var clientes = new List<ClienteResponse>()
        {
            new ClienteResponse()
            {
                Nome = "Arthur",
                Cidade = "Mallet",
                Estado = "PR",
                Cpf = "98765432100",
                Ddd = "42",
                Endereco = "Uma Rua Qualquer",
                Telefone = "999999999"
            }
        };

        _clienteService.Setup(x => x.ListarDocumentosProcessadosPorData(DateTime.Today)).ReturnsAsync(documentos);
        _clienteService.Setup(x => x.ListarClientesDocumento(documentos[0].Id)).ReturnsAsync(clientes);

        await _service.GerarRelatorioDiario();

        _clienteService.Verify(x => x.ListarDocumentosProcessadosPorData(DateTime.Today), Times.Once);
        _clienteService.Verify(x => x.ListarClientesDocumento(documentos[0].Id), Times.Once);
    }
}
