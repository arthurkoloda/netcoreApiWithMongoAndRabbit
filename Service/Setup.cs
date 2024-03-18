using Microsoft.Extensions.DependencyInjection;
using Service.Fila;
using Service.Interface.Documento;
using Service.Interface.Fila;
using Service.Interface.Cliente;
using Domain.Services;
using Domain;
using System.Diagnostics.CodeAnalysis;
using Domain.Interfaces;

namespace Service;

[ExcludeFromCodeCoverage]
public static class Setup
{
    /// <summary>
    /// Injeta as dependências dos Services, em seguida, chama o método que injetará as dependências dos repositories
    /// </summary>
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Setup), typeof(MappingProfile));

        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IClienteRabbitMQ, ClienteRabbitMQ>();
        services.AddScoped<IClienteValidator, ClienteValidator>();
        services.AddScoped<IRelatorioService, RelatorioService>();

        Database.Setup.AddDependencyInjectionRepositories(services);

        return services;
    }
}
