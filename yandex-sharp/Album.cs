
using System;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Collections;

namespace Mono.Yandex.Fotki
{
	public class Album:PhotoCollection {
		
		public string id;
		public string title;
		public string author;
		public string summary;
		public int count;
		
		public Album (XPathDocument xml):base (xml)
		{
			ParseXml (xml);
		}
		
		/*
		public void Delete ()
		{
			RequestManager.Delete (this);
		}
		
		public void Update ()
		{
			RequestManager.Edit (this);
		}
		*/
		
		private void ParseXml (XPathDocument doc)
		{
			var nav = doc.CreateNavigator ();
			var mr = new XmlNamespaceManager (nav.NameTable);
			mr.AddNamespace ("atom","http://www.w3.org/2005/Atom");
			mr.AddNamespace ("f","yandex:fotki");
			id = (string)nav.Evaluate ("substring-before(substring-after(/atom:feed/atom:id,':album:'),':photos')",mr);
			title = (string)nav.Evaluate ("string(/atom:feed/atom:title)",mr);
			author = (string)nav.Evaluate ("string(/atom:feed/atom:author/atom:name)",mr);
			summary = (string)nav.Evaluate ("string(/atom:feed/atom:summary)",mr);
			count = int.Parse((string)nav.Evaluate ("string(/atom:feed/f:image-count/@value)",mr));
		}
	}
}
