using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.EncomiendaC
{
    public class UpdateEncomiendaCommand : IRequest<Response<int>>
    {
        public int IdEncomienda { get; set; }
        public string Remitente { get; set; }
        public string Destinatario { get; set; }
        public string Descripcion { get; set; }
        public int IdViaje { get; set; }
        public decimal Precio { get; set; }
        public string Estado { get; set; }
        public decimal Peso { get; set; }
        public bool Pagado { get; set; }
        public string GuiaCarga { get; set; }
    }

    public class UpdateEncomiendaCommandHandler : IRequestHandler<UpdateEncomiendaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Encomienda> _repositoryAsync;
        private readonly IMapper _mapper;

        public UpdateEncomiendaCommandHandler(IRepositoryAsync<Encomienda> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdateEncomiendaCommand request, CancellationToken cancellationToken)
        {
            var encomienda = await _repositoryAsync.GetByIdAsync(request.IdEncomienda);
            if (encomienda == null)
                throw new KeyNotFoundException("Registro no encontrado");

            encomienda.Remitente = request.Remitente;
            encomienda.Destinatario = request.Destinatario;
            encomienda.Descripcion = request.Descripcion;
            encomienda.IdViaje = request.IdViaje;
            encomienda.Precio = request.Precio;
            encomienda.Estado = request.Estado;
            encomienda.Peso = request.Peso;
            encomienda.Pagado = request.Pagado;
            encomienda.GuiaCarga = request.GuiaCarga;

            await _repositoryAsync.UpdateAsync(encomienda);
            return new Response<int>(encomienda.IdEncomienda);
        }
    }
}
