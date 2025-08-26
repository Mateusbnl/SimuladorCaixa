using SimuladorCaixa.Core.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorCaixa.Application.Repository
{
    public interface IProdutoRepository
    {
        public Task<Produto> Get(decimal? valor);
        public Task<Produto> GetPorId(int id);
        public Task<List<Produto>> GetAll();
    }
}
