using System;
using System.ComponentModel.DataAnnotations;

namespace Dominio.Common
{
    public abstract class AuditableBaseEntity
    {
        public string UsuarioCreacion { get; set; }               
        public DateTime? FechaCreacion { get; set; }        
        public string UsuarioModificacion { get; set; }        
        public DateTime? FechaModificacion { get; set; }               
        //public int SucursalCreacion { get; set; }
        //public int? SucursalModificacion { get; set; }
    }
}
