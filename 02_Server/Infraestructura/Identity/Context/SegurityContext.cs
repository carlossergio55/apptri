using Dominio.Entities.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace Identity.Context
{
    public class SegurityContext : DbContext
    {
        public SegurityContext()
        {
        }

        public SegurityContext(DbContextOptions<SegurityContext> option) : base(option)
        {
        }

        //TODO: Agregar aqui DbSets correspondientes al contexto de conexcion de seguridad
        #region Dbsets
        public virtual DbSet<SegClasificador> SegClasificador { get; set; }
        public virtual DbSet<SegPerfil> SegPerfil { get; set; }
        public virtual DbSet<SegPersonaExt> SegPersonaExt { get; set; }
        public virtual DbSet<SegSistema> SegSistema { get; set; }
        public virtual DbSet<SegvUsuario> Usuario { get; set; }
        public virtual DbSet<SegvMenuobjetos> Menuobjetos { get; set; }
        #endregion

    }
}
