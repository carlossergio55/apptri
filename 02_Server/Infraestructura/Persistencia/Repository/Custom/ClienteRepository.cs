using Aplicacion.DTOs;
using Aplicacion.Interfaces.Repositories;
using Aplicacion.Wrappers;
using Microsoft.EntityFrameworkCore;
using Persistencia.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistencia.Repository.Custom
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AplicationDbContext _context;

        public ClienteRepository(AplicationDbContext context)
        {
            _context = context;
        }

        
    }
}
