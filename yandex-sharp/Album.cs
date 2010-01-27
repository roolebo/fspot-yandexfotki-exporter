
using System;

namespace Mono.Yandex.Folki
{
	public class Album{
		private Connection conn;
		private string id;
		public string title;
		public string summary;
		public string password;
		
		public Album (Connection conn)
		{
			this.conn = conn;
		}
	}
}
