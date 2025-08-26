using SimuladorCaixa.Application.DTO;
using SimuladorCaixa.Application.Repository;
using SimuladorCaixa.Core.Enums;

namespace SimuladorCaixa.Application.UseCases
{
    public class ListarSimulacoesRealizadasPaginadasUseCase
    {
        private readonly IPropostaRepository _propostaRepository;

        public ListarSimulacoesRealizadasPaginadasUseCase(IPropostaRepository propostaRepository)
        {
            _propostaRepository = propostaRepository;
        }

        public async Task<ResponseListaPropostasDTO> ExecuteAsync(int pagina, int tamanhoPagina) {
            var totalPropostas = await _propostaRepository.consultarTotalPropostas();
            var propostas = await _propostaRepository.consultarPropostas(pagina, tamanhoPagina);
            var response = new ResponseListaPropostasDTO
            {
                pagina = pagina,
                qtdRegistros = totalPropostas,
                qtdRegistrosPagina = propostas.Count,
                registros = new List<RegistroDTO>()
            };
            foreach (var proposta in propostas)
            {
                response.registros.Add(new RegistroDTO
                {
                    idSimulacao = proposta.id,
                    valorDesejado = proposta.valor,
                    prazo = proposta.prazo,
                    valorTotalParcelas = proposta.simulacoes.Where(x => x.modalidade == ModalidadeEnum.SAC).Sum(x => x.prestacoes.Sum(p => p.valorPrestacao))
                });
            }
            return response;
        }
    }
}
