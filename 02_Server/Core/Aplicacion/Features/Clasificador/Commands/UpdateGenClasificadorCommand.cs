using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities;
using Dominio.Entities.Seguridad;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Clasificador.Commands
{

    public class UpdateGenClasificadorCommand : IRequest<Response<int>>
    {
        public int IdgenClasificador { get; set; }
        public string Descripcion { get; set; }
        public int IdgenClasificadortipo { get; set; }
		public string Valor2 { get; set; }
		// public int IdgenClasificador { get; set; }
		//TODO: agregar parametros
	}

    public class UpdateGenClasificadorCommandHandler : IRequestHandler<UpdateGenClasificadorCommand, Response<int>>
    {
        private IRepositoryAsync<GenClasificador> _repositoryAsync;
        private readonly IMapper _mapper;
        public UpdateGenClasificadorCommandHandler(IRepositoryAsync<GenClasificador> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdateGenClasificadorCommand request, CancellationToken cancellationToken)
        {
            var _GenClasificador = await _repositoryAsync.GetByIdAsync(request.IdgenClasificador);
            if (_GenClasificador == null)
            {
                throw new KeyNotFoundException("Registro no encontrado");
            }
            else
            {
                _GenClasificador.IdgenClasificador = request.IdgenClasificador;
                _GenClasificador.Descripcion = request.Descripcion;
                _GenClasificador.IdgenClasificadortipo = request.IdgenClasificadortipo;
                _GenClasificador.Valor2 = request.Valor2;   
                //TODO: agregar mas propiedades

                await _repositoryAsync.UpdateAsync(_GenClasificador);
                return new Response<int>(_GenClasificador.IdgenClasificador);
            }
        }

    }

}
