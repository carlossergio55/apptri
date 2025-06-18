using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRest.Model
{
    public class Efficiency
    {
        public string ReporteCertificacion { get; set; }
    }

    public class EfficiencyRoot
    {
        public EfficiencyEnvelope Envelope { get; set; }
    }

    public class EfficiencyResult
    {
        public string ReporteCertificacion { get; set; }
    }

    public class EfficiencyResponse
    {
        public EfficiencyResult ConsultaDatoPersonaCertificacionResult { get; set; }
    }

    public class EfficiencyBody
    {
        public EfficiencyResponse ConsultaDatoPersonaCertificacionResponse { get; set; }
    }

    public class EfficiencyEnvelope
    {
        public EfficiencyBody Body { get; set; }
    }
}
