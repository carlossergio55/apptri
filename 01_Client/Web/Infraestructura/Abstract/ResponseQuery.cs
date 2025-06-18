using System.Collections.Generic;

namespace Infraestructura.Abstract
{
    public class ResponseQuery<T> : ResponseBase
    {
        public List<T> Data{ get; set; }
    }
}
