//  
// Mono.Yandex.Fotki.DateTimeHelper.cs:
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
using System.Globalization;

namespace Mono.Yandex.Fotki {
        class DateTimeHelper {
                internal static DateTime ConvertRfc3339ToDateTime(string datetime)
                {
                        string[] date_format = {"yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'",
                                "yyyy'-'MM'-'dd'T'HH':'mm':'ss"};
                        if (datetime.EndsWith ("Z"))
                                return DateTime.ParseExact (datetime, date_format[0], 
                                                null, DateTimeStyles.AssumeUniversal);
                        else
                                return DateTime.ParseExact (datetime, date_format[1], 
                                                null);
                }
        }
}
