using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using SimuladorCaixa.Application.DTO;
using SimuladorCaixa.Application.Repository;
using SimuladorCaixa.Core.Builders;
using SimuladorCaixa.Core.Entidades;
using System.Text.Json;

namespace SimuladorCaixa.Application.UseCases
{
    public class SimularTodasModalidadesUseCase
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IPropostaRepository _propostaRepository;
        private readonly IRelatorioRepository _relatorioRepository;
        private readonly ISimulacaoRepository _simulacaoRepository;
        private readonly EventHubProducerClient _eventHubProducerClient;

        public SimularTodasModalidadesUseCase(
            IProdutoRepository produtoRepository,
            IPropostaRepository propostaRepository,
            IRelatorioRepository relatorioRepository,
            ISimulacaoRepository simulacaoRepository,
            EventHubProducerClient eventHubProducerClient)
        {
            _produtoRepository = produtoRepository;
            _propostaRepository = propostaRepository;
            _relatorioRepository = relatorioRepository;
            _simulacaoRepository = simulacaoRepository;
            _eventHubProducerClient = eventHubProducerClient;
        }

        public async Task<Proposta> ExecuteAsync(PropostaDTO propostaDTO)
        {
            try
            {
                var proposta = new PropostaBuilder()
                    .ComId(0)
                    .ComPrazo(propostaDTO.prazo)
                    .ComValor(propostaDTO.valorDesejado)
                    .ComProduto(await _produtoRepository.Get(propostaDTO.valorDesejado))
                    .ComSimulacoes(new List<Simulacao>())
                    .Build();

                if (proposta.IsValid())
                {
                    List<Simulacao> simulacoesExistentes = await BuscarSimulacoesAsync(proposta);

                    if (simulacoesExistentes != null && simulacoesExistentes.Count > 0)
                    {
                        proposta.simulacoes = simulacoesExistentes;
                        await SalvarPropostaAsync(proposta);
                        SalvarParaRelatorio(proposta);
                        EnviarEventoPropostaAsync(proposta);
                        return proposta;
                    }

                    var modalidades = Enum.GetValues(typeof(SimuladorCaixa.Core.Enums.ModalidadeEnum))
                           .Cast<SimuladorCaixa.Core.Enums.ModalidadeEnum>();

                    GerarSimulacoesPorModalidade(proposta, modalidades);

                    await SalvarPropostaAsync(proposta);
                    SalvarSimulacoes(proposta);
                    SalvarParaRelatorio(proposta);
                    EnviarEventoPropostaAsync(proposta);
                    return proposta;
                }
                else
                {
                    throw new ArgumentException("Proposta inválida: Verifique os valores de prazo e valor.");
                }
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new Exception("Erro ao simular todas as modalidades");
            }
        }

        private static void GerarSimulacoesPorModalidade(Proposta proposta, IEnumerable<Core.Enums.ModalidadeEnum> modalidades)
        {
            foreach (var modalidade in modalidades)
            {
                var simulacao = new SimulacaoBuilder()
                    .ComId(0)
                    .ComValorFinanciado(proposta.valor)
                    .ComPrazo(proposta.prazo)
                    .ComTaxaJuros(proposta.produto.taxaJuros)
                    .ComValorTotalFinanciado(proposta.valor)
                    .ComModalidade(modalidade)
                    .Build();

                proposta.simulacoes.Add(simulacao);
            }
        }

        private async Task SalvarSimulacoes(Proposta proposta)
        {
            await _simulacaoRepository.SalvarSimulacao(proposta.prazo, proposta.valor, proposta.produto.taxaJuros, proposta.simulacoes);
        }

        private async Task SalvarParaRelatorio(Proposta proposta)
        {
            await _relatorioRepository.salvarProposta(proposta);
        }

        private async Task SalvarPropostaAsync(Proposta proposta)
        {
            var id = await _propostaRepository.salvarProposta(proposta);
            proposta.id = id;
        }

        private async Task<List<Simulacao>> BuscarSimulacoesAsync(Proposta proposta)
        {
            return await _simulacaoRepository.GetSimulacao(proposta.prazo, proposta.valor, proposta.produto.taxaJuros);
        }

        private async Task EnviarEventoPropostaAsync(Proposta proposta)
        {
            var evento = new
            {
                propostaId = proposta.id,
                prazo = proposta.prazo,
                valor = proposta.valor,
                produto = proposta.produto?.nome,
                dataCriacao = proposta.dataCriacao,
                simulacoes = proposta.simulacoes.Select(s => new
                {
                    modalidade = s.modalidade.ToString(),
                    valorFinanciado = s.valorFinanciado,
                    prazo = s.prazo,
                    taxaJuros = s.taxaJuros,
                    valorTotalFinanciado = s.valorTotalFinanciado
                }).ToList()
            };

            string jsonEvento = JsonSerializer.Serialize(evento);
            using EventDataBatch eventBatch = await _eventHubProducerClient.CreateBatchAsync();
            eventBatch.TryAdd(new EventData(System.Text.Encoding.UTF8.GetBytes(jsonEvento)));
            await _eventHubProducerClient.SendAsync(eventBatch);
        }
    }
}
