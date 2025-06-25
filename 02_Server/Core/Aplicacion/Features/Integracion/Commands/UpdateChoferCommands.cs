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

namespace Aplicacion.Features.Integracion.Commands
{

    public class UpdateChoferCommand : IRequest<Response<int>>
    {
        public int IdChofer { get; set; }
        public string Nombre { get; set; }
        public int Carnet { get; set; }
        public int Celular { get; set; }
        public string Licencia { get; set; }
        //TODO: agregar parametros
    }

    public class UpdateChoferCommandHandler : IRequestHandler<UpdateChoferCommand, Response<int>>
    {
        private IRepositoryAsync<Chofer> _repositoryAsync;
        private readonly IMapper _mapper;
        public UpdateChoferCommandHandler(IRepositoryAsync<Chofer> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdateChoferCommand request, CancellationToken cancellationToken)
        {
            var _Chofer = await _repositoryAsync.GetByIdAsync(request.IdChofer);
            if (_Chofer == null)
            {
                throw new KeyNotFoundException("Registro no encontrado");
            }
            else
            {
                _Chofer.IdChofer = request.IdChofer;
                _Chofer.Nombre = request.Nombre;
                _Chofer.Carnet = request.Carnet;
                _Chofer.Celular = request.Celular;
                _Chofer.Licencia = request.Licencia;
                //TODO: agregar mas propiedades

                await _repositoryAsync.UpdateAsync(_Chofer);
                return new Response<int>(_Chofer.IdChofer);
            }
        }

        
    }

   
}
