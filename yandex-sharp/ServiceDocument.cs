using System;
using System.

namespace Mono.Yandex.Fotki{
	public class ServiceDocument{
		Connection conn;
		public string user;
		public string albums;
		public string photos;
		
		public ServiceDocument(Connection conn)
		{
			this.conn = conn;
		}
		
		public ServiceDocument(Connection conn, string user):this(conn)
		{
			this.user = user;
		}
		
		private void ParseXml(string xml)
		{
						
		}
	}
}
