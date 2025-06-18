using System.Data;

namespace Aplicacion.Wrappers
{
    public class PgRefCursor 
    {
        public object Val { get; set; }
        public ParameterDirection? Dir { get; set; }
        public string Name { get; set; }
       
        public class RefCursor
        {
            private readonly PgRefCursor _pgParam = new PgRefCursor();

            public RefCursor(string name)
            {
                _pgParam.Name = name;
            }

            public RefCursor Val(object Value)
            {
                _pgParam.Val = Value;
                return this;
            }

            /// <summary>
            /// Dirección del parametro recibodo por la funcion posgrest (input, output)
            /// </summary>
            /// <param name="direction">ParameterDirection</param>
            /// <returns></returns>
            public RefCursor Dir(ParameterDirection? direction)
            {
                _pgParam.Dir = direction == null ? ParameterDirection.Input : direction;
                return this;
            }
        }
    }
}
