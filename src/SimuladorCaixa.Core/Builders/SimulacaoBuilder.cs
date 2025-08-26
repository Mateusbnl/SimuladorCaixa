namespace SimuladorCaixa.Core.Builders
{
    using SimuladorCaixa.Core.Entidades;
    using SimuladorCaixa.Core.Enums;
    using System.Collections.Generic;

    public class SimulacaoBuilder
    {
        private int _id;
        private decimal _valorFinanciado;
        private int _prazo;
        private decimal _taxaJuros;
        private decimal _valorTotalFinanciado;
        private List<Prestacao> _prestacoes = new();
        private ModalidadeEnum _modalidade;

        public SimulacaoBuilder ComId(int id)
        {
            _id = id;
            return this;
        }

        public SimulacaoBuilder ComValorFinanciado(decimal valorFinanciado)
        {
            _valorFinanciado = valorFinanciado;
            return this;
        }

        public SimulacaoBuilder ComPrazo(int prazo)
        {
            _prazo = prazo;
            return this;
        }

        public SimulacaoBuilder ComTaxaJuros(decimal taxaJuros)
        {
            _taxaJuros = taxaJuros;
            return this;
        }

        public SimulacaoBuilder ComValorTotalFinanciado(decimal valorTotalFinanciado)
        {
            _valorTotalFinanciado = valorTotalFinanciado;
            return this;
        }

        public SimulacaoBuilder ComModalidade(ModalidadeEnum modalidade)
        {
            _modalidade = modalidade;
            return this;
        }

        public Simulacao Build()
        {
            return new Simulacao(
                _id,
                _valorFinanciado,
                _prazo,
                _taxaJuros,
                _valorTotalFinanciado,
                _prestacoes,
                _modalidade
            );
        }
    }
}
