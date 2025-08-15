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
        public DbSet<Parada> Parada { get; set; }
        public DbSet<RutaParada> RutaParada { get; set; }
        public DbSet<TarifaTramo> TarifaTramo { get; set; }
        public DbSet<GuiaCarga> GuiaCarga { get; set; }


        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Horario>(e =>
            {
                e.HasOne(h => h.Ruta).WithMany().HasForeignKey(h => h.IdRuta).OnDelete(DeleteBehavior.Restrict);
                e.Property(h => h.HoraSalida).HasColumnType("time(0)");
                e.Property(h => h.Direccion).HasMaxLength(10);
                e.Property(h => h.DiaSemana).HasMaxLength(10);
            });

            // ---------- Viaje ----------
            modelBuilder.Entity<Viaje>(e =>
            {
                e.Property(v => v.HoraSalida).HasColumnType("time(0)");
                e.Property(v => v.Direccion).HasMaxLength(10);
                e.HasOne(v => v.Ruta).WithMany().HasForeignKey(v => v.IdRuta).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(v => v.Chofer).WithMany().HasForeignKey(v => v.IdChofer).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(v => v.Bus).WithMany().HasForeignKey(v => v.IdBus).OnDelete(DeleteBehavior.Restrict);
            });

            // ---------- Boleto ----------
            modelBuilder.Entity<Boleto>(e =>
            {
                e.Property(b => b.Precio).HasColumnType("numeric(10,2)");
                e.Property(b => b.Estado).HasMaxLength(20);
                e.HasIndex(b => new { b.IdViaje, b.IdAsiento }).IsUnique().HasDatabaseName("uq_boleto_viaje_asiento");
            });

            // ---------- Encomienda ----------
            modelBuilder.Entity<Encomienda>(e =>
            {
                e.Property(x => x.Precio).HasColumnType("numeric(10,2)");
                e.Property(x => x.Peso).HasColumnType("numeric(10,2)");
                e.Property(x => x.Estado).HasMaxLength(20);
                e.HasIndex(x => x.IdViaje).HasDatabaseName("ix_encomienda_viaje");

                
                // Relación con GuiaCarga
                e.HasOne(x => x.Guia)
                 .WithMany(g => g.Encomiendas)
                 .HasForeignKey(x => x.IdGuiaCarga)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------- GuiaCarga (NUEVO) ----------
            modelBuilder.Entity<GuiaCarga>(e =>
            {
                e.ToTable("guia_carga", "public");
                e.HasKey(g => g.IdGuiaCarga);
                e.Property(g => g.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
                e.HasIndex(g => g.Codigo).IsUnique();
            });

            // ---------- Parada ----------
            modelBuilder.Entity<Parada>(e =>
            {
                e.ToTable("parada", "public");
                e.HasKey(x => x.IdParada);
                e.Property(x => x.IdParada).HasColumnName("id_parada");
                e.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(60).IsRequired();
                e.HasIndex(x => x.Nombre).IsUnique();
            });

            // ---------- RutaParada ----------
            modelBuilder.Entity<RutaParada>(e =>
            {
                e.ToTable("ruta_parada", "public");
                e.HasKey(x => new { x.IdRuta, x.IdParada });
                e.Property(x => x.IdRuta).HasColumnName("id_ruta");
                e.Property(x => x.IdParada).HasColumnName("id_parada");
                e.Property(x => x.Orden).HasColumnName("orden");
                e.HasIndex(x => new { x.IdRuta, x.Orden }).HasDatabaseName("ix_ruta_parada_ruta_orden");
                e.HasOne(x => x.Ruta).WithMany().HasForeignKey(x => x.IdRuta).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Parada).WithMany().HasForeignKey(x => x.IdParada).OnDelete(DeleteBehavior.Cascade);
            });

            // ---------- TarifaTramo ----------
            modelBuilder.Entity<TarifaTramo>(e =>
            {
                e.ToTable("tarifa_tramo", "public");
                e.HasKey(x => new { x.IdRuta, x.OrigenParadaId, x.DestinoParadaId });
                e.Property(x => x.IdRuta).HasColumnName("id_ruta");
                e.Property(x => x.OrigenParadaId).HasColumnName("origen_parada_id");
                e.Property(x => x.DestinoParadaId).HasColumnName("destino_parada_id");
                e.Property(x => x.Precio).HasColumnName("precio").HasColumnType("numeric(10,2)");
                e.HasOne(x => x.Ruta).WithMany().HasForeignKey(x => x.IdRuta).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.OrigenParada).WithMany().HasForeignKey(x => x.OrigenParadaId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.DestinoParada).WithMany().HasForeignKey(x => x.DestinoParadaId).OnDelete(DeleteBehavior.Restrict);
            });
        }


    }
}
