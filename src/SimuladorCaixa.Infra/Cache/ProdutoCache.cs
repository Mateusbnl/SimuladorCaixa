using Microsoft.Extensions.Caching.Memory;
using SimuladorCaixa.Application.Repository;
using SimuladorCaixa.Core.Entidades;
using SimuladorCaixa.Infra.Sqlite.Repository;

namespace SimuladorCaixa.Infra.Cache
{
    public class CachedProdutoRepository : IProdutoRepository
    {
        private readonly ProdutoRepository _produtoRepository;
        private readonly IMemoryCache _memoryCache;
        private const string CacheKey = "Produtos";

        public CachedProdutoRepository(ProdutoRepository produtoRepository, IMemoryCache memoryCache)
        {
            _produtoRepository = produtoRepository;
            _memoryCache = memoryCache;
        }

        public async Task<List<Produto>> GetAll()
        {
            return _memoryCache.GetOrCreate(CacheKey, entry =>
            {
                entry.SetOptions(new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(40)));
                return _produtoRepository.GetAll().Result;
            })!;
        }

        public async Task<Produto> Get(decimal? valor)
        {
            // Tenta obter a lista de produtos do cache
            if (!_memoryCache.TryGetValue(CacheKey, out List<Produto>? produtos))
            {
                produtos = await _produtoRepository.GetAll();
                _memoryCache.Set(CacheKey, produtos, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(40)
                });
            }

            // Procura o produto que atende ao valor
            var produto = produtos
                .Where(x => valor >= x.valorMinimo && valor <= x.valorMaximo)
                .FirstOrDefault();

            if (produto == null)
                throw new ArgumentException("Nenhum produto encontrado para o valor informado.");

            return produto;
        }

        public async Task<Produto> GetPorId(int codigo)
        {
            // Tenta obter a lista de produtos do cache
            if (!_memoryCache.TryGetValue(CacheKey, out List<Produto>? produtos))
            {
                produtos = await _produtoRepository.GetAll();
                _memoryCache.Set(CacheKey, produtos, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(40)
                });
            }

            // Procura o produto que atende ao valor
            var produto = produtos
                .Where(x => x.codigo == codigo)
                .FirstOrDefault();

            if (produto == null)
                throw new InvalidOperationException("Nenhum produto encontrado para o valor informado.");

            return produto;
        }
    }
}
