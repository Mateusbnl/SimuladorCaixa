using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorCaixa.Core.Entidades
{
    public class Proposta
    {
        public Proposta(int id, int prazo, decimal valor, Produto produto, List<Simulacao> simulacoes)
        {
            this.id = id;
            this.prazo = prazo;
            this.valor = valor;
            this.produto = produto;
            this.simulacoes = simulacoes;
        }

        public int id { get; set; }
        public int prazo { get; private set; }
        public decimal valor { get; private set; }
        public Produto produto { get; private set; }
        public List<Simulacao> simulacoes { get; set; }
       
        public DateTime dataCriacao = DateTime.Now;

        public bool IsValid()
        {
            if(prazo == 0 || valor == 0)
                return false;
            if (prazo < produto.prazoMinimo || prazo > produto.prazoMaximo)
                return false;
            if (valor < produto.valorMinimo || valor > produto.valorMaximo)
                return false;
            return true;
        }
    }
}
