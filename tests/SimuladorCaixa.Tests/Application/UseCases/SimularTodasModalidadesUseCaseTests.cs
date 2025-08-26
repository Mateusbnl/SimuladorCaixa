using Moq;
using SimuladorCaixa.Application.DTO;
using SimuladorCaixa.Application.Repository;
using SimuladorCaixa.Application.UseCases;
using SimuladorCaixa.Core.Entidades;

namespace SimuladorCaixa.Application.Tests.UseCases
{
    public class SimularTodasModalidadesUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_DeveRetornarPropostaComSimulacoesParaTodasModalidades()
        {
            // Arrange
            var produtoMock = new Produto(1, "Produto Teste", 0.05m, 12, 60, 1000m, 50000m);
            var produtoRepositoryMock = new Mock<IProdutoRepository>();
            var relatorioRepositoryMock = new Mock<IRelatorioRepository>();
            var propostaRepositoryMock = new Mock<IPropostaRepository>();
            produtoRepositoryMock.Setup(r => r.Get(It.IsAny<decimal>())).ReturnsAsync(produtoMock);
            relatorioRepositoryMock.Setup(r => r.salvarProposta(It.IsAny<Proposta>())).Returns(Task.CompletedTask);
            propostaRepositoryMock.Setup(r => r.salvarProposta(It.IsAny<Proposta>())).ReturnsAsync(1);

            var propostaDTO = new PropostaDTO(20000m, 24);

            var useCase = new SimularTodasModalidadesUseCase(produtoRepositoryMock.Object, propostaRepositoryMock.Object, relatorioRepositoryMock.Object);

            // Act
            var proposta = await useCase.ExecuteAsync(propostaDTO);

            // Assert
            Assert.NotNull(proposta);
            Assert.True(proposta.IsValid());
            Assert.Equal(2, proposta.simulacoes.Count); // ModalidadeEnum tem 2 valores: SAC e PRICE
            Assert.Contains(proposta.simulacoes, s => s.modalidade == SimuladorCaixa.Core.Enums.ModalidadeEnum.SAC);
            Assert.Contains(proposta.simulacoes, s => s.modalidade == SimuladorCaixa.Core.Enums.ModalidadeEnum.PRICE);
        }


        [Fact]
        public async Task ExecuteAsync_DeveLancarArgumentExceptionParaValorNegativo()
        {
            // Arrange
            var produtoMock = new Produto(1, "Produto Teste", 0.05m, 12, 60, 1000m, 50000m);
            var produtoRepositoryMock = new Mock<IProdutoRepository>();
            var relatorioRepositoryMock = new Mock<IRelatorioRepository>();
            var propostaRepositoryMock = new Mock<IPropostaRepository>();
            produtoRepositoryMock.Setup(r => r.Get(It.IsAny<decimal>())).ReturnsAsync(produtoMock);
            relatorioRepositoryMock.Setup(r => r.salvarProposta(It.IsAny<Proposta>())).Returns(Task.CompletedTask);

            var propostaDTO = new PropostaDTO(-20000m, 24); // Valor negativo

            var useCase = new SimularTodasModalidadesUseCase(produtoRepositoryMock.Object, propostaRepositoryMock.Object, relatorioRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await useCase.ExecuteAsync(propostaDTO));
        }
        }
    }

