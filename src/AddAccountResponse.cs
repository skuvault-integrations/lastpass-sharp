using System.Xml.Serialization;

namespace SkuVault.LastPass
{
	[XmlRoot( "xmlresponse" )]
	public class AddAccountResponseWrapper
	{
		[XmlElement("result")]
		public AddAccountResponse Result;
	}

	public class AddAccountResponse
	{
		[XmlAttribute(attributeName: "action")]
		public string Action;	//"added"

		[XmlAttribute(attributeName: "aid")]
		public string AccountId;

		[XmlAttribute(attributeName: "msg")]
		public string Message;	//"accountadded"
	}
}
