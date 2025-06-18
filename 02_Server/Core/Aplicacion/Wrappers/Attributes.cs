using System.Data;
using System.Runtime.Serialization;

namespace Aplicacion.Wrappers
{
    public class Attributes 
    {
    }

    [DataContract]
    public class Date : System.Attribute
    {
    }

    [DataContract]
    public class Direction : System.Attribute
    {
        public ParameterDirection Dir { get; set; }
    }

}
