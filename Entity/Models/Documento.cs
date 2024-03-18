using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Database.Enums;

namespace Database.Models;

/// <summary>
/// Classe responsável por mapear os dados dos clientes no banco de dados
/// </summary>
public class Documento
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>
    /// Nome do arquivo
    /// </summary>
    public string? Nome { get; set; }

    /// <summary>
    /// Data em que o arquivo foi recebido pelo sistema
    /// </summary>
    public DateTime? DataImportacao { get; set; }

    /// <summary>
    /// Data em que o arquivo foi processado pelo sistema
    /// </summary>
    public DateTime? DataProcessamento { get; set; }

    /// <summary>
    /// Situação em que se encontra o documento, referente ao processamento dos dados
    /// </summary>
    public ESituacaoDocumento Situacao { get; set; } = ESituacaoDocumento.AguardandoProcessamento;

    /// <summary>
    /// Armazena as informações referentes aos erros ocorridos no processamento do arquivo
    /// </summary>
    public IList<string> ErrorLog { get; set; } = new List<string>();

    /// <summary>
    /// Armazena informações importantes referente ao arquivo
    /// </summary>
    public IList<string> InformationLog { get; set; } = new List<string>();
}
