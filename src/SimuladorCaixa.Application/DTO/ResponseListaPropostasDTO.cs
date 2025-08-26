using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorCaixa.Application.DTO
{
    public class ResponseListaPropostasDTO
    {
        public int pagina{ get; set; }
        public int qtdRegistros{ get; set; }
        public int qtdRegistrosPagina{ get; set; }
        public List<RegistroDTO> registros{ get; set; }
    }

    public class RegistroDTO
    {
        public int idSimulacao { get; set; }
        public decimal valorDesejado { get; set; }
        public int prazo { get; set; }
        public decimal valorTotalParcelas { get; set; }
    }
}
