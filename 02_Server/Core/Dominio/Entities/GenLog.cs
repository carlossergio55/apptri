using System;
using System.Collections.Generic;

#nullable disable

namespace Dominio.Entities
{
    public partial class GenLog
    {
        public int IdgenLog { get; set; }
        public string Registro { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string UsuarioModificacion { get; set; }
    }
}
