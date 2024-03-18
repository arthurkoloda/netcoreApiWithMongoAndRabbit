using Database.Enums;
using Database.Interface;
using Database.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Database.Repository;

internal class DocumentoRepository : IDocumentoRepository
{
    private readonly IMongoCollection<Documento> _documentoCollection;

    public DocumentoRepository()
    {
        var mongoClient = new MongoClient(DatabaseSettings.ConnectionString);
        var mongoDB = mongoClient.GetDatabase(DatabaseSettings.DatabaseName);

        _documentoCollection = mongoDB.GetCollection<Documento>("Documento");
    }

    public async Task InsertAsync(Documento documento)
    {
        await _documentoCollection.InsertOneAsync(documento);
    
    }

    public async Task<Documento?> GetAsync(string id)
    {
        return await _documentoCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(string id, Documento documento)
    {
        await _documentoCollection.ReplaceOneAsync(x => x.Id == id, documento);
    }

    public async Task<IList<Documento>?> ListAllAsync()
    {
        return await _documentoCollection.Find(_ => true)
                                            .SortByDescending(x => x.DataImportacao)
                                            .ToListAsync();
    }

    public async Task<IList<Documento>?> ListPending()
    {
        var situacoes = new List<ESituacaoDocumento>() { ESituacaoDocumento.AguardandoProcessamento, ESituacaoDocumento.Processando };

        return await _documentoCollection.Find(x => situacoes.Contains(x.Situacao))
                                                                    .SortByDescending(x => x.DataImportacao)
                                                                    .ToListAsync();
    }

    public async Task<IList<Documento>?> ListProcessed()
    {
        var situacoes = new List<ESituacaoDocumento>() { ESituacaoDocumento.Falha, ESituacaoDocumento.Processado, ESituacaoDocumento.ProcessadoComErro };

        return await _documentoCollection.Find(x => situacoes.Contains(x.Situacao))
                                                                    .SortByDescending(x => x.DataImportacao)
                                                                    .ToListAsync();
    }

    public async Task<IList<Documento>?> ListProcessedDaily(DateTime dataProcessamento)
    {
        return await _documentoCollection
                                .Find(x => x.DataProcessamento.HasValue &&
                                            x.DataProcessamento.Value.Date == dataProcessamento.Date)
                                .SortByDescending(x => x.DataProcessamento)
                                .ToListAsync();
    }
}
