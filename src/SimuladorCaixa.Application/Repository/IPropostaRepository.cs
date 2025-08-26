using SimuladorCaixa.Core.Entidades;


namespace SimuladorCaixa.Application.Repository
{
    public interface IPropostaRepository
    {
        public Task<int> salvarProposta(Proposta proposta);
        public Task<List<Proposta>> consultarPropostas(int pagina, int tamanhoPagina);
        public Task<int> consultarTotalPropostas();
    }
}
