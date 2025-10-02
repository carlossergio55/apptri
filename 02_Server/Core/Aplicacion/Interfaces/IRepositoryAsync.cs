using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Interfaces
{
    public interface IRepositoryAsync<T> : IRepositoryBase<T> where T : class
    {
        Task<List<T>> CallFunctionReFCursor(string nameFunction, params object[] param);

        // ✅ Corregido: predicado sobre T
        Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken ct);
    }

    public interface IReadRepositoryAsync<T> : IReadRepositoryBase<T> where T : class
    {
    }
}
