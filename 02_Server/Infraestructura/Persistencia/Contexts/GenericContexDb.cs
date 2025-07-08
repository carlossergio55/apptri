using Dominio.Entities;
using Dominio.Entities.Integracion;
using Dominio.Entities.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace Persistencia.Contexts
{
    public class GenericContexDb : DbContext
    {
        public GenericContexDb(DbContextOptions options) : base(options)
        {
        }

        //TODO: Agregar aqui DbSets de las entidades de dominio correspondiente al contexto de conexcion general.
        #region DbSets
        public DbSet<SegUsuario> SegUsuario { get; set; }
        public DbSet<GenClasificador> Clasificador { get; set; }
        public DbSet<GenClasificadortipo> ClasificadorTipo { get; set; }
        public DbSet<GenClasificador> GenClasificador { get; set; }
        public DbSet<GenClasificadortipo> GenClasificadortipo { get; set; }
        public DbSet<Ruta> Ruta { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Chofer> Chofer { get; set; }
        public DbSet<Bus> Bus { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Horario> Horario { get; set; }
        public DbSet<Asiento> Asiento { get; set; }
        public DbSet<Viaje> Viaje { get; set; }
        public DbSet<Boleto> Boleto { get; set; }
        public DbSet<Encomienda> Encomienda { get; set; }
        public DbSet<Pago> Pago { get; set; }



        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Horario>()
                .HasOne(h => h.Ruta)
                .WithMany() // O .WithMany(r => r.Horarios) si Ruta tiene colección Horarios
                .HasForeignKey(h => h.IdRuta)
                .OnDelete(DeleteBehavior.Restrict);

            // Aquí puedes agregar otras configuraciones para otras entidades si las tienes
        }

    }
}
