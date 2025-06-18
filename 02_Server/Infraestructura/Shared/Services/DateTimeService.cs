using Aplicacion.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.Now; // Zona horaria local
        public DateTime NowUtc => DateTime.UtcNow; // UTC
    }
}
