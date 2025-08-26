using SimuladorCaixa.Application.DTO;
using SimuladorCaixa.Core.Entidades;

namespace SimuladorCaixa.Application.Repository
{
    public interface IRelatorioRepository
    {
        public Task salvarProposta(Proposta proposta);
        public Task<PropostaResumoDTO> consultarVolumePropostasPorProdutoData(int codigoProduto, DateTime data);
    }
}