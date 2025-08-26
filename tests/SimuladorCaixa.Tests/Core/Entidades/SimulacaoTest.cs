using SimuladorCaixa.Core.Builders;
using SimuladorCaixa.Core.Entidades;
using SimuladorCaixa.Core.Enums;

namespace SimuladorCaixa.Core.Tests.Entidades
{
    public class SimulacaoTests
    {
        [Fact]
        public void CalculaParcelas_DeveRetornarQuantidadeCorretaDeParcelas_PRICE()
        {
            // Arrange
            var simulacao = new SimulacaoBuilder()
                .ComValorFinanciado(12000m)
                .ComPrazo(12)
                .ComTaxaJuros(6m)
                .Build();
            // Act
            var parcelas = simulacao.calculaParcelas(ModalidadeEnum.PRICE);

            // Assert
            Assert.Equal(simulacao.prazo, parcelas.Count);
        }

        [Fact]
        public void CalculaParcelas_DeveRetornarQuantidadeCorretaDeParcelas_SAC()
        {
            // Arrange
            var simulacao = new SimulacaoBuilder()
                .ComValorFinanciado(900m)
                .ComPrazo(5)
                .ComTaxaJuros(0.00179m)
                .Build();

            // Act
            var parcelas = simulacao.calculaParcelas(ModalidadeEnum.SAC);

            // Assert
            Assert.Equal(simulacao.prazo, parcelas.Count);
        }

        [Fact]
        public void CalculaParcelasPrice_DeveCalcularValoresCorretos()
        {
            // Arrange
            var simulacao = new SimulacaoBuilder()
                .ComValorFinanciado(900m)
                .ComPrazo(5)
                .ComTaxaJuros(0.0179m)
                .Build();


            // Act
            var parcelas = simulacao.calculaParcelas(ModalidadeEnum.PRICE);

            // Assert

            Assert.True(parcelas.ElementAt(2).valorPrestacao == 189.78m);
            Assert.True(parcelas.ElementAt(2).valorAmortizacao == 179.94m);
            Assert.True(parcelas.ElementAt(2).valorJuros == 9.84m);
        }

        [Fact]
        public void CalculaParcelasSAC_DeveCalcularValoresCorretos()
        {
            // Arrange
            var simulacao = new SimulacaoBuilder()
                .ComValorFinanciado(900m)
                .ComPrazo(5)
                .ComTaxaJuros(0.0179m)
                .Build();

            // Act
            var parcelas = simulacao.calculaParcelas(ModalidadeEnum.SAC);
            // Assert

            Assert.True(parcelas.ElementAt(2).valorPrestacao == 189.67m);
            Assert.True(parcelas.ElementAt(2).valorAmortizacao == 180.00m);
            Assert.True(parcelas.ElementAt(2).valorJuros == 9.67m);
            
        }

        [Fact]
        public void CalculaParcelas_DeveGerarDatasDeVencimentoCorretas()
        {
            // Arrange
            var simulacao = new SimulacaoBuilder()
                .ComValorFinanciado(12000m)
                .ComPrazo(12)
                .ComTaxaJuros(6m)
                .Build();
            var hoje = DateTime.Now;

            // Act
            var parcelas = simulacao.calculaParcelas(ModalidadeEnum.PRICE);

            // Assert
            for (int i = 0; i < simulacao.prazo; i++)
            {
                Assert.Equal(hoje.AddMonths(i + 1).Date, parcelas[i].dataVencimento.Date);
            }
        }
    }
}
