using MongoDB.Bson;
using MongoDB.Driver;
using SimuladorCaixa.Application.DTO;
using SimuladorCaixa.Application.Repository;
using SimuladorCaixa.Core.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimuladorCaixa.Infra.MongoDB.Repository
{
    internal class RelatorioRepository : IRelatorioRepository
    {
        private readonly IMongoCollection<Proposta> _collection;
        private readonly IProdutoRepository _produtoRepository;

        public RelatorioRepository(IMongoDatabase database, IProdutoRepository produtoRepository)
        {
            _collection = database.GetCollection<Proposta>("propostas");
            _produtoRepository = produtoRepository;
        }

        // Substitua os trechos do pipeline de agregação para calcular corretamente os valores das prestações.
        // O problema ocorre porque "$avg" e "$sum" não funcionam diretamente em arrays aninhados.
        // Use "$unwind" para desmembrar os arrays antes de agrupar e calcular.

        public async Task<PropostaResumoDTO> consultarVolumePropostasPorProdutoData(int codigoProduto, DateTime data)
        {
            // Plano:
            // 1. Definir dataInicio e dataFim para filtrar propostas do dia.
            // 2. Filtrar propostas pelo código do produto e data.
            // 3. Desmembrar simulacoes e prestacoes usando $unwind.
            // 4. Agrupar por produto e data (fixa pelo parâmetro).
            // 5. Calcular valorTotal (soma dos valores das propostas), prestacaoMedia (média das prestações), 
            //    taxaMediaJuros (média das taxas de juros do produto), valorTotalParcelas (soma das parcelas).
            // 6. Montar DTO de retorno.

            var dataInicio = data.Date;
            var dataFim = dataInicio.AddDays(1);

            // Substitua as linhas de Unwind incorretas por chamadas corretas usando o método de extensão .Unwind(string fieldName).
            // O método .Unwind<TDocument, TResult>(Expression<Func<TDocument, object>> field) espera uma expressão, não uma string.
            // Para usar o nome do campo como string, utilize o método .Unwind(string fieldName).

            var aggregate = _collection.Aggregate()
                .Match(p => p.produto.codigo == codigoProduto && p.dataCriacao >= dataInicio && p.dataCriacao < dataFim)
                .Unwind("simulacoes")
                .Unwind("simulacoes.prestacoes")
                .Group(new BsonDocument
                {
                    { "_id", new BsonDocument
                        {
                            { "produto", "$produto.codigo" },
                            { "dataCriacao", dataInicio.ToString("yyyy-MM-dd") }
                        }
                    },
                    { "valorTotal", new BsonDocument { { "$sum", "$valor" } } },
                    { "prestacaoMedia", new BsonDocument { { "$avg", "$simulacoes.prestacoes.valorPrestacao" } } },
                    { "taxaMediaJuros", new BsonDocument { { "$avg", "$produto.taxaJuros" } } },
                    { "valorTotalParcelas", new BsonDocument { { "$sum", "$simulacoes.prestacoes.valorPrestacao" } } }
                })
                .Project(new BsonDocument
                {
                    { "produto", "$_id.produto" },
                    { "dataCriacao", "$_id.dataCriacao" },
                    { "valorTotal", 1 },
                    { "prestacaoMedia", 1 },
                    { "taxaMediaJuros", 1 },
                    { "valorTotalParcelas", 1 }
                });

            var result = await aggregate.FirstOrDefaultAsync();

            if (result == null)
                return null;

            return new PropostaResumoDTO
            {
                dataReferencia = DateTime.Parse(result.GetValue("dataCriacao", "").AsString),
                simulacoes = new List<ResumoDTO>
                {
                    new ResumoDTO
                    {
                        codigoProduto = result.GetValue("produto", 0).AsInt32,
                        descricaoProduto = (await _produtoRepository.GetPorId(codigoProduto)).nome,
                        valorTotalDesejado = Math.Round(result.GetValue("valorTotal", 0).ToDecimal(),2),
                        valorMedioPrestacao = Math.Round(result.GetValue("prestacaoMedia", 0).ToDecimal(), 2),
                        valorTotalCredito = Math.Round(result.GetValue("valorTotalParcelas", 0).ToDecimal(), 2),
                        taxaMediaJuro = result.GetValue("taxaMediaJuros", 0).ToDecimal()
                    }
                }
            };
        }

        public async Task salvarProposta(Proposta proposta)
        {
            await _collection.InsertOneAsync(proposta);
        }
    }
}
