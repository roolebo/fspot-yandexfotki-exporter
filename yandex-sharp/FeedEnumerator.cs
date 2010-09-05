//
// Mono.Yandex.Fotki.FeedEnumerator.cs
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
using System.Xml;
using System.Xml.XPath;
//using System.IO;
//using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Mono.Yandex.Fotki {
                class FeedEnumerator : IEnumerator<string> {
                        private XmlDocument document;
                        private XmlNamespaceManager manager;
                        private XPathNavigator navigator;
                        private XPathNodeIterator iterator;
                        private XPathExpression entry_expression;
                        private XPathExpression next_page_expression;
                        private string current_page;
                        private string first_page_url;
                        private string next_page_url;
                        private RequestManager request_manager;

                        public FeedEnumerator (string feedUrl, RequestManager requestManager)
                        {
                                if (feedUrl == null || requestManager == null)
                                        throw new ArgumentNullException ();

                                document = new XmlDocument ();
                                entry_expression = XPathExpression.Compile (
                                                "/atom:feed/atom:entry");
                                next_page_expression = XPathExpression.Compile (
                                                "string(/atom:feed/atom:link[@rel='next']/@href)");

                                request_manager = requestManager;
                                next_page_url = first_page_url = feedUrl;
                        }

                        public bool MoveNext ()
                        {
                                bool res = false;
                                if (iterator != null)
                                        res = iterator.MoveNext ();
                                if (res == false && !String.IsNullOrEmpty (next_page_url)) {
                                        current_page = request_manager.GetString (next_page_url);
                                        LoadXml (current_page);
                                        next_page_url = GetNextPageUrl ();
                                        //move to entry on newly loaded page
                                        res = iterator.MoveNext ();
                                }

                                if (res == true)
                                        current_page = iterator.Current.OuterXml;
                                else
                                        current_page = null;

                                return res;
                        }

                        public void Reset ()
                        {
                                next_page_url = first_page_url;
                                current_page = null;
                        }

                        public void Dispose ()
                        {
                        }

                        public string Current {
                                get {
                                        if (current_page == null)
                                                throw new InvalidOperationException ();
                                        return current_page;
                                }
                        }

                        object IEnumerator.Current {
                                get {
                                        return Current;
                                }
                        }

                        string GetNextPageUrl ()
                        {
                                return (string) navigator.Evaluate (next_page_expression);
                        }

                        void LoadXml (string xml)
                        {
                                document.LoadXml (xml);
                                navigator = document.CreateNavigator ();

                                manager = new XmlNamespaceManager (navigator.NameTable);
                                //manager.AddNamespace ("app","http://www.w3.org/2007/app");
                                //manager.AddNamespace ("f","yandex:fotki");
                                manager.AddNamespace ("atom","http://www.w3.org/2005/Atom");

                                entry_expression.SetContext (manager);
                                next_page_expression.SetContext (manager);

                                iterator = navigator.Select (entry_expression);
                        }
                }
}
