using SimuladorCaixa.Application.DTO;
using SimuladorCaixa.Application.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorCaixa.Application.UseCases
{
    public class ListarAgregadoSimulacaoPorProdutoDataUseCase
    {
        private readonly IRelatorioRepository _relatorioRepository;
        public ListarAgregadoSimulacaoPorProdutoDataUseCase(IRelatorioRepository relatorioRepository)
        {
            _relatorioRepository = relatorioRepository;
        }
        public async Task<PropostaResumoDTO> ExecuteAsync(int codigoProduto, DateTime data)
        {
           return await _relatorioRepository.consultarVolumePropostasPorProdutoData(codigoProduto,data);
        }
    }
}
