using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.HorarioC
{
    public class CreateHorarioCommand : IRequest<Response<int>>
    {
        public HorarioDto Horario { get; set; } = null!;
    }

    public class CreateHorarioCommandHandler : IRequestHandler<CreateHorarioCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Horario> _repo;
        private readonly IMapper _mapper;

        public CreateHorarioCommandHandler(
            IRepositoryAsync<Horario> repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateHorarioCommand request, CancellationToken ct)
        {
            // Mapear directo y guardar (sin validaciones)
            var entity = _mapper.Map<Horario>(request.Horario);
            var saved = await _repo.AddAsync(entity);

            // Devolver siempre una lista de errores vacía para evitar el error $.errors
            return new Response<int>
            {
                Data = saved.IdHorario,
                Message = "Guardado correctamente.",
                Errors = new List<string>()
            };
        }
    }
}
