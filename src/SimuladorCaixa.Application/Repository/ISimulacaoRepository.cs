using SimuladorCaixa.Core.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorCaixa.Application.Repository
{
    public interface ISimulacaoRepository
    {
        public Task SalvarSimulacao(int prazo ,decimal valorDesejado, decimal taxaJuro, List<Simulacao> simulacoes);
        public Task<List<Simulacao>> GetSimulacao(int prazo, decimal valorDesejado, decimal taxaJuro);
    }
}
