using SimuladorCaixa.Application.DTO;
using SimuladorCaixa.Application.Model;

namespace SimuladorCaixa.Api.Middleware.Repository
{
    public interface ITelemetriaRepository
    {
        public Task SalvarTelemetriaAsync(Telemetria telemetria);
        public Task<TelemetriaTotalResumoDTO> ConsultarTelemetriasAsync(DateTime data);
    }
}
