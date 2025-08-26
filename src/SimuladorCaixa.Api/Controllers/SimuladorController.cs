using Microsoft.AspNetCore.Mvc;
using SimuladorCaixa.Application.DTO;
using SimuladorCaixa.Application.UseCases;
using SimuladorCaixa.Application.Extensions;
using MongoDB.Bson;

namespace SimuladorCaixa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimuladorController : ControllerBase
    {
        private readonly SimularTodasModalidadesUseCase _simularTodasModalidadesUseCase;
        private readonly ListarSimulacoesRealizadasPaginadasUseCase _listarSimulacoesRealizadasPaginadasUseCase;
        private readonly ListarAgregadoSimulacaoPorProdutoDataUseCase _listarAgregadoSimulacaoPorProdutoDataUseCase;
        private readonly ListarTelemetriaEndpointUseCase _ListarTelemetriaEndpointUseCase;
        private readonly ILogger _logger;

        public SimuladorController(SimularTodasModalidadesUseCase simularTodos, ListarSimulacoesRealizadasPaginadasUseCase listarSimulacoesRealizadasPaginadasUseCase, ListarAgregadoSimulacaoPorProdutoDataUseCase listarAgregadoSimulacaoPorProdutoDataUseCase, ListarTelemetriaEndpointUseCase listarTelemetriaEndpointUseCase, ILogger<SimuladorController> logger)
        {
            _simularTodasModalidadesUseCase = simularTodos;
            _listarSimulacoesRealizadasPaginadasUseCase = listarSimulacoesRealizadasPaginadasUseCase;
            _listarAgregadoSimulacaoPorProdutoDataUseCase = listarAgregadoSimulacaoPorProdutoDataUseCase;
            _ListarTelemetriaEndpointUseCase = listarTelemetriaEndpointUseCase;
            _logger = logger;
        }

        [HttpPost("simulacao")]
        public async Task<IActionResult> simular([FromBody] PropostaDTO request)
        {
            try
            {
                var resultado = await _simularTodasModalidadesUseCase.ExecuteAsync(request);
                return Ok(resultado.toResponsePropostaDTO());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao Simular");
                return StatusCode(500, new { message = "Ocorreu um erro interno no servidor." });
            }
        }
        

        [HttpGet("simulacao")]
        public async Task<IActionResult> Get(
            [FromQuery] int pagina,
            [FromQuery] int tamanhoPagina)
        {
            try
            {
                var propostas = await _listarSimulacoesRealizadasPaginadasUseCase.ExecuteAsync(pagina, tamanhoPagina);
                return Ok(propostas);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao Obter Simulacao");
                return StatusCode(500, new { message = "Ocorreu um erro interno no servidor." });  
            }
        }

        [HttpGet("simulacao/relatorio")]
        public async Task<IActionResult> GetRelatorioPorDataProduto(
            [FromQuery] int codigoProduto,
            [FromQuery] int ano,
            [FromQuery] int mes,
            [FromQuery] int dia)
        {
            try
            {
                var data = new DateTime(ano, mes, dia);
                var propostas = await _listarAgregadoSimulacaoPorProdutoDataUseCase.ExecuteAsync(codigoProduto, data);
                return Ok(propostas);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao Obter Relatorio");
                return StatusCode(500, new { message = "Ocorreu um erro interno no servidor." });
            }
        }

        [HttpGet("simulacao/telemetria")]
        public async Task<IActionResult> GetTelemetria(
            [FromQuery] int ano,
            [FromQuery] int mes,
            [FromQuery] int dia)
        {
            try
            {
                var data = new DateTime(ano, mes, dia);
                var telemetria = await _ListarTelemetriaEndpointUseCase.ExecuteAsync(data);

                var resultadoFormatado = new
                {
                    dataReferencia = telemetria.dataReferencia.ToString("yyyy-MM-dd"),
                    telemetrias = telemetria.telemetrias.Select(t => new
                    {
                        t.nomeApi,
                        t.qtdRequisicoes,
                        t.tempoMedio,
                        t.tempoMinimo,
                        t.tempoMaximo,
                        percentualSucesso = Math.Round(Convert.ToDecimal(t.percentualSucesso), 2)
                    })
                };

                return Ok(resultadoFormatado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao Obter Relatorio");
                return StatusCode(500, new { message = "Ocorreu um erro interno no servidor." }); 
            }
        }
    }
}
