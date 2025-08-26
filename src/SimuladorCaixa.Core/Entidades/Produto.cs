using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorCaixa.Core.Entidades
{
    public class Produto
    {
        public Produto(int codigo, string nome, decimal taxaJuros, int prazoMinimo, int prazoMaximo, decimal valorMinimo, decimal valorMaximo)
        {
            this.codigo = codigo;
            this.nome = nome;
            this.taxaJuros = taxaJuros;
            this.prazoMinimo = prazoMinimo;
            this.prazoMaximo = prazoMaximo;
            this.valorMinimo = valorMinimo;
            this.valorMaximo = valorMaximo;
        }

        public Produto() { 
        }

        public int codigo { get; private set; }
        public string nome { get; private set; }
        public decimal taxaJuros { get; private set; }
        public int prazoMinimo { get; private set; }
        public int? prazoMaximo { get; private set; }
        public decimal valorMinimo { get; private set; }
        public decimal valorMaximo { get; private set; }
    }
}
