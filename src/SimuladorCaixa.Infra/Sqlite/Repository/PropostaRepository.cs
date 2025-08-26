using Microsoft.Data.Sqlite;
using SimuladorCaixa.Application.Repository;
using SimuladorCaixa.Core.Builders;
using SimuladorCaixa.Core.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimuladorCaixa.Infra.Sqlite.Repository
{
    public class PropostaRepository : IPropostaRepository
    {
        private const string _connectionString = "Data Source=localPropostas.db";
        private readonly IProdutoRepository _produtoRepository;

        public PropostaRepository(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
            CriarTabelaSeNaoExistir();
        }

        private void CriarTabelaSeNaoExistir()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Propostas (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Prazo INTEGER NOT NULL,
                    Valor REAL NOT NULL,
                    ProdutoCodigo INTEGER NOT NULL,
                    SimulacoesJson TEXT NOT NULL
                );
                ";
            cmd.ExecuteNonQuery();
        }

        public async Task<List<Proposta>> consultarPropostas(int pagina, int tamanhoPagina)
        {
            var propostas = new List<Proposta>();
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT * FROM Propostas
                LIMIT @TamanhoPagina OFFSET @Offset
            ";
            cmd.Parameters.AddWithValue("@TamanhoPagina", tamanhoPagina);
            cmd.Parameters.AddWithValue("@Offset", (pagina - 1) * tamanhoPagina);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var simulacoesJson = reader.GetString(reader.GetOrdinal("SimulacoesJson"));
                var simulacoes = JsonSerializer.Deserialize<List<Simulacao>>(simulacoesJson) ?? new List<Simulacao>();

                var proposta = new PropostaBuilder()
                                .ComId(reader.GetInt32(reader.GetOrdinal("Id")))
                                .ComPrazo(reader.GetInt32(reader.GetOrdinal("Prazo")))
                                .ComValor(reader.GetDecimal(reader.GetOrdinal("Valor")))
                                .ComSimulacoes(simulacoes)
                                .ComProduto(await _produtoRepository.GetPorId(reader.GetInt32(reader.GetOrdinal("ProdutoCodigo"))))
                                .Build();

                propostas.Add(proposta);
            }

            return propostas;
        }

        public async Task<int> salvarProposta(Proposta proposta)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var simulacoesJson = JsonSerializer.Serialize(proposta.simulacoes);

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Propostas (
                    Prazo, Valor, ProdutoCodigo, SimulacoesJson
                ) VALUES (
                    @Prazo, @Valor, @ProdutoCodigo, @SimulacoesJson
                );
                SELECT last_insert_rowid();
            ";

            cmd.Parameters.AddWithValue("@Prazo", proposta.prazo);
            cmd.Parameters.AddWithValue("@Valor", proposta.valor);
            cmd.Parameters.AddWithValue("@ProdutoCodigo", proposta.produto.codigo);
            cmd.Parameters.AddWithValue("@SimulacoesJson", simulacoesJson);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public Task<int> consultarTotalPropostas()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Propostas";
            var result = cmd.ExecuteScalar();
            return Task.FromResult(Convert.ToInt32(result));
        }

    }
}
