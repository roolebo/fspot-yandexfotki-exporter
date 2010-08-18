using System;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Net;

namespace Mono.Yandex.Fotki {
        //without downloading files
        public class DownloadWholePhotoCollection {
                static void Main (string[] args)
                {
                        string link_service_document = 
                                "http://api-fotki.yandex.ru/api/users/";
                        RequestManager request_manager = new RequestManager ();

                        foreach (string user in args) {
                                string user_service_document = link_service_document + user + "/";

                                try {
                                        string photo_collection_content;
                                        string service_document = 
                                                request_manager.GetString (user_service_document);
                                        string photo_collection_url = GetPhotoCollectionUrl (service_document);
                                        Console.WriteLine ("Photo collection URL: {0}", photo_collection_url);
                                        uint page_cnt = 0;
                                        
                                        Console.WriteLine ("Start downloading photo collection from " + user + "...");
                                        DateTime date_start = DateTime.Now;
                                        do {
                                                photo_collection_content = request_manager.GetString (photo_collection_url);
                                                page_cnt++;
                                                //trying to get next page url
                                                Console.Write ("\rDownload {0} page", page_cnt);
                                                photo_collection_url = GetNextPageUrl (photo_collection_content);
                                        } while (!String.IsNullOrEmpty (photo_collection_url));
                                        Console.WriteLine ();
                                        DateTime date_end = DateTime.Now;

                                        TimeSpan time_span = date_end.Subtract (date_start);
                                        Console.WriteLine ("Time of getting {0} pages: {1}", page_cnt, time_span);

                                } catch (WebException exception) {
                                        HttpWebResponse response = (HttpWebResponse) exception.Response;
                                        if (response != null) {
                                                HttpStatusCode status_code = response.StatusCode;

                                                string response_text;
                                                StreamReader reader;
                                                using (reader = new StreamReader (response.GetResponseStream ())) {
                                                        response_text = reader.ReadToEnd ();
                                                }

                                                if (status_code == HttpStatusCode.NotFound)
                                                        throw new UserNotFoundException (response_text);
                                        }

                                        throw new ConnectionFailedException (exception.Message);
                                }

                        }
                }

                static string GetPhotoCollectionUrl (string serviceDocument)
                {
                        var document = new XmlDocument ();
                        document.LoadXml (serviceDocument);
                        var nav = document.CreateNavigator ();
                        var mr = new XmlNamespaceManager (nav.NameTable);
                        mr.AddNamespace ("app","http://www.w3.org/2007/app");
                        mr.AddNamespace ("f","yandex:fotki");
                        mr.AddNamespace ("atom","http://www.w3.org/2005/Atom");

                        return (string) nav.Evaluate ("string(//app:collection[2]/@href)", mr);
                }

                //returns null if there is no next page
                static string GetNextPageUrl (string xml)
                {
                        var document = new XmlDocument ();
                        document.LoadXml (xml);
                        var nav = document.CreateNavigator ();
                        var mr = new XmlNamespaceManager (nav.NameTable);
                        mr.AddNamespace ("app","http://www.w3.org/2007/app");
                        mr.AddNamespace ("f","yandex:fotki");
                        mr.AddNamespace ("atom","http://www.w3.org/2005/Atom");

                        return (string) nav.Evaluate ("string(//atom:link[@rel='next']/@href)", mr);
                }
        }
}
