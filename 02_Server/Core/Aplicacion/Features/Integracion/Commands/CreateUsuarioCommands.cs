using Aplicacion.DTOs.Integracion;
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

    public class CreateUsuarioCommand : IRequest<Response<int>>
    {
        public UsuarioDto Usuario { get; set; }
    }

    public class CreateUsuarioCommandHandler : IRequestHandler<CreateUsuarioCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Usuario> _repositoryAsync;
        private readonly IMapper _mapper;
        public CreateUsuarioCommandHandler(IRepositoryAsync<Usuario> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateUsuarioCommand request, CancellationToken cancellationToken)
        {
            var nuevoRegistro = _mapper.Map<Usuario>(request.Usuario);
            var data = await _repositoryAsync.AddAsync(nuevoRegistro);
            return new Response<int>(data.IdUsuario);
        }

      
    }

 
}
