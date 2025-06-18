using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRest.Service
{
	public class ServiceExternoSegip
    {
		public static string Url = "http://testconsultarui.segip.gob.bo/ServicioExternoInstitucion.svc";

		public static string GetRequestSpeedup(string core, string serialTime, string time)
		{  /// DATA EJEMPLO
			var request = "<soapenv:Envelope xmlns:x='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tem='http://tempuri.org/' xmlns:soa='http://schemas.datacontract.org/2004/07/SoapService'>" +
            "<soapenv:Header/>" +
                "<soapenv:Body>" +
				"<tem:Speedup>" +
				"<tem:data>" +
				"<soa:Core>" + core + "</soa:Core>" +
				"<soa:SerialTime>" + serialTime + "</soa:SerialTime>" +
				"<soa:Time>" + time + "</soa:Time>" +
				"</tem:data>" +
				"</tem:Speedup>" +
                "</soapenv:Body>" +
                "</soapenv:Envelope>";

			return request;
		}

		public static string GetRequestEfficiency(string core, string speedup)
		{
            
                var request = "<x:Envelope xmlns:x='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tem='http://tempuri.org/'>" +
                    "<x:Header/>" +
                        "<x:Body>" +
                            "<tem:ConsultaDatoPersonaCertificacion>" +
                            "<tem:pCodigoInstitucion>175</tem:pCodigoInstitucion>" +
                            "<tem:pUsuario>usuario.sedem</tem:pUsuario>" +
                            "<tem:pContrasenia>Sedem2022</tem:pContrasenia>" +
                            "<tem:pClaveAccesoUsuarioFinal>R1904013411941</tem:pClaveAccesoUsuarioFinal>" +
                            "<tem:pNumeroDocumento>22334455</tem:pNumeroDocumento>" +
                            "<tem:pComplemento>1H</tem:pComplemento>" +
                            "</tem:ConsultaDatoPersonaCertificacion>" +
                        "</x:Body>" +
                    "</x:Envelope>";

            return request;
		}
	}
}
