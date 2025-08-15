using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.RutaParadaC
{
    public class CreateRutaParadaCommand : IRequest<Response<int>>
    {
        public RutaParadaDto Item { get; set; } = null!;
    }

    public class CreateRutaParadaCommandHandler : IRequestHandler<CreateRutaParadaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<RutaParada> _repo;
        private readonly IRepositoryAsync<Ruta> _repoRuta;
        private readonly IRepositoryAsync<Parada> _repoParada;
        private readonly IMapper _mapper;

        public CreateRutaParadaCommandHandler(
            IRepositoryAsync<RutaParada> repo,
            IRepositoryAsync<Ruta> repoRuta,
            IRepositoryAsync<Parada> repoParada,
            IMapper mapper)
        { _repo = repo; _repoRuta = repoRuta; _repoParada = repoParada; _mapper = mapper; }

        public async Task<Response<int>> Handle(CreateRutaParadaCommand request, CancellationToken ct)
        {
            // validaciones mínimas
            if (await _repoRuta.GetByIdAsync(request.Item.IdRuta) is null)
                throw new Exception("Ruta no existe.");
            if (await _repoParada.GetByIdAsync(request.Item.IdParada) is null)
                throw new Exception("Parada no existe.");

            // evitar duplicidad en (ruta, parada)
            var all = await _repo.ListAsync();
            if (all.Any(x => x.IdRuta == request.Item.IdRuta && x.IdParada == request.Item.IdParada))
                throw new Exception("La parada ya está asociada a esa ruta.");

            var entity = _mapper.Map<RutaParada>(request.Item);
            await _repo.AddAsync(entity);
            return new Response<int>(1); // clave compuesta
        }
    }
}
