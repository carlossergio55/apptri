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

namespace Aplicacion.Features.Integracion.Commands.ChoferC
{

    public class CreateChoferCommand : IRequest<Response<int>>
    {
        public ChoferDto Chofer { get; set; }
    }

    public class CreateChoferCommandHandler : IRequestHandler<CreateChoferCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Chofer> _repositoryAsync;
        private readonly IMapper _mapper;
        public CreateChoferCommandHandler(IRepositoryAsync<Chofer> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateChoferCommand request, CancellationToken cancellationToken)
        {
            var nuevoRegistro = _mapper.Map<Chofer>(request.Chofer);
            var data = await _repositoryAsync.AddAsync(nuevoRegistro);
            return new Response<int>(data.IdChofer);
        }

       
    }

   
}
