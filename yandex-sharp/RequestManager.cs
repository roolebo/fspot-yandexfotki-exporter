using System;
using System.Net;

namespace Mono.Yandex.Fotki {

    public class RequestManager {
	        WebClient web_client = new WebClient ();
	
	        public RequestManager ()
	        {
	        }
	
	        public void Add (Photo photo)
	        {
	        }
	
	        public void Add (Album album)
	        {
	        }
	
	        public void Delete (Photo photo)
	        {
	        }
	
	        public void Delete (Album album)
	        {
	        }
	
	        public byte[] Download (Photo photo)
	        {
	            return null;
	        }
	
	        public void Edit (Photo photo)
	        {
	        }
	
	        public void Edit (Album album)
	        {
	        }
	
	        public void Get (Photo photo)
	        {
	        }
	
	        public void Get (Album album)
	        {
	        }
		
		public static XPathDocument GetServiceDocument ()
		{}
		
		public static XPathDocument GetPhotoCollection (string url)
		{}
		
		public static XPathDocument GetAlbumCollection (string url)
		{}
	
	}
}
