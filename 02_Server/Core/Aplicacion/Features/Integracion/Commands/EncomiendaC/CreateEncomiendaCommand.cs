using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.EncomiendaC
{
    public class CreateEncomiendaCommand : IRequest<Response<int>>
    {
        public EncomiendaDto Encomienda { get; set; } = null!;
    }

    public class CreateEncomiendaCommandHandler : IRequestHandler<CreateEncomiendaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Encomienda> _repoEncomienda;
        private readonly IRepositoryAsync<GuiaCarga> _repoGuia;
        private readonly IMapper _mapper;

        public CreateEncomiendaCommandHandler(
            IRepositoryAsync<Encomienda> repoEncomienda,
            IRepositoryAsync<GuiaCarga> repoGuia,
            IMapper mapper)
        {
            _repoEncomienda = repoEncomienda;
            _repoGuia = repoGuia;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateEncomiendaCommand request, CancellationToken ct)
        {
            var dto = request.Encomienda;

            // Si no hay FK de guía pero sí viene el código, creamos la guía primero
            if (dto.IdGuiaCarga <= 0 && !string.IsNullOrWhiteSpace(dto.CodigoGuia))
            {
                var guia = new GuiaCarga { Codigo = dto.CodigoGuia };
                var savedGuia = await _repoGuia.AddAsync(guia);
                dto.IdGuiaCarga = savedGuia.IdGuiaCarga;
            }

            var entity = _mapper.Map<Encomienda>(dto);
            var saved = await _repoEncomienda.AddAsync(entity);
            return new Response<int>(saved.IdEncomienda);
        }
    }
}
