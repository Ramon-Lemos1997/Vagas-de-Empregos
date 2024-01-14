using Domain.Entities;
using Domain.Interfaces.Infra;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;


namespace Infra.Data.Data.Repositories
{
    /// <summary>
    /// Repositório genérico para operações com entidades.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade.</typeparam>
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        //----------------------------------------------------------------------------------

        /// <summary>
        /// Adiciona uma nova entidade ao contexto.
        /// </summary>
        /// <param name="entity">Entidade a ser adicionada.</param>
        /// <param name="cancellationToken">Token de cancelamento (opcional).</param>
        /// <returns>A entidade adicionada.</returns>
        public async Task<T> Add(T entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        /// <summary>
        /// Inicia uma transação no contexto do banco de dados.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento (opcional).</param>
        /// <returns>A transação iniciada.</returns>
        public async Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken = default)
           => await _context.Database.BeginTransactionAsync(cancellationToken);

        /// <summary>
        /// Remove uma entidade do contexto.
        /// </summary>
        /// <param name="entity">Entidade a ser removida.</param>
        /// <param name="cancellationToken">Token de cancelamento (opcional).</param>
        /// <returns>Uma tarefa que representa a operação de remoção.</returns>
        public async Task Delete(T entity, CancellationToken cancellationToken = default)
            => _context.Set<T>().Remove(entity);

        /// <summary>
        /// Obtém uma entidade por ID.
        /// </summary>
        /// <param name="id">ID da entidade a ser obtida.</param>
        /// <param name="cancellationToken">Token de cancelamento (opcional).</param>
        /// <returns>A entidade encontrada ou null caso não seja encontrada.</returns>
        public async Task<T> GetById(int id, CancellationToken cancellationToken = default)
        {
            var keyValues = new object[] { id };
            return await _context.Set<T>().FindAsync(keyValues, cancellationToken) ?? Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Lista todas as entidades.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento (opcional).</param>
        /// <returns>Uma lista somente leitura das entidades encontradas.</returns>
        public async Task<IEnumerable<T>> ListAll(CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Atualiza uma entidade no contexto.
        /// </summary>
        /// <param name="entity">Entidade a ser atualizada.</param>
        /// <param name="cancellationToken">Token de cancelamento (opcional).</param>
        /// <returns>Uma tarefa que representa a operação de atualização.</returns>
        public async Task Update(T entity, CancellationToken cancellationToken = default)
             => _context.Entry(entity).State = EntityState.Modified;



        //-----------------------------------------------------------------------------------
    }
}
