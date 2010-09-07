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
        enum RequestMethod {PostMultipart, GetBinary};

        class RequestCompletedEventArgs : EventArgs {
                internal RequestMethod RequestMethod { get; private set; }

                internal RequestCompletedEventArgs (RequestMethod requestMethod)
                {
                        RequestMethod = requestMethod;
                }
        }

        class RequestProgressChangedEventArgs : EventArgs {
                internal RequestMethod RequestMethod { get; private set; }
                internal long BytesTransferred { get; private set; }
                internal long TotalBytesToTranfer { get; private set; }

                internal RequestProgressChangedEventArgs (RequestMethod requestMethod, 
                                long bytesTransferred, long totalBytesToTransfer)
                {
                        RequestMethod = requestMethod;
                        BytesTransferred = bytesTransferred;
                        TotalBytesToTranfer = totalBytesToTransfer;
                }
        }

        delegate void RequestCompletedEventHandler (object sender, 
                        RequestCompletedEventArgs args);

        delegate void RequestProgressChangedEventHandler (object sender, 
                        RequestProgressChangedEventArgs args);

        class RequestManager {
                private string token;

                internal event RequestCompletedEventHandler RequestCompleted;
                internal event RequestProgressChangedEventHandler RequestProgressChanged; 

                internal RequestManager () {}

                internal RequestManager (string username, string password)
                {
                        token = Authentication.GetAuthorizationToken (username, password);
                }
                
                internal byte[] GetBinary (string uri) {
                        return GetBinary (uri, false);
                }

                internal byte[] GetBinary (string uri, bool async)
                {
                        HttpWebRequest request = CreateRequest (uri);
                        request.Method = "GET";

                        using (HttpWebResponse response = (HttpWebResponse) request.GetResponse ()) {
                                Stream response_stream = response.GetResponseStream ();
                                byte[] buffer = new byte[1024];

                                using (MemoryStream output = new MemoryStream ()) {
                                        int read;
                                        long readBytes = 0;

                                        while((read = response_stream.Read (buffer,
                                                        0, buffer.Length)) > 0) {
                                                output.Write(buffer, 0, read);
                                                readBytes += read;
                                                if (async)
                                                        OnRequestProgressChanged (
                                                                        new RequestProgressChangedEventArgs (
                                                                                RequestMethod.GetBinary,
                                                                                readBytes,
                                                                                response.ContentLength));
                                        }
                                        if (async)
                                                OnRequestCompleted (new RequestCompletedEventArgs (
                                                                        RequestMethod.GetBinary));

                                        return output.ToArray ();
                                }
                        }
                }

                internal string GetString (string uri)
                {
                        HttpWebRequest request = CreateRequest (uri);
                        request.Method = "GET";

                        using (HttpWebResponse response = (HttpWebResponse) request.GetResponse ()) {
                                Stream response_stream = response.GetResponseStream ();
                                byte [] response_bytes = 
                                        StreamHelper.ReadFully (response_stream);
                                return Encoding.UTF8.GetString (response_bytes);
                        }
                }

                internal void Delete (string uri)
                {
                        HttpWebRequest request = CreateRequest (uri);
                        request.Method = "DELETE";

                        using (HttpWebResponse response = (HttpWebResponse) request.GetResponse ()) {
                                //TODO exception
                        }

                }

                internal string PutAtomEntry (string uri, string xml)
                {
                        HttpWebRequest request = CreateRequest (uri);
                        request.Method = "PUT";
                        request.ContentType = "application/atom+xml; charset=utf-8; type=entry";

                        byte [] xml_bytes = Encoding.UTF8.GetBytes (xml);

                        using (Stream request_stream = request.GetRequestStream ()) {
                                request_stream.Write (xml_bytes, 0, xml_bytes.Length);
                        }

                        return WebHelper.GetResponseString (request);
                }

                internal string PostAtomEntry (string uri, string xml)
                {
                        HttpWebRequest request = CreateRequest (uri);
                        request.Method = "POST";
                        request.ContentType = "application/atom+xml; charset=utf-8; type=entry";

                        byte [] xml_bytes = Encoding.UTF8.GetBytes (xml);

                        using (Stream request_stream = request.GetRequestStream ()) {
                                request_stream.Write (xml_bytes, 0, xml_bytes.Length);
                        }

                        return WebHelper.GetResponseString (request);
                }

                internal string PostMultipart (string uri, MultipartData data)
                {
                        return PostMultipart (uri, data, false);
                }
                internal string PostMultipart (string uri, MultipartData data, bool async)
                {
                        const string boundary = "AaB03x";

                        HttpWebRequest request = CreateRequest (uri);
                        request.Method = "POST";
                        request.ContentType = "multipart/form-data; boundary=" + boundary;
                        Stream content = data.Form (boundary);
                        request.ContentLength = content.Length;
                        request.AllowWriteStreamBuffering = false;
                        Stream request_stream = request.GetRequestStream ();

                        byte[] buffer = new byte[1024];
                        int read;
                        long readBytes = 0;

                        while((read = content.Read(buffer, 0, buffer.Length)) > 0) {
                                request_stream.Write(buffer, 0, read);
                                readBytes += read;
                                if (async) {
                                        OnRequestProgressChanged (
                                                        new RequestProgressChangedEventArgs (
                                                                RequestMethod.PostMultipart,
                                                                readBytes,
                                                                content.Length));
                                }
                        }
                        if (async)
                                OnRequestCompleted (new RequestCompletedEventArgs (
                                                        RequestMethod.PostMultipart));

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

                private void OnRequestCompleted (RequestCompletedEventArgs args)
                {
                        if (RequestCompleted != null)
                                RequestCompleted (this, args);
                }

                private void OnRequestProgressChanged (RequestProgressChangedEventArgs args)
                {
                        if (RequestProgressChanged != null)
                              RequestProgressChanged (this, args);
                }
        }
} 
