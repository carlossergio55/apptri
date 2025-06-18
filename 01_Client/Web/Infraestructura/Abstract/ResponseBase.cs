using System.Collections.Generic;

namespace Infraestructura.Abstract
{
    public enum State
    {
        Success = 1,
        Warning = 2,
        Error = 3,
        NoData = 4
    }

    public abstract class ResponseBase
    {
        public State State { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public string Referencia { get; set; }
        public string Bandeja { get; set; }

    }




}
