using Ardalis.Specification;
using System.Linq;

namespace Aplicacion.Specifications
{
    public class PagedSpecification : Specification<object>
    {
        public PagedSpecification(int pageSize, int pageNumber, string nombre, string apellido)
        {
            Query.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);


            //if (!string.IsNullOrEmpty(nombre))
            //    Query.Search(x => x.Nombre, $"%{nombre}%");


            //if (!string.IsNullOrEmpty(apellido))
            //    Query.Search(x => x.Apellido, $"%{apellido}%");
        }
    }
}
