using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorCaixa.Application.DTO
{
    public class ResponsePropostaDTO
    {
        public ResponsePropostaDTO(int idSimulacao, int codigoProduto, string descricaoProduto, decimal taxaJuros, List<SimulacaoDTO> resultadoSimulacao)
        {
            this.idSimulacao = idSimulacao;
            this.codigoProduto = codigoProduto;
            this.descricaoProduto = descricaoProduto;
            this.taxaJuros = taxaJuros;
            this.resultadoSimulacao = resultadoSimulacao;
        }

        public int idSimulacao { get; private set; }
        public int codigoProduto { get; private set; }
        public string descricaoProduto { get; private set; }
        public decimal taxaJuros { get; private set; }
        public List<SimulacaoDTO> resultadoSimulacao { get; private set; }
    }

    public class SimulacaoDTO
    {
        public string tipo { get; set; }
        public List<ParcelaDTO> parcelas { get; set; } 
    }

    public class ParcelaDTO
    {
        public int numero { get; set; }
        public decimal valorAmortizacao { get; set; }
        public decimal valorJuros { get; set; }
        public decimal valorPrestacao { get; set; }
    }
}
