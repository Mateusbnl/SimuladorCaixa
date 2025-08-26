using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SimuladorCaixa.Application.Repository;
using SimuladorCaixa.Core.Entidades;
using SimuladorCaixa.Infra.SqlServer.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorCaixa.Infra.Sqlite.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly SqlConnection _connection;

        const string _selectAllQuery = "SELECT * FROM PRODUTO";
        const string _selectByValorQuery = "SELECT * FROM PRODUTO WHERE VR_MINIMO <= @valor AND VR_MAXIMO >= @valor";

        public ProdutoRepository([FromServices] SqlConnectionFactory connectionFactory)
        {
            _connection = connectionFactory.CreateConnection();
        }

        public async Task<Produto> Get(decimal? valor)
        {
            await _connection.OpenAsync();
            using var command = _connection.CreateCommand();
            command.CommandText = _selectByValorQuery;
            command.Parameters.AddWithValue("@valor", valor);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var produto = new Produto(
                    reader.GetInt32(reader.GetOrdinal("CO_PRODUTO")),
                    reader.GetString(reader.GetOrdinal("NO_PRODUTO")),
                    reader.GetDecimal(reader.GetOrdinal("PC_TAXA_JUROS")),
                    reader.GetInt16(reader.GetOrdinal("NU_MINIMO_MESES")),

                    reader.IsDBNull(reader.GetOrdinal("NU_MAXIMO_MESES"))
                        ? Int16.MaxValue
                        : reader.GetInt16(reader.GetOrdinal("NU_MAXIMO_MESES")),

                    reader.GetDecimal(reader.GetOrdinal("VR_MINIMO")),

                    reader.IsDBNull(reader.GetOrdinal("VR_MAXIMO"))
                        ? Decimal.MaxValue
                        : reader.GetDecimal(reader.GetOrdinal("VR_MAXIMO"))
                );
                return produto;
            }
            return null;
        }

        public async Task<List<Produto>> GetAll()
        {
            var produtos = new List<Produto>();
            await _connection.OpenAsync();
            using var command = _connection.CreateCommand();
            command.CommandText = _selectAllQuery;

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var produto = new Produto(
                    reader.GetInt32(reader.GetOrdinal("CO_PRODUTO")),
                    reader.GetString(reader.GetOrdinal("NO_PRODUTO")),
                    reader.GetDecimal(reader.GetOrdinal("PC_TAXA_JUROS")),
                    reader.GetInt16(reader.GetOrdinal("NU_MINIMO_MESES")),

                    reader.IsDBNull(reader.GetOrdinal("NU_MAXIMO_MESES"))
                        ? Int16.MaxValue
                        : reader.GetInt16(reader.GetOrdinal("NU_MAXIMO_MESES")),

                    reader.GetDecimal(reader.GetOrdinal("VR_MINIMO")),

                    reader.IsDBNull(reader.GetOrdinal("VR_MAXIMO"))
                        ? Decimal.MaxValue
                        : reader.GetDecimal(reader.GetOrdinal("VR_MAXIMO"))
                );
                produtos.Add(produto);
            }
            return produtos;
        }

        public Task<Produto> GetPorId(int id)
        {
            throw new NotImplementedException();
        }
    }
}

