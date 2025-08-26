using SimuladorCaixa.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorCaixa.Core.Entidades
{
    public class Simulacao
    {
        public Simulacao(int id, decimal valorFinanciado, int prazo, decimal taxaJuros, decimal valorTotalFinanciado, List<Prestacao> prestacoes, ModalidadeEnum modalidade)
        {
            this.id = id;
            this.valorFinanciado = valorFinanciado;
            this.prazo = prazo;
            this.taxaJuros = taxaJuros;
            this.valorTotalFinanciado = valorTotalFinanciado;
            this.modalidade = modalidade;
            this.prestacoes = calculaParcelas(modalidade);
        }

        public int id { get; private set; }
        public decimal valorFinanciado { get; private set; }
        public int prazo { get; private set; }
        public decimal taxaJuros { get; private set; }
        public decimal valorTotalFinanciado { get; private set; }
        public List<Prestacao> prestacoes { get; private set; }
        public ModalidadeEnum modalidade { get; private set; }

        public List<Prestacao> calculaParcelas(ModalidadeEnum modalidadeEnum)
        {
            if(modalidadeEnum == ModalidadeEnum.PRICE)
            {
                return calculaParcelasPrice(this.valorFinanciado, this.prazo, this.taxaJuros);
            }
            else
            {
                return calculaParcelasSAC(this.valorFinanciado, this.prazo, this.taxaJuros);
            }
        }

        private List<Prestacao> calculaParcelasPrice(decimal valor, int prazo, decimal taxa)
        {
            List<Prestacao> parcelas = new List<Prestacao>();
            decimal taxaMensal = taxa;
            decimal fator = (decimal)Math.Pow((double)(1 + taxaMensal), prazo);
            decimal valorPrestacao = Math.Round(valor * taxaMensal * fator / (fator - 1), 2);

            decimal saldoDevedor = valor;
            for (int i = 1; i <= prazo; i++)
            {
                decimal juros = Math.Round(saldoDevedor * taxaMensal, 2);
                decimal amortizacao = Math.Round(valorPrestacao - juros, 2);
                parcelas.Add(new Prestacao
                {
                    numero = i,
                    valorAmortizacao = amortizacao,
                    valorJuros = juros,
                    valorPrestacao = valorPrestacao,
                    dataVencimento = DateTime.Now.AddMonths(i)
                });
                saldoDevedor -= amortizacao;
            }
            return parcelas;
        }

        private List<Prestacao> calculaParcelasSAC(decimal valor, int prazo, decimal taxa)
        {
            List<Prestacao> parcelas = new List<Prestacao>();
            decimal valorAmortizacao = Math.Round(valor / prazo, 2);
            decimal taxaMensal = taxa;
            for (int i = 1; i <= prazo; i++)
            {
                decimal juros = Math.Round((valor - (valorAmortizacao * (i - 1))) * taxaMensal, 2);
                decimal valorPrestacao = Math.Round(valorAmortizacao + juros, 2);
                parcelas.Add(new Prestacao
                {
                    numero = i,
                    valorAmortizacao = valorAmortizacao,
                    valorJuros = juros,
                    valorPrestacao = valorPrestacao,
                    dataVencimento = DateTime.Now.AddMonths(i)
                });
            }
            return parcelas;
        }


    }
}
