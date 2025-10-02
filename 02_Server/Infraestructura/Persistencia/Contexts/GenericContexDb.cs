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
 
        public DbSet<Sucursal> Sucursal { get; set; }

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
            // Persistencia.Contexts.GenericContexDb.OnModelCreating
            modelBuilder.Entity<Viaje>(e =>
            {
                e.Property(v => v.HoraSalida).HasColumnType("time(0)");
                e.Property(v => v.Direccion).HasMaxLength(10);

                e.HasIndex(v => new { v.IdRuta, v.Fecha, v.HoraSalida, v.Direccion })
                 .IsUnique()
                 .HasDatabaseName("uq_viaje_ruta_fecha_hora_dir");
            });


            // ---------- Boleto ----------
            modelBuilder.Entity<Boleto>(e =>
            {
                e.Property(b => b.Precio).HasColumnType("numeric(10,2)");
                e.Property(b => b.Estado).HasMaxLength(20);
                e.HasIndex(b => new { b.IdViaje, b.IdAsiento }).IsUnique().HasDatabaseName("uq_boleto_viaje_asiento");
            });



            // ---------- Encomienda (sin relación a GuiaCarga) ----------
            modelBuilder.Entity<Encomienda>(e =>
            {
                e.ToTable("encomienda", "public");
                e.HasKey(x => x.IdEncomienda);

                e.Property(x => x.IdEncomienda).HasColumnName("id_encomienda");

                e.Property(x => x.Remitente)
                    .HasColumnName("remitente")
                    .HasMaxLength(40)
                    .IsRequired();

                e.Property(x => x.Destinatario)
                    .HasColumnName("destinatario")
                    .HasMaxLength(40)
                    .IsRequired();

                e.Property(x => x.Descripcion)
                    .HasColumnName("descripcion");

                // La columna generada por trigger (no seteada por la app)
                e.Property(x => x.Guiacarga)
                    .HasColumnName("guiacarga")
                    .HasMaxLength(6);

                e.Property(x => x.IdViaje)
                    .HasColumnName("id_viaje")
                    .IsRequired();

                e.Property(x => x.Precio)
                    .HasColumnName("precio")
                    .HasColumnType("numeric(10,2)");

                e.Property(x => x.Estado)
                    .HasColumnName("estado")
                    .HasMaxLength(20);

                e.Property(x => x.Peso)
                    .HasColumnName("peso")
                    .HasColumnType("numeric(10,2)");

                e.Property(x => x.Pagado)
                    .HasColumnName("pagado");

                e.Property(x => x.OrigenParadaId)
                    .HasColumnName("origen_parada_id");

                e.Property(x => x.DestinoParadaId)
                    .HasColumnName("destino_parada_id");

                // Relaciones reales de Encomienda
                e.HasOne(x => x.Viaje)
                    .WithMany()
                    .HasForeignKey(x => x.IdViaje)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.OrigenParada)
                    .WithMany()
                    .HasForeignKey(x => x.OrigenParadaId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.DestinoParada)
                    .WithMany()
                    .HasForeignKey(x => x.DestinoParadaId)
                    .OnDelete(DeleteBehavior.Restrict);
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
                e.HasKey(x => x.IdRutaParada);  
                e.Property(x => x.IdRutaParada)
                    .HasColumnName("id_ruta_parada")
                    .ValueGeneratedOnAdd();
                e.HasIndex(x => new { x.IdRuta, x.IdParada })
                    .IsUnique()
                    .HasDatabaseName("uq_ruta_parada_ruta_parada");

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

                // PK simple
                e.HasKey(x => x.IdTarifaTramo);
                e.Property(x => x.IdTarifaTramo)
                    .HasColumnName("id_tarifa_tramo")
                    .ValueGeneratedOnAdd();

                e.Property(x => x.IdRuta).HasColumnName("id_ruta").IsRequired();
                e.Property(x => x.OrigenParadaId).HasColumnName("origen_parada_id").IsRequired();
                e.Property(x => x.DestinoParadaId).HasColumnName("destino_parada_id").IsRequired();
                e.Property(x => x.Precio).HasColumnName("precio").HasColumnType("numeric(10,2)").IsRequired();

                // FKs
                e.HasOne(x => x.Ruta)
                    .WithMany()
                    .HasForeignKey(x => x.IdRuta)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.OrigenParada)
                    .WithMany()
                    .HasForeignKey(x => x.OrigenParadaId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.DestinoParada)
                    .WithMany()
                    .HasForeignKey(x => x.DestinoParadaId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Regla de unicidad (ruta + origen + destino)
                e.HasIndex(x => new { x.IdRuta, x.OrigenParadaId, x.DestinoParadaId })
                    .IsUnique()
                    .HasDatabaseName("uq_tarifa_tramo_ruta_origen_destino");

                // (Opcional) Evita origen == destino
                e.HasCheckConstraint("ck_tarifa_tramo_origen_destino_distintos",
                                     "origen_parada_id <> destino_parada_id");
            });
        }


    }
}
