using Consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service;

Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services.AddDependencyInjection();
        services.AddHostedService<DocumentoConsumer>();
        services.AddHostedService<RelatorioConsumer>();

    })
    .Build()
    .Run();