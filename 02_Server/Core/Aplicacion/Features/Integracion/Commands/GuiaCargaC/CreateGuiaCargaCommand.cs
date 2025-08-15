
using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.GuiaCargaC
{
    public class CreateGuiaCargaCommand : IRequest<Response<int>>
    {
        public GuiaCargaDto Guia { get; set; } = null!;
    }

    public class CreateGuiaCargaCommandHandler : IRequestHandler<CreateGuiaCargaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<GuiaCarga> _repo;
        private readonly IMapper _mapper;

        public CreateGuiaCargaCommandHandler(IRepositoryAsync<GuiaCarga> repo, IMapper mapper)
        { _repo = repo; _mapper = mapper; }

        public async Task<Response<int>> Handle(CreateGuiaCargaCommand request, CancellationToken ct)
        {
            var entity = _mapper.Map<GuiaCarga>(request.Guia);
            var saved = await _repo.AddAsync(entity);
            return new Response<int>(saved.IdGuiaCarga);
        }
    }
}
