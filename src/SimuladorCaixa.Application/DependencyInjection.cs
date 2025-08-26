using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimuladorCaixa.Application.Repository;
using SimuladorCaixa.Application.UseCases;


namespace SimuladorCaixa.Infra
{
    public static class DependencyInjection
    {   public static void AddUseCases(this IHostApplicationBuilder builder)
        {
           // Adiciona Banco de Dados
           builder.Services.AddScoped<SimularTodasModalidadesUseCase>();
           builder.Services.AddScoped<ListarSimulacoesRealizadasPaginadasUseCase>();
           builder.Services.AddScoped<ListarAgregadoSimulacaoPorProdutoDataUseCase>();
           builder.Services.AddScoped<ListarTelemetriaEndpointUseCase>();
            // Adiciona Repositórios
        }
    }
}
