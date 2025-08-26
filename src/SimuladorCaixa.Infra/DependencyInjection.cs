using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using SimuladorCaixa.Api.Middleware.Repository;
using SimuladorCaixa.Application.Repository;
using SimuladorCaixa.Infra.Cache;
using SimuladorCaixa.Infra.MongoDB.Repository;
using SimuladorCaixa.Infra.Redis.Repository;
using SimuladorCaixa.Infra.Sqlite.Repository;
using SimuladorCaixa.Infra.SqlServer.Factory;
using StackExchange.Redis;

namespace SimuladorCaixa.Infra
{
    public static class DependencyInjection
    {   public static void AddInfra(this IHostApplicationBuilder builder)
        { 
            var configuration = builder.Configuration;
            // Adiciona Banco de Dados
            builder.Services.AddSingleton<SqlConnectionFactory>();

            // Adiciona MongoDB
            var mongoClient = new MongoClient(configuration.GetConnectionString("mongoDb"));
            var mongoDatabase = mongoClient.GetDatabase("simuladorcaixa");
            builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);

            //Adiciona Redis
            builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetConnectionString("redis")));

            // Adiciona Repositórios e Caches
            builder.Services.AddScoped<IProdutoRepository, CachedProdutoRepository>();
            builder.Services.AddScoped<ProdutoRepository>();
            builder.Services.AddScoped<IPropostaRepository, PropostaRepository>();
            builder.Services.AddScoped<IRelatorioRepository, RelatorioRepository>();
            builder.Services.AddScoped<ISimulacaoRepository, SimulacaoCache>();
            builder.Services.AddSingleton<ITelemetriaRepository, TelemetriaRepository>();
            builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
        }
    }
}
