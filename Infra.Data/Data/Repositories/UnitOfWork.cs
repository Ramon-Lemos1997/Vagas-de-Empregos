using Domain.Interfaces.Infra;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infra.Data.Data.Repositories
{
    /// <summary>
    /// Implementação da interface de Unit of Work para gerenciar transações e repositórios.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;
        private readonly IVaga _vagas;

        public UnitOfWork(ApplicationDbContext context, IVaga vagas)
        {
            _context = context;
            _vagas = vagas;
        }


        //------------------------------------------------------------------------------------

        /// <summary>
        /// Instância de vagaRepository.
        /// </summary>
        public IVaga VagaRepository => _vagas;

        /// <summary>
        /// Salva todas as mudanças realizadas no contexto do banco de dados.
        /// </summary>
        /// <returns>O número de entidades adicionadas, modificadas ou excluídas no contexto.</returns>
        public async Task<int> SaveChanges()
        {
            try
            {
                int saveResult = await _context.SaveChangesAsync();
                return saveResult != 0 ? 1 : 0;
            }
            catch (DbUpdateException ex)
            {
                //Console.WriteLine("Erro ao salvar alterações no banco de dados:");
                //Console.WriteLine(ex.Message);
                //Console.WriteLine(ex.StackTrace);
                throw; 
            }
        }

        /// <summary>
        /// Inicia uma nova transação se não houver uma transação ativa.
        /// </summary>
        public async Task BeginTransaction()
        {
            if (_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }

        /// <summary>
        /// Confirma a transação ativa.
        /// </summary>
        public async Task Commit()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Desfaz a transação ativa.
        /// </summary>
        public async Task Rollback()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Libera os recursos utilizados pela UnitOfWork.
        /// </summary>
        public async void Dispose()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            await _context.DisposeAsync();
        }

        //------------------------------------------------------------------------------
    }
}
