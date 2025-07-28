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

namespace Aplicacion.Features.Integracion.Commands.HorarioC
{

    public class CreateHorarioCommand : IRequest<Response<int>>
    {
        public HorarioDto Horario { get; set; }
    }

    public class CreateHorarioCommandHandler : IRequestHandler<CreateHorarioCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Horario> _repositoryAsync;
        private readonly IMapper _mapper;

        public CreateHorarioCommandHandler(IRepositoryAsync<Horario> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateHorarioCommand request, CancellationToken cancellationToken)
        {
            var nuevoRegistro = _mapper.Map<Horario>(request.Horario);

            
            var data = await _repositoryAsync.AddAsync(nuevoRegistro);

            return new Response<int>(data.IdHorario);
        }
    }



}
