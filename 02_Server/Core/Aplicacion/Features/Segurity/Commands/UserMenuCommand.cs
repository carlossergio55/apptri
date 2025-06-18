using Aplicacion.DTOs.Segurity;
using Aplicacion.Interfaces;
using Aplicacion.Interfaces.Repositories;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using AutoMapper;
using Dominio.Entities.Seguridad;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Segurity.Commands
{

    public class GetAllUserMenuQuery : IRequest<Response<List<UserMenuDto>>>
    {
        public int idsegperfil { get; set; }
        public int idsegSistema { get; set; }


        public class GetAllUserMenuQueryHandler : IRequestHandler<GetAllUserMenuQuery, Response<List<UserMenuDto>>>
        {
        
            private readonly ISegurityMenuRepository _repo;

            public GetAllUserMenuQueryHandler(ISegurityMenuRepository _rep)
            {
                _repo = _rep;
            }

            public async Task<Response<List<UserMenuDto>>> Handle(GetAllUserMenuQuery request, CancellationToken cancellationToken)
            {
                var _Result = new Response<List<UserMenuDto>>();

                try
                {
                    _Result = await _repo.ValidateUserMenu(request.idsegperfil, request.idsegSistema);
                    _Result.Message = _Result.Data.Count > 0 ? String.Format(Resources.Generic.Resource.QuerySucces, _Result.Data.Count) : Resources.Generic.Resource.QueryBad;
                    _Result.Succeeded = _Result.Data.Count > 0;
                }
                catch (Exception e)
                {
                    _Result.Message = e.Message;
                    _Result.Succeeded = false;
                }

                return _Result;

         
            }
        }

    }

  
}
