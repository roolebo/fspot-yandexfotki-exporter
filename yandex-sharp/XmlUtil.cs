
using System;
using System.IO;
using System.Xml;
using System.Text;

namespace Mono.Yandex.Fotki{
	
	public class XmlUtil{
		StringWriter sr;
		XmlTextWriter writer;
		
		public XmlUtil ()
		{
			sr = new StringWriter ();
			writer = XmlTextWriter (sr);
			writer.Formatting = Formatting.Indented;
			writer.Indentation = 2;
		}
		
		public void WriteElementString (string name, string val)
		{
			writer.WriteElementString (name, val);
		}

		public void WriteStartElement (string elem)
		{
			writer.WriteStartElement (elem, null);
		}
		
		public void WriteAttributeString (string prefix, string name, string ns, string val)
		{
			writer.WriteAttributeString (prefix, name, ns, val);
		}
		
		public string GetDocumentString ()
		{
			writer.Close ();
			return sr.ToString ();
		}
	}
}
