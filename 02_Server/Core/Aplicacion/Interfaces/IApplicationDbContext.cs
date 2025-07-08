using Dominio.Entities.Integracion;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Boleto> Boleto { get; }
        DbSet<Encomienda> Encomienda { get; }
        DbSet<Pago> Pago { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
