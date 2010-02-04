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
                
                RequestManager (string username, string password)
                {
                        token = Authentication.GetAuthorizationToken (username, password);
                }
                
                byte[] GetBinary (string uri)
                {
                        HttpWebRequest request = (HttpWebRequest) WebRequest.Create (uri);

                        using (HttpWebResponse response = (HttpWebResponse) request.GetResponse ()) {
                                //TODO change reading to correct way
                                //Stream response_stream = response.GetResponseStream ();
                                //byte[] result = new byte[response_stream.Length];
                                //response_stream.Read (result, 0, result.Length);
                                return null;
                        }
                }

                string Get (string uri)
                {
                        return Encoding.UTF8.GetString (GetBinary (uri));
                }

                string PostMultipart (string uri, MultipartData data)
                {
                        const string boundary = "AaB03x";

                        HttpWebRequest request = (HttpWebRequest) WebRequest.Create (uri);
                        request.Method = "POST";
                        request.Headers.Add(@"Authorization: FimpToken realm=""fotki.yandex.ru"", token="""
                                        + token + @"""");
                        request.ContentType = "multipart/form-data; boundary=" + boundary;
                        Stream content = data.Form (boundary);
                        Stream request_stream = request.GetRequestStream ();
                        CopyStream(content, request_stream);
                        request_stream.Close ();
                        content.Close ();

                        using (HttpWebResponse response = (HttpWebResponse) request.GetResponse ()) {
                                Stream response_stream = response.GetResponseStream ();
                                StreamReader reader = new StreamReader (response_stream);
                                return reader.ReadToEnd ();
                        }
                }
                public static void CopyStream(Stream input, Stream output)
                {
                        byte[] buffer = new byte[32768];
                        while (true) {
                                int read = input.Read (buffer, 0, buffer.Length);
                                if (read <= 0)
                                        return;
                                output.Write (buffer, 0, read);
                        }
                }
        }
} 
