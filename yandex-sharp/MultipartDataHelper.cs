//
// Mono.Yandex.Fotki.MultipartDataHelper.cs
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

namespace Mono.Yandex.Fotki {
        class MultipartDataHelper {
                internal static MultipartData BuildMultipartData (Photo photo)
                {
                        MultipartData data = new MultipartData ();
                        data.Add (new MultipartData.Parameter ("image", photo.Filepath,
                                                MultipartData.Parameter.ParamType.File));
                        data.Add (new MultipartData.Parameter ("title", photo.Title,
                                                MultipartData.Parameter.ParamType.Field));
                        data.Add (new MultipartData.Parameter ("access_type", Photo.ToString (photo.AccessLevel),
                                                MultipartData.Parameter.ParamType.Field));
                        data.Add (new MultipartData.Parameter ("disable_comments",
                                                photo.DisableComments.ToString ().ToLower (),
                                                MultipartData.Parameter.ParamType.Field));
                        data.Add (new MultipartData.Parameter ("hide_orignal",
                                                photo.HideOriginal.ToString ().ToLower (),
                                                MultipartData.Parameter.ParamType.Field));
                        data.Add (new MultipartData.Parameter ("xxx",
                                                photo.AdultContent.ToString ().ToLower (),
                                                MultipartData.Parameter.ParamType.Field));
                        return data;
                }
        }
}
