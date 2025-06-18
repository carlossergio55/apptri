using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Interfaces
{
    public interface ICastMappingUpdate
    {
        /// <summary>
        /// Mappea sin nececidad de registrar con la condicion que las propiedades sean exactamente iguales
        /// </summary>
        /// <typeparam name="T">Entidad Generica Dominio</typeparam>
        /// <param name="Source">Entidad Dtop recibida</param>
        /// <param name="target">Entidad Domnio</param>
        void CastMapping<T>(object Source, ref T target);
    }
}
