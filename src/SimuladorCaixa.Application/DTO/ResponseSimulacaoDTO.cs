using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorCaixa.Application.DTO
{
    public class ResponseSimulacaoDTO
    {
        public int idSimulacao { get; set; }
        public int codigoProduto { get; set; }
        public string descricaoProduto { get; set; }
        public decimal taxaJuros { get; set; }
        public List<SimulacaoDTO> resultadoSimulacao { get; set; }
    }

    public class SimulacaoDto
    {
        public string tipo { get; set; }
        public List<ParcelaDTO> parcelas { get; set; }
    }

    public class ParcelaDto
    {
        public int numero { get; set; }
        [DecimalFormat("0.00")]
        public decimal valorAmortizacao {  get; set; }
        [DecimalFormat("0.00")]
        public decimal valorJuros {  get; set; }
        [DecimalFormat("0.00")]
        public decimal valorPrestacao {  get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DecimalFormatAttribute : Attribute
    {
        public string Format { get; }

        public DecimalFormatAttribute(string format)
        {
            Format = format;
        }
    }
}
