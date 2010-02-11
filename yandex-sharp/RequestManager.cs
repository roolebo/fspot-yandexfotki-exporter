//  
// Mono.Yandex.Fotki.RequestManager.cs: Performs GET, POST, PUT, DELETE requests in convenient way
//
// Author:
//    Roman Bolshakov
//
// Copyright (C) 2010 Roman Bolshakov
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Collections.Generic;
//TODO exception handling
namespace Mono.Yandex.Fotki {
        class RequestManager {
                private string token;
                
                internal RequestManager (string username, string password)
                {
                        token = Authentication.GetAuthorizationToken (username, password);
                }
                
                internal byte[] GetBinary (string uri)
                {
                        HttpWebRequest request = CreateRequest (uri);
                        request.Method = "GET";

                        using (HttpWebResponse response = (HttpWebResponse) request.GetResponse ()) {
                                return StreamHelper.ReadFully (response.GetResponseStream ());
                        }
                }

                internal string Get (string uri)
                {
                        return Encoding.UTF8.GetString (GetBinary (uri));
                }

                internal void Delete (string uri)
                {
                        HttpWebRequest request = CreateRequest (uri);
                        request.Method = "DELETE";

                        using (HttpWebResponse response = (HttpWebResponse) request.GetResponse ()) {
                                //TODO exception
                        }

                }

                internal string Put (string uri, IContent content)
                {
                        HttpWebRequest request = CreateRequest (uri);
                        request.Method = "PUT";
                        request.ContentType = content.Type;

                        Stream content_stream = content.GetStream ();
                        Stream request_stream = request.GetRequestStream ();
                        StreamHelper.CopyStream (content_stream, request_stream);
                        request_stream.Close ();
                        content_stream.Close ();

                        return WebHelper.GetResponseString (request);
                }

                internal string Post (string uri, IContent content)
                {
                        HttpWebRequest request = CreateRequest (uri);
                        request.Method = "POST";

                        if (content.Name != null)
                                request.Headers.Add ("Slug: " + content.Name);
                        request.ContentType = content.Type;

                        Stream content_stream = content.GetStream ();
                        Stream request_stream = request.GetRequestStream ();
                        StreamHelper.CopyStream (content_stream, request_stream);
                        request_stream.Close ();
                        content_stream.Close ();

                        return WebHelper.GetResponseString (request);
                }

                internal string PostMultipart (string uri, MultipartData data)
                {
                        const string boundary = "AaB03x";

                        HttpWebRequest request = CreateRequest (uri);
                        request.Method = "POST";
                        request.ContentType = "multipart/form-data; boundary=" + boundary;
                        Stream content = data.Form (boundary);
                        Stream request_stream = request.GetRequestStream ();
                        StreamHelper.CopyStream(content, request_stream);
                        request_stream.Close ();
                        content.Close ();

                        return WebHelper.GetResponseString (request);
                }

                private HttpWebRequest CreateRequest (string uri)
                {
                        HttpWebRequest request = (HttpWebRequest) WebRequest.Create (uri);
                        if (token != null)
                                request.Headers.Add (@"Authorization: FimpToken realm=""fotki.yandex.ru"", token="""
                                                + token + @"""");
                        return request;
                }
        }
} 
