using MongoDB.Driver;
using SimuladorCaixa.Api.Middleware.Repository;
using SimuladorCaixa.Application.DTO;
using SimuladorCaixa.Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorCaixa.Infra.MongoDB.Repository
{
    public class TelemetriaRepository : ITelemetriaRepository
    {
        private readonly IMongoCollection<Telemetria> _telemetriaCollection;

        public TelemetriaRepository(IMongoDatabase database)
        {
            _telemetriaCollection = database.GetCollection<Telemetria>("Telemetria");
        }

        public async Task SalvarTelemetriaAsync(Telemetria telemetria)
        {
            await _telemetriaCollection.InsertOneAsync(telemetria);
        }

        public async Task<TelemetriaTotalResumoDTO> ConsultarTelemetriasAsync(DateTime data)
        {
                var dataInicio = data.Date;
                var dataFim = data.Date.AddDays(1);

                var response = new TelemetriaTotalResumoDTO
                {
                    dataReferencia = dataInicio,
                    telemetrias = new List<TelemetriaResumoDTO>()
                };

            var pipeline = new[]
                {
                    // Filtra telemetrias pela data
                    Builders<Telemetria>.Filter.Gte(t => t.dataHora, dataInicio),
                    Builders<Telemetria>.Filter.Lt(t => t.dataHora, dataFim)
                };

                var match = Builders<Telemetria>.Filter.And(pipeline);

                var aggregate = _telemetriaCollection.Aggregate()
                    .Match(match)
                    .Group(
                        t => t.endpoint,
                        g => new
                        {
                            Endpoint = g.Key,
                            QtdRequisicoes = g.Count(),
                            TempoMedio = g.Average(x => x.tempo),
                            TempoMinimo = g.Min(x => x.tempo),
                            TempoMaximo = g.Max(x => x.tempo),
                            QtdSucesso = g.Count(x => x.sucesso)
                        }
                    );

                var resultado = await aggregate.ToListAsync();
                foreach (var item in resultado)
                {
                    var resumo = new TelemetriaResumoDTO
                    {
                        nomeApi = item.Endpoint,
                        qtdRequisicoes = item.QtdRequisicoes,
                        tempoMedio = (int)item.TempoMedio,
                        tempoMinimo = (int)item.TempoMinimo,
                        tempoMaximo = (int)item.TempoMaximo,
                        percentualSucesso = item.QtdRequisicoes > 0 ? (decimal)item.QtdSucesso / item.QtdRequisicoes * 100 : 0
                    };
                    response.telemetrias.Add(resumo);
            }


            return response;
        }
    }
}
