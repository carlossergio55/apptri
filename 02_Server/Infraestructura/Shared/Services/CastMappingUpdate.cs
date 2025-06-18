using Aplicacion.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public class CastMappingUpdate : ICastMappingUpdate
    {
        public void CastMapping<T>(object Source, ref T target)
        {        
            foreach (var prop in Source.GetType().GetProperties())
            {
                target.GetType().GetProperty(prop.Name).SetValue(target, prop.GetValue(Source, null), null);
            }
        }
    }
}
