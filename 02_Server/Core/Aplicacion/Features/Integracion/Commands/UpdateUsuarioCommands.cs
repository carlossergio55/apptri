using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands
{

    public class UpdateUsuarioCommand : IRequest<Response<int>>
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public string Tipo { get; set; }
        //TODO: agregar parametros
    }

    public class UpdateUsuarioCommandHandler : IRequestHandler<UpdateUsuarioCommand, Response<int>>
    {
        private IRepositoryAsync<Usuario> _repositoryAsync;
        private readonly IMapper _mapper;
        public UpdateUsuarioCommandHandler(IRepositoryAsync<Usuario> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdateUsuarioCommand request, CancellationToken cancellationToken)
        {
            var _Usuario = await _repositoryAsync.GetByIdAsync(request.IdUsuario);
            if (_Usuario == null)
            {
                throw new KeyNotFoundException("Registro no encontrado");
            }
            else
            {
                _Usuario.IdUsuario = request.IdUsuario;
                _Usuario.Nombre = request.Nombre;
                _Usuario.Correo = request.Correo;
                _Usuario.Contraseña = request.Contraseña;
                _Usuario.Tipo = request.Tipo;
                //TODO: agregar mas propiedades

                await _repositoryAsync.UpdateAsync(_Usuario);
                return new Response<int>(_Usuario.IdUsuario);
            }
        }

    }

    
}
