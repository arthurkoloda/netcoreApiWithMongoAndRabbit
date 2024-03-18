using Database.Interface;
using Database.Models;
using MongoDB.Driver;

namespace Database.Repository;

public class ClienteRepository : IClienteRepository
{
    private readonly IMongoCollection<Cliente> _clienteCollection;

    public ClienteRepository()
    {
        var mongoClient = new MongoClient(DatabaseSettings.ConnectionString);
        var mongoDB = mongoClient.GetDatabase(DatabaseSettings.DatabaseName);

        _clienteCollection = mongoDB.GetCollection<Cliente>("Cliente");
    }

    public async Task InsertAsync(Cliente entity)
    {
        await _clienteCollection.InsertOneAsync(entity);
    }

    public async Task<IList<Cliente>?> ListByDocumentoId(string documentoId)
    {
        return await _clienteCollection.Find(x => x.DocumentoId == documentoId).ToListAsync();
    }
}
