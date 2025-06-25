using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;


namespace Aplicacion.Features.Integracion.Commands
{

    public class UpdateBusCommand : IRequest<Response<int>>
    {
        public int IdBus { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public int Capacidad { get; set; }
        //TODO: agregar parametros
    }

    public class UpdateBusCommandHandler : IRequestHandler<UpdateBusCommand, Response<int>>
    {
        private IRepositoryAsync<Bus> _repositoryAsync;
        private readonly IMapper _mapper;
        public UpdateBusCommandHandler(IRepositoryAsync<Bus> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdateBusCommand request, CancellationToken cancellationToken)
        {
            var _Bus = await _repositoryAsync.GetByIdAsync(request.IdBus);
            if (_Bus == null)
            {
                throw new KeyNotFoundException("Registro no encontrado");
            }
            else
            {
                _Bus.IdBus = request.IdBus;
                _Bus.Placa = request.Placa;
                _Bus.Modelo = request.Modelo;
                _Bus.Capacidad = request.Capacidad;
                //TODO: agregar mas propiedades

                await _repositoryAsync.UpdateAsync(_Bus);
                return new Response<int>(_Bus.IdBus);
            }
        }
    }

    
}
