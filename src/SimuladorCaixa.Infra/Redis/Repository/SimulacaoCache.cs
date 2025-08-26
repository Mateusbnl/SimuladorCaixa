using SimuladorCaixa.Application.Repository;
using SimuladorCaixa.Core.Entidades;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimuladorCaixa.Infra.Redis.Repository
{
    public class SimulacaoCache : ISimulacaoRepository
    {
        private readonly IDatabase _redisDb;

        public SimulacaoCache(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        private static string GerarChave(int prazo, decimal valorDesejado, decimal taxaJuro)
        {
            var input = $"{prazo}:{valorDesejado}:{taxaJuro}";
            return input.ToString();
        }

        // ...

        public async Task<List<Simulacao>> GetSimulacao(int prazo, decimal valorDesejado, decimal taxaJuro)
        {
            var chave = GerarChave(prazo, valorDesejado, taxaJuro);
            var resultado = await _redisDb.StringGetAsync(chave);
            if (!resultado.HasValue)
                return null;

            return JsonSerializer.Deserialize<List<Simulacao>>(resultado.ToString());
        }

        public async Task SalvarSimulacao(int prazo, decimal valorDesejado, decimal taxaJuro, List<Simulacao> simulacoes)
        {
            var chave = GerarChave(prazo, valorDesejado, taxaJuro);
            await _redisDb.StringSetAsync(chave, JsonSerializer.Serialize(simulacoes));
        }
    }
}
