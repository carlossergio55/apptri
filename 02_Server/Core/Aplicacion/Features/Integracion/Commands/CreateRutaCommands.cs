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

    public class CreateRutaCommand : IRequest<Response<int>>
    {
        public RutaDto Ruta { get; set; }
    }

    public class CreateRutaCommandHandler : IRequestHandler<CreateRutaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Ruta> _repositoryAsync;
        private readonly IMapper _mapper;
        public CreateRutaCommandHandler(IRepositoryAsync<Ruta> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateRutaCommand request, CancellationToken cancellationToken)
        {
            var nuevoRegistro = _mapper.Map<Ruta>(request.Ruta);
            var data = await _repositoryAsync.AddAsync(nuevoRegistro);
            return new Response<int>(data.IdRuta);
        }

        
    }

    
}
