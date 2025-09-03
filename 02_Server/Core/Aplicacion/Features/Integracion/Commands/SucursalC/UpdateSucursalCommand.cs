using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.SucursalC
{
    public class UpdateSucursalCommand : IRequest<Response<int>>
    {
        public int IdSucursal { get; set; }
        public int IdParada { get; set; }
        public string Nombre { get; set; }
        public string Ciudad { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Estado { get; set; }
    }

    public class UpdateSucursalCommandHandler : IRequestHandler<UpdateSucursalCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Sucursal> _repositoryAsync;
        private readonly IMapper _mapper;

        public UpdateSucursalCommandHandler(IRepositoryAsync<Sucursal> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdateSucursalCommand request, CancellationToken cancellationToken)
        {
            var sucursal = await _repositoryAsync.GetByIdAsync(request.IdSucursal);

            if (sucursal == null)
            {
                throw new KeyNotFoundException("Sucursal no encontrada con el id proporcionado");
            }

            // Actualizar campos
            sucursal.IdParada = request.IdParada;
            sucursal.Nombre = request.Nombre;
            sucursal.Ciudad = request.Ciudad;
            sucursal.Direccion = request.Direccion;
            sucursal.Telefono = request.Telefono;
            sucursal.Estado = request.Estado;

            await _repositoryAsync.UpdateAsync(sucursal);

            return new Response<int>(sucursal.IdSucursal);
        }
    }
}
