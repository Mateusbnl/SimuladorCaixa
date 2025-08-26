using SimuladorCaixa.Core.Entidades;

namespace SimuladorCaixa.Core.Builders
{
    public class PropostaBuilder
    {
        private int _id;
        private int _prazo;
        private decimal _valor;
        private Produto _produto;
        private List<Simulacao> _simulacoes = new();

        public PropostaBuilder ComId(int id)
        {
            _id = id;
            return this;
        }

        public PropostaBuilder ComPrazo(int prazo)
        {
            _prazo = prazo;
            return this;
        }

        public PropostaBuilder ComValor(decimal valor)
        {
            _valor = valor;
            return this;
        }

        public PropostaBuilder ComProduto(Produto produto)
        {
            _produto = produto;
            return this;
        }

        public PropostaBuilder ComSimulacoes(List<Simulacao> simulacoes)
        {
            _simulacoes = simulacoes;
            return this;
        }

        public PropostaBuilder AdicionarSimulacao(Simulacao simulacao)
        {
            _simulacoes.Add(simulacao);
            return this;
        }

        public Proposta Build()
        {
            return new Proposta(_id, _prazo, _valor, _produto, _simulacoes);
        }
    }
}

