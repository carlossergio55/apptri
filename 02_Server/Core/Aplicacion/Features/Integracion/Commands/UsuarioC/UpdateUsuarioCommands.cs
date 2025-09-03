using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.UsuarioC
{
    public class UpdateUsuarioCommand : IRequest<Response<int>>
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public string Tipo { get; set; }
        public int? IdSucursal { get; set; }
    }

    public class UpdateUsuarioCommandHandler : IRequestHandler<UpdateUsuarioCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Usuario> _repositoryAsync;
        private readonly IMapper _mapper;

        public UpdateUsuarioCommandHandler(IRepositoryAsync<Usuario> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdateUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _repositoryAsync.GetByIdAsync(request.IdUsuario);

            if (usuario == null)
                throw new KeyNotFoundException("Usuario no encontrado");

            // Actualizar propiedades
            usuario.Nombre = request.Nombre;
            usuario.Correo = request.Correo;
            usuario.Contraseña = request.Contraseña;
            usuario.Tipo = request.Tipo;

            usuario.IdSucursal = request.IdSucursal;

            await _repositoryAsync.UpdateAsync(usuario);

            return new Response<int>(usuario.IdUsuario);
        }
    }
}
