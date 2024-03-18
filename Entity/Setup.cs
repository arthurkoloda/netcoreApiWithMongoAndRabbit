using Database.Interface;
using Database.Repository;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Database;

[ExcludeFromCodeCoverage]
public static class Setup
{
    /// <summary>
    /// Injeta as dependências dos Repositories
    /// </summary>
    public static IServiceCollection AddDependencyInjectionRepositories(this IServiceCollection services)
    {
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IDocumentoRepository, DocumentoRepository>();

        return services;
    }
}
