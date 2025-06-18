using Aplicacion.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Persistencia.Contexts;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Persistencia.Repository.Common.Aplicacion
{
    public class AppUnitOfWork : IUnitOfWork
    {
        private readonly AplicationDbContext _dbContext;
        private bool disposed;
        private Hashtable _repositories;
        private IDbContextTransaction _transaction;

        public AppUnitOfWork(AplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IRepositoryAsync<T> RepositoryAsync<T>() where T : class
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(AppRepositoryAsync<>);

                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _dbContext);

                _repositories.Add(type, repositoryInstance);
            }

            return (IRepositoryAsync<T>)_repositories[type];
        }

        public async Task commit(CancellationToken cancellationToken)
        {
            await _transaction.CommitAsync(cancellationToken);
        }

        public async Task Rollback()
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
            _dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                    if (_transaction != null)
                        _transaction.Dispose();

                    _dbContext.Dispose();
                }
            }
            //dispose unmanaged resources
            disposed = true;

        }

        public async Task<T> IUnitAddAsync<T>(T entity, CancellationToken cancellation = default) where T : class
        {
            if (_transaction == null)
                _transaction = await _dbContext.Database.BeginTransactionAsync(cancellation);

            await _dbContext.AddAsync(entity, cancellation);
            await _dbContext.SaveChangesAsync(cancellation);
            return entity;
        }

        public async Task<T> IUnitUpdateAsync<T>(T entity, CancellationToken cancellation = default) where T : class
        {
            if (_transaction == null)
                _transaction = await _dbContext.Database.BeginTransactionAsync(cancellation);

            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync(cancellation);
            return entity;
        }

        public async Task<T> IUnitDeleteAsync<T>(T entity, CancellationToken cancellation = default) where T : class
        {
            if (_transaction == null)
                _transaction = await _dbContext.Database.BeginTransactionAsync(cancellation);

            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellation);
            return entity;
        }
    }
}
