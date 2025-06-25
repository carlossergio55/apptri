using Aplicacion.Features.Integracion.Commands;
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

    public class UpdateClienteCommand : IRequest<Response<int>>
    {
        public int IdCliente { get; set; }
        public string Nombre { get; set; }
        public int Carnet { get; set; }
        public string Correo { get; set; }
        public int Celular { get; set; }
    }
    //TODO: agregar parametros
}

public class UpdateClienteCommandHandler : IRequestHandler<UpdateClienteCommand, Response<int>>
{
    private IRepositoryAsync<Cliente> _repositoryAsync;
    private readonly IMapper _mapper;
    public UpdateClienteCommandHandler(IRepositoryAsync<Cliente> repositoryAsync, IMapper mapper)
    {
        _repositoryAsync = repositoryAsync;
        _mapper = mapper;
    }

    public async Task<Response<int>> Handle(UpdateClienteCommand request, CancellationToken cancellationToken)
    {
        var _Cliente = await _repositoryAsync.GetByIdAsync(request.IdCliente);
        if (_Cliente == null)
        {
            throw new KeyNotFoundException("Registro no encontrado");
        }
        else
        {
            _Cliente.IdCliente = request.IdCliente;
            _Cliente.Nombre = request.Nombre;
            _Cliente.Carnet = request.Carnet;
            _Cliente.Correo = request.Correo;
            _Cliente.Celular = request.Celular;
            //TODO: agregar mas propiedades

            await _repositoryAsync.UpdateAsync(_Cliente);
            return new Response<int>(_Cliente.IdCliente);
        }
    }


}
