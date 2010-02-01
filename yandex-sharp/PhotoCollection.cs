
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace Mono.Yandex.Fotki{
	
	public class PhotoCollection:IEnumerable<Photo> {
		public string link;
		private ArrayList photos;

		public PhotoCollection (XPathDocument xml)
		{
			photos = new ArrayList ();
			ParseXml (xml);
		}
		/*
		public void Upload (Photo photo)
		{
			RequestManager.Add (photo);
		}
		*/
		
		public IEnumerator<Photo> GetEnumerator ()
		{
			foreach (Photo photo in photos)
			{
				yield return photo;
			}
		}
		
		private void ParseXml (XPathDocument xml)
		{
			var nav = xml.CreateNavigator ();
			
			var iterator = nav.Select ("//entry");
			foreach (XPathNodeIterator item in iterator){
				photos.Add (new Photo (new XPathDocument 
				                       (new StringReader (item.ToString ()))));
			}
		}
		
	}
}
