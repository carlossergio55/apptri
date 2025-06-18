//using Aplicacion.Wrappers;
//using Ardalis.Specification.EntityFrameworkCore;
//using Persistencia.Contexts;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Persistencia.Repository.Custom
//{
//    public class PacFormularioRepository : IPacFormularioRepository
//    {
//        private readonly AplicationDbContext _context;

//        public PacFormularioRepository(AplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<Response<List<PacFormularioDto>>> GetPacFormularioAsync()
//        {
//            var query = await _context.PacFormulario
//                .Include(f => f.PacContratacion)  // Confirma que PacContratacion es una entidad de navegación
//                .Include(f => f.GenArea)          // Confirma que GenArea es una entidad de navegación
//                .Include(f => f.PacUsuario)
//                .Select(f => new PacFormularioDto
//                {
//                    IdpacFormulario = f.IdpacFormulario,
//                    ObjetoContrato = f.Objetocontrato,
//                    ContratacionDescripcion = f.PacContratacion != null ? f.PacContratacion.Descripcion : null,
//                    AreaDescripcion = f.GenArea != null ? f.GenArea.Descripcion : null,
//                    UsuarioNombreCompleto = f.PacUsuario != null ? f.PacUsuario.Nombre + " " + f.PacUsuario.Apellido : null,
//                    EstadoProceso = f.EstadoAccion,
//                    EstadoTexto = f.EstadoAccion == "1" ? "Iniciado" : "No iniciado"
//                })
//                .OrderBy(f => f.ObjetoContrato)
//                .ToListAsync();

//            return new Response<List<PacFormularioDto>>(query);
//        }


//    }
//}
