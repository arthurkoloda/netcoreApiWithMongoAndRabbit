namespace Domain.Models;

public class ClienteResponse
{
    /// <summary>
    /// Nome do cliente
    /// </summary>
    public string Nome { get; set; }

    /// <summary>
    /// Documento CPF do cliente
    /// </summary>
    public string Cpf { get; set; }

    /// <summary>
    /// Endereço de moradia do cliente
    /// </summary>
    public string Endereco { get; set; }

    /// <summary>
    /// Cidade de moradia do cliente
    /// </summary>
    public string Cidade { get; set; }

    /// <summary>
    /// Estado de moradia do cliente
    /// </summary>
    public string Estado { get; set; }

    /// <summary>
    /// Código de área do telefone principal do cliente
    /// </summary>
    public string Ddd { get; set; }

    /// <summary>
    /// Número de telefone do cliente (sem código de área/DDD)
    /// </summary>
    public string Telefone { get; set; }
}
