using SimuladorCaixa.Application.DTO;
using SimuladorCaixa.Core.Entidades;
using SimuladorCaixa.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorCaixa.Application.Extensions
{
    public static class PropostaExtension
    {
        public static ResponseSimulacaoDTO toDTO(this Proposta proposta)
        {
            var response = new ResponseSimulacaoDTO();
            response.taxaJuros = proposta.produto.taxaJuros;
            response.descricaoProduto = proposta.produto.nome;
            response.codigoProduto = proposta.produto.codigo;
            response.idSimulacao = proposta.id;

            foreach(var simulacao in proposta.simulacoes)
            {
                var simulacaoDTO = new SimulacaoDTO();
                var listaParcelas = new List<ParcelaDTO>();
                if(simulacao.modalidade is ModalidadeEnum.SAC)
                {
                    simulacaoDTO.tipo = "SAC";
                } else
                {
                    simulacaoDTO.tipo = "PRICE";
                }
                foreach(var parcela in simulacao.prestacoes)
                {
                    var parcelaDTO = new ParcelaDTO();
                    parcelaDTO.numero = parcela.numero;
                    parcelaDTO.valorJuros = parcela.valorJuros;
                    parcelaDTO.valorPrestacao = parcela.valorPrestacao;
                    parcelaDTO.valorAmortizacao = parcela.valorAmortizacao;
                    listaParcelas.Add(parcelaDTO);
                }
                simulacaoDTO.parcelas = listaParcelas;
                response.resultadoSimulacao.Add(simulacaoDTO);
            }
            return response;
        }
    }
}
