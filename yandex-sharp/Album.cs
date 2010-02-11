
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
		
		private bool isPhotoLoaded = false;
		
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
			
			id = (string)nav.Evaluate ("substring-after(//atom:id[1],':album:')",mr);
			if (id.EndsWith (":photos")){
				id = (string)nav.Evaluate ("substring-before('"+id+"',':photos')",mr);
			}
			title = (string)nav.Evaluate ("string(//atom:title[1])",mr);
			author = (string)nav.Evaluate ("string(//atom:author[1]/atom:name)",mr);
			summary = (string)nav.Evaluate ("string(//atom:summary[1])",mr);
			count = int.Parse((string)nav.Evaluate ("string(//f:image-count/@value)",mr));
			
			if(string.Compare((string)nav.Evaluate ("name(/*)"),"feed") == 0){
				this.isPhotoLoaded = true;
			}
		}
		
		public bool IsPhotoLoaded (){
			return this.isPhotoLoaded;
		}
	}
}
