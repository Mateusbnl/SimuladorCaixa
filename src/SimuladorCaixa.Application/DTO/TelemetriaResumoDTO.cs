using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorCaixa.Application.DTO
{
    public class TelemetriaTotalResumoDTO()
    {
        public DateTime dataReferencia;
        public List<TelemetriaResumoDTO> telemetrias;
    }

    public class TelemetriaResumoDTO()
    {
        public string nomeApi;
        public int qtdRequisicoes;
        public int tempoMedio;
        public int tempoMinimo;
        public int tempoMaximo;
        public decimal percentualSucesso;
    }
}
