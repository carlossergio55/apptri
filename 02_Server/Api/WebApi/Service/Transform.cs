using Newtonsoft.Json;
using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ApiRest.Service
{
	public static class Transform
    {
		public static string Exec(string soapResponse)
		{
			var xm = XElement.Parse(soapResponse);
			var response = RemoveAllNamespacesXml(xm).CleanXml().ToJson();

			return response;
		}

		private static string RemoveAllNamespacesXml(XElement xmlDocument)
		{
			try
			{
				if (!xmlDocument.HasElements)
				{
					var xElement = new XElement(xmlDocument.Name.LocalName);
					xElement.Value = xmlDocument.Value;

					foreach (XAttribute attribute in xmlDocument.Attributes())
						xElement.Add(attribute);

					return xElement.ToString();
				}
				return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespacesXml(el))).ToString();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception in T Remove Namespaces XML: " + ex.Message);
				return "";
			}
		}

		private static string ToJson(this string soapResponse)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(soapResponse);

			return JsonConvert.SerializeXmlNode(xmlDoc);
		}

		private static string CleanXml(this string soapResponse)
		{
			soapResponse = soapResponse.Replace("amp;", "").Replace("&lt;", "<").Replace("&gt;", ">");
			return soapResponse;
		}
	}
}
