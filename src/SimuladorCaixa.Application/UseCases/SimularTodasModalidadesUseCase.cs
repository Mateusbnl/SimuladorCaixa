using SimuladorCaixa.Application.DTO;
using SimuladorCaixa.Application.Repository;
using SimuladorCaixa.Core.Builders;
using SimuladorCaixa.Core.Entidades;

namespace SimuladorCaixa.Application.UseCases
{
    public class SimularTodasModalidadesUseCase
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IPropostaRepository _propostaRepository;
        private readonly IRelatorioRepository _relatorioRepository;
        private readonly ISimulacaoRepository _simulacaoRepository;

        public SimularTodasModalidadesUseCase(IProdutoRepository produtoRepository, IPropostaRepository propostaRepository, IRelatorioRepository relatorioRepository, ISimulacaoRepository simulacaoRepository)
        {
            _produtoRepository = produtoRepository;
            _propostaRepository = propostaRepository;
            _relatorioRepository = relatorioRepository;
            _simulacaoRepository = simulacaoRepository;
        }

        public async Task<Proposta> ExecuteAsync(PropostaDTO propostaDTO)
        {
            try
            {
                //Cria Proposta
                var proposta = new PropostaBuilder()
                    .ComId(0) // ID será gerado pelo banco de dados
                    .ComPrazo(propostaDTO.prazo)
                    .ComValor(propostaDTO.valorDesejado)
                    .ComProduto(await _produtoRepository.Get(propostaDTO.valorDesejado))
                    .ComSimulacoes(new List<Simulacao>())
                    .Build();

                if (proposta.IsValid())
                {
                    //Busca simulacoes com mesmos parametros no redis (prazo, valor, taxaJuros)
                    List<Simulacao> simulacoesExistentes = await BuscarSimulacoesAsync(proposta);

                    if (simulacoesExistentes != null && simulacoesExistentes.Count > 0)
                    {
                        proposta.simulacoes = simulacoesExistentes;
                        SalvarPropostaAsync(proposta);
                        SalvarParaRelatorio(proposta);
                        return proposta;
                    }

                    //Para cada modalidade, cria uma simulação e adiciona na proposta
                    var modalidades = Enum.GetValues(typeof(SimuladorCaixa.Core.Enums.ModalidadeEnum))
                           .Cast<SimuladorCaixa.Core.Enums.ModalidadeEnum>();

                    GerarSimulacoesPorModalidade(proposta, modalidades);

                    //Salva Proposta
                    SalvarPropostaAsync(proposta);
                    SalvarSimulacoes(proposta);
                    SalvarParaRelatorio(proposta);
                    return proposta;
                }
                else
                {
                    throw new ArgumentException("Proposta inválida: Verifique os valores de prazo e valor.");
                }
            }
            catch (ArgumentException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao simular todas as modalidades");
            }
        }

        private static void GerarSimulacoesPorModalidade(Proposta proposta, IEnumerable<Core.Enums.ModalidadeEnum> modalidades)
        {
            //fiz um teste para fazer cada simulacao em paralelo, mas nao foi satisfatorio o desempenho
            foreach (var modalidade in modalidades)
            {
                var simulacao = new SimulacaoBuilder()
                    .ComId(0) // ID será gerado pelo banco de dados
                    .ComValorFinanciado(proposta.valor)
                    .ComPrazo(proposta.prazo)
                    .ComTaxaJuros(proposta.produto.taxaJuros)
                    .ComValorTotalFinanciado(proposta.valor)
                    .ComModalidade(modalidade)
                    .Build();

                proposta.simulacoes.Add(simulacao);
            }
        }

        private void SalvarSimulacoes(Proposta proposta)
        {
            _simulacaoRepository.SalvarSimulacao(proposta.prazo, proposta.valor, proposta.produto.taxaJuros, proposta.simulacoes);
        }

        private void SalvarParaRelatorio(Proposta proposta)
        {
            _relatorioRepository.salvarProposta(proposta);
        }

        private async Task SalvarPropostaAsync(Proposta proposta)
        {
            var id2 = await _propostaRepository.salvarProposta(proposta);
            proposta.id = id2;
        }

        private async Task<List<Simulacao>> BuscarSimulacoesAsync(Proposta proposta)
        {
            return await _simulacaoRepository.GetSimulacao(proposta.prazo, proposta.valor, proposta.produto.taxaJuros);
        }
    }
}
