using Dominio.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositoryAsync<T> RepositoryAsync<T>() where T : class;
        Task commit(CancellationToken cancellationToken);

        Task Rollback();

        Task<T> IUnitAddAsync<T>(T entity, CancellationToken cancellation) where T : class;

        Task<T> IUnitUpdateAsync<T>(T entity, CancellationToken cancellation) where T : class;

        Task<T> IUnitDeleteAsync<T>(T entity, CancellationToken cancellation) where T : class;



    }
}
