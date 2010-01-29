using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Yandex.Fotki{
	public class Service{
		public string user;
		public string password;
		public PhotoCollection photos;
		public AlbumCollection albums;

		public Service (string user)
		{
			this.user = user;
		}
		
		public Service (string user, string password):this (user)
		{
			this.password = password;
		}
		
		private void DownloadPhotoCollection ()
		{}
		
		private void DownloadAlbumCollection ()
		{}

		private void ParseXml (string xml)
		{
			var sr = new StringReader (xml);
			var doc = new XPathDocument (sr);
			var nav = doc.CreateNavigator ();
			var iterator = nav.Select("//app:collection/@href");
			albums.link = iterator.Current.ToString ();
			iterator.MoveNext ();
			photos.link = iterator.Current.ToString ();
		}
	}
}