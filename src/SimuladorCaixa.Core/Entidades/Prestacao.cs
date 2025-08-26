using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorCaixa.Core.Entidades
{
    public class Prestacao
    {
        public int numero { get; set; }
        public decimal valorAmortizacao { get; set; }
        public decimal valorJuros { get; set; }
        public decimal valorPrestacao { get; set; }
        public DateTime dataVencimento { get; set; }
    }
}
