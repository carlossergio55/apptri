using System.Collections.Generic;
using System.Net;

namespace Infraestructura.Models.Authentication
{
    public class SegResponse
    {
        public bool succeeded { get; set; }
        public ObjectEntity data { get; set; }        
        public string message { get; set; }
        public List<string> errors { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<SegMenuResponse> Menu { get; set; }

    }
}
