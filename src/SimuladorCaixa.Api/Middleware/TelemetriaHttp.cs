using SimuladorCaixa.Api.Middleware.Repository;
using SimuladorCaixa.Application.Model;
using System.Diagnostics;

namespace SimuladorCaixa.Api.Middleware
{
    public class TelemetriaHttp
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TelemetriaHttp> _logger;
        private readonly ITelemetriaRepository _telemetriaRepository;

        public TelemetriaHttp(RequestDelegate next, ILogger<TelemetriaHttp> logger, ITelemetriaRepository telemetriaRepository)
        {
            _next = next;
            _logger = logger;
            _telemetriaRepository = telemetriaRepository;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var endpoint = context.Request.Path;
            var method = context.Request.Method;
            int statusCode = 200;
            bool sucesso = true;

            try
            {
                await _next(context);
                statusCode = context.Response.StatusCode;
                sucesso = statusCode >= 200 && statusCode < 400;
            }
            catch (Exception ex)
            {
                sucesso = false;
                statusCode = 500;
                _logger.LogError(ex, "Erro na chamada ao endpoint {Endpoint}", endpoint);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                var telemetria = new Telemetria
                {
                    endpoint = endpoint,
                    metodo = method,
                    statusCode = statusCode,
                    sucesso = sucesso,
                    tempo = stopwatch.ElapsedMilliseconds
                };
                _telemetriaRepository.SalvarTelemetriaAsync(telemetria);
                _logger.LogInformation(
                    "Chamada HTTP: {Method} {Endpoint} | Status: {StatusCode} | Sucesso: {Sucesso} | Tempo: {Tempo}ms",
                    method, endpoint, statusCode, sucesso, stopwatch.ElapsedMilliseconds
                );
            }
        }
    }
}
