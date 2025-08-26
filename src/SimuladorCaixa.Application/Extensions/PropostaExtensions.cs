using SimuladorCaixa.Application.DTO;
using SimuladorCaixa.Core.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorCaixa.Application.Extensions
{
    public static class PropostaExtensions
    {
        public static ResponsePropostaDTO toResponsePropostaDTO(this Proposta proposta)
        {
            return new ResponsePropostaDTO(
                proposta.id,
                proposta.produto.codigo,
                proposta.produto.nome,
                proposta.produto.taxaJuros,
                proposta.simulacoes.Select(s => new SimulacaoDTO
                {
                    tipo = s.modalidade.ToString(),
                    parcelas = s.prestacoes.Select(p => new ParcelaDTO
                    {
                        numero = p.numero,
                        valorAmortizacao = p.valorAmortizacao,
                        valorJuros = p.valorJuros,
                        valorPrestacao = p.valorPrestacao
                    }).ToList()
                }).ToList()
            );
        }

        public static RegistroDTO toRegistroDTO(this Proposta proposta)
        {
            return new RegistroDTO
            {
                idSimulacao = proposta.id,
                prazo = proposta.prazo,
                valorDesejado = proposta.valor,
                valorTotalParcelas = proposta.simulacoes.Sum(s => s.prestacoes.Sum(p => p.valorPrestacao))
            };
        }
    }
}
