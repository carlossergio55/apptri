using System;

namespace Server.Models
{
    public class FacturaModel
    {
        public int IdfactFacturacabecera { get; set; }
        public int IdfactFacturadosificacion { get; set; }
        public string CodigoControl { get; set; }
        public int NroFactura { get; set; }
        public DateTime FechaFactura { get; set; }
        public string Observaciones { get; set; }
        public string NitCedula { get; set; }
        public string Nombre { get; set; }
        public string Estado { get; set; }

    }

    public class FacturaCommand
    {
        public int IdfactFacturadosificacion { get; set; }
        public string CodigoControl { get; set; }
        public int NroFactura { get; set; }
        public DateTime FechaFactura { get; set; }
        public int IdfactPlanillaentidad { get; set; }
        public int IdcEstadofactura { get; set; }
        public string NitCedula { get; set; }
        public string Nombre { get; set; }
    }

    public class FacturaUpdate {

        public int IdfactFacturacabecera { get; set; }
        public string Nombre { get; set; }
        public string NitCedula { get; set; }
        public DateTime FechaFactura { get; set; }
    }

    public class GetId
    {
        public int id { get; set; }
    }

    public class GetRange
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }

    }

}
