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

namespace Aplicacion.Features.Integracion.Commands.RutaC
{

    public class UpdateRutaCommand : IRequest<Response<int>>
    {
        public int IdRuta { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public string Duracion { get; set; }
        public bool EsExtendido { get; set; }
        //TODO: agregar parametros
    }

    public class UpdateRutaCommandHandler : IRequestHandler<UpdateRutaCommand, Response<int>>
    {
        private IRepositoryAsync<Ruta> _repositoryAsync;
        private readonly IMapper _mapper;
        public UpdateRutaCommandHandler(IRepositoryAsync<Ruta> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdateRutaCommand request, CancellationToken cancellationToken)
        {
            var _Ruta = await _repositoryAsync.GetByIdAsync(request.IdRuta);
            if (_Ruta == null)
            {
                throw new KeyNotFoundException("Registro no encontrado");
            }
            else
            {
                _Ruta.IdRuta = request.IdRuta;
                _Ruta.Origen = request.Origen;
                _Ruta.Destino = request.Destino;
                _Ruta.Duracion = request.Duracion;
                _Ruta.EsExtendido = request.EsExtendido;
                //TODO: agregar mas propiedades

                await _repositoryAsync.UpdateAsync(_Ruta);
                return new Response<int>(_Ruta.IdRuta);
            }
        }

        
    }

   
}
