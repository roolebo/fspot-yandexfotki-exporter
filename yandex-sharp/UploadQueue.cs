//
// Mono.Yandex.Fotki.UploadQueue.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Web;

namespace Mono.Yandex.Fotki {
        class UploadQueue {
                private static UploadQueue instance;
                private FotkiService fotki;
                private Queue<Photo> photos_to_upload;
                private Thread upload_thread;

                private UploadQueue (FotkiService fotki)
                {
                        this.fotki = fotki;
                        photos_to_upload = new Queue<Photo> ();
                }

                internal static UploadQueue Instance (FotkiService fotki)
                {
                        if (instance == null)
                                instance = new UploadQueue (fotki);
                        return instance;
                }

                internal void Add (Photo photo)
                {
                        lock (photos_to_upload) {
                                photos_to_upload.Enqueue (photo);
                        }

                        if (upload_thread == null || !upload_thread.IsAlive) {
                                upload_thread = new Thread (UploadPhotosFromQueue);
                                upload_thread.Start ();
                        }
                }

                private void UploadPhotosFromQueue ()
                {
                        Photo photo, uploaded_photo;

                        Monitor.Enter (photos_to_upload);
                        while (photos_to_upload.Count > 0) {
                                Monitor.Exit (photos_to_upload);

                                photo = photos_to_upload.Dequeue ();
                                MultipartData data = MultipartDataHelper.BuildMultipartData (photo);
                                string response = fotki.Request.PostMultipart (
                                                PhotoCollection.post_uri, data, true);
                                uint image_id = uint.Parse ((HttpUtility.ParseQueryString
                                                                (response)) ["image_id"]);
                                uploaded_photo = fotki.GetPhotos ().GetPhoto (image_id);
                                var args = new UploadPhotoCompletedEventArgs (
                                                uploaded_photo);
                                fotki.OnUploadPhotoCompleted (args);

                                Monitor.Enter (photos_to_upload);
                        }

                        Monitor.Exit (photos_to_upload);
                }
        }
}
