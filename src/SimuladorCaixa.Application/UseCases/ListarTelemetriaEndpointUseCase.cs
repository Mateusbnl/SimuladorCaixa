
using SimuladorCaixa.Api.Middleware.Repository;
using SimuladorCaixa.Application.DTO;

namespace SimuladorCaixa.Application.UseCases
{
    public class ListarTelemetriaEndpointUseCase
    {
        private readonly ITelemetriaRepository _telemetriaRepository;
        public ListarTelemetriaEndpointUseCase(ITelemetriaRepository telemetriaRepository)
        {
            _telemetriaRepository = telemetriaRepository;
        }
        public async Task<TelemetriaTotalResumoDTO> ExecuteAsync(DateTime data)
        {
            return await _telemetriaRepository.ConsultarTelemetriasAsync(data);
        }
    }
}
