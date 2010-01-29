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
		private string link_photos;
		private string link_albums;

		public Service (string user)
		{
			this.user = user;
		}
		
		public Service (string user, string password):this (user)
		{
			this.password = password;
		}
		
		private void GetPhotoCollection ()
		{
			var xml = RequestManager.GetPhotoCollection (link_photos);
			photos = new PhotoCollection (xml);
		}
		
		private void GetAlbumCollection ()
		{
			var xml = RequestManager.GetAlbumCollection (link_albums);
			albums = new AlbumCollection (xml);
		}

		private void ParseXml (XPathDocument xml)
		{
			var nav = xml.CreateNavigator ();
			var iterator = nav.Select("//app:collection/@href");
			link_albums = iterator.Current.ToString ();
			iterator.MoveNext ();
			link_photos = iterator.Current.ToString ();
		}
	}
}