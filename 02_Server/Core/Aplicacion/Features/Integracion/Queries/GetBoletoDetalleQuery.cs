using System.Threading;
using System.Threading.Tasks;
using MediatR;

using Aplicacion.Wrappers;
using Aplicacion.Interfaces;                 // IRepositoryAsync<T>
using Dominio.Entities.Integracion;         // Boleto, Cliente

namespace Aplicacion.Features.Integracion.Queries
{
    public class GetBoletoDetalleQuery : IRequest<Response<BoletoDetalleDto>>
    {
        public int IdBoleto { get; set; }
    }

    public class BoletoDetalleDto
    {
        public int IdBoleto { get; set; }
        public string Estado { get; set; } = "";
        public decimal Precio { get; set; }
        public int IdViaje { get; set; }
        public int IdAsiento { get; set; }
        public int? OrigenParadaId { get; set; }
        public int? DestinoParadaId { get; set; }

        public int IdCliente { get; set; }
        public string ClienteNombre { get; set; } = "";
        public string? ClienteCI { get; set; }
        public string? ClienteCelular { get; set; }
    }

    public class GetBoletoDetalleQueryHandler : IRequestHandler<GetBoletoDetalleQuery, Response<BoletoDetalleDto>>
    {
        private readonly IRepositoryAsync<Boleto> _boletoRepo;
        private readonly IRepositoryAsync<Cliente> _clienteRepo;

        public GetBoletoDetalleQueryHandler(IRepositoryAsync<Boleto> boletoRepo, IRepositoryAsync<Cliente> clienteRepo)
        {
            _boletoRepo = boletoRepo;
            _clienteRepo = clienteRepo;
        }

        public async Task<Response<BoletoDetalleDto>> Handle(GetBoletoDetalleQuery request, CancellationToken ct)
        {
            var b = await _boletoRepo.GetByIdAsync(request.IdBoleto);
            if (b == null) throw new System.Collections.Generic.KeyNotFoundException("Boleto no encontrado.");

            var c = await _clienteRepo.GetByIdAsync(b.IdCliente);

            var dto = new BoletoDetalleDto
            {
                IdBoleto = b.IdBoleto,
                Estado = b.Estado,
                Precio = b.Precio,
                IdViaje = b.IdViaje,
                IdAsiento = b.IdAsiento,
                OrigenParadaId = b.OrigenParadaId,
                DestinoParadaId = b.DestinoParadaId,
                IdCliente = b.IdCliente,
                ClienteNombre = c?.Nombre ?? "-",
                ClienteCI = c != null ? c.Carnet.ToString() : null,
                ClienteCelular = c?.Celular.ToString()
            };

            return new Response<BoletoDetalleDto>(dto);
        }
    }
}
