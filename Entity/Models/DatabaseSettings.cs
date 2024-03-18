using System.Diagnostics.CodeAnalysis;

namespace Database.Models;

[ExcludeFromCodeCoverage]
/// <summary>
/// Classe estática onde estão armazenados os dados para conectar-se ao banco de dados do MongoDB
/// </summary>
public static class DatabaseSettings
{
    /// <summary>
    /// String de conexão para acesso ao banco
    /// </summary>
    public static string ConnectionString => "mongodb://localhost:27017";

    /// <summary>
    /// Nome da base de dados que estára sendo conectada
    /// </summary>
    public static string DatabaseName => "mongodb";
}
