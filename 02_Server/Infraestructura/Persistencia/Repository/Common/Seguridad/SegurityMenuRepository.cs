using Aplicacion.DTOs.Segurity;
using Aplicacion.Interfaces.Repositories;
using Aplicacion.Wrappers;
using Identity.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistencia.Repository.Common.Seguridad
{
    public class SegurityMenuRepository : ISegurityMenuRepository
    {
        private readonly SegurityContext _context;


        public SegurityMenuRepository(SegurityContext context)
        {
            _context = context;

        }


        public async Task<Response<List<UserMenuDto>>> ValidateUserMenu(int pIdPerfil, int pIdSistema)
        {
            var _Result = new Response<List<UserMenuDto>>();
            try
            {
                var vQuery = await Task.FromResult((from r in _context.Menuobjetos.Where(f => f.idseg_perfil == pIdPerfil && f.idseg_sistema== pIdSistema)

                                                    select new UserMenuDto
                                                    {
                                                        id = r.id,
                                                        parentid = r.parentid,
                                                        texto = r.texto,
                                                        url = r.url,
                                                        idseg_sistema = r.idseg_sistema,
                                                        nombre_sistema = r.nombre_sistema,
                                                        idseg_perfil = r.idseg_sistema,
                                                        nombre_perfil = r.nombre_perfil,
                                                        posicion_modulo = r.posicion_modulo,
                                                        IconoModulo = r.IconoModulo,
                                                        menu = r.menu,
                                                    })
                                         .ToList());
                _Result.Succeeded = vQuery.Count > 0;
                _Result.Data = new List<UserMenuDto>();
                _Result.Data = vQuery;
            }
            catch (Exception e)
            {
                _Result.Succeeded = false;
                _Result.Message = e.Message;
            }

            return _Result;
        }

    }
}

