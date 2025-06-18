using Aplicacion.Interfaces;
using Dominio.Common;
using Dominio.Settings;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Persistencia.Contexts
{
    public class AplicationDbContext : GenericContexDb
    {               
        private readonly IDateTimeService _dateTime;
        private readonly ICurrentUserService _user;
                         
        public AplicationDbContext(DbContextOptions<AplicationDbContext> options, IDateTimeService dateTime, ICurrentUserService user) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _dateTime = dateTime;
            _user = user;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                switch (entry.State)
                {                    
                    case EntityState.Modified:
                        entry.Entity.UsuarioModificacion = _user.LoginUsuario;
                        entry.Entity.FechaModificacion = _dateTime.Now;
                        break;
                    case EntityState.Added:
                        entry.Entity.UsuarioCreacion = _user.LoginUsuario;
                        entry.Entity.FechaCreacion = _dateTime.Now;
                        break;                    
                }

                var propAttr = entry.Entity.GetType().GetProperties().ToList().Where(prop => prop.IsDefined(typeof(IsUpperCase), false)).ToList();
                for (int i = 0; i < propAttr.Count; i++)
                {
                    var value = entry.Entity.GetType().GetProperty(propAttr[i].Name).GetValue(entry.Entity) != null ? entry.Entity.GetType().GetProperty(propAttr[i].Name).GetValue(entry.Entity).ToString().ToUpper() : null;
                    if (value != null)
                    {
                        entry.Entity.GetType().GetProperty(propAttr[i].Name).SetValue(entry.Entity, value);
                    }
                }

            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
