using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.HorarioC
{
    public class UpdateHorarioCommand : IRequest<Response<int>>
    {
        public int IdHorario { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public string DiaSemana { get; set; }
        public int IdRuta { get; set; }
    }

    public class UpdateHorarioCommandHandler : IRequestHandler<UpdateHorarioCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Horario> _repositoryAsync;
        private readonly IMapper _mapper;

        public UpdateHorarioCommandHandler(IRepositoryAsync<Horario> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdateHorarioCommand request, CancellationToken cancellationToken)
        {
            var horario = await _repositoryAsync.GetByIdAsync(request.IdHorario);
            if (horario == null)
                throw new KeyNotFoundException("Registro no encontrado");

            horario.HoraSalida = request.HoraSalida;
            horario.DiaSemana = request.DiaSemana;
            horario.IdRuta = request.IdRuta;

            await _repositoryAsync.UpdateAsync(horario);
            return new Response<int>(horario.IdHorario);
        }
    }
}
