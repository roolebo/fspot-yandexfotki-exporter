//  
// Mono.Yandex.Fotki.MultipartData.cs: Represents multipart/form-data content
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
using System.Text;
using System.Collections.Generic;

namespace Mono.Yandex.Fotki {
        class MultipartData {
                private Parameter file_parameter;
                private HashSet<Parameter> fields = new HashSet<Parameter> ();

                public void Add (Parameter parameter)
                {
                        if (parameter.Type == Parameter.ParamType.File)
                                file_parameter = parameter;
                        else
                                fields.Add(parameter);
                }

                public Stream Form (string boundary)
                {
                        Stream data_stream = new MemoryStream ();
                        byte[] end_boundary = Encoding.UTF8.GetBytes ("--" + boundary + "--\r\n");

                        FormFields (data_stream, boundary);
                        FormFile (data_stream, boundary);
                        data_stream.Write (end_boundary, 0, end_boundary.Length);
                        data_stream.Seek (0, 0);

                        return data_stream;
                }

                private void FormFields (Stream stream, string boundary)
                {
                        //TODO Set encoding
                        string field_template = "--" + boundary + "\r\n"
                                + @"Content-Disposition: form-data; name=""{0}""" + "\r\n"
                                + "\r\n"
                                + @"{1}" + "\r\n";

                        foreach(Parameter field in fields) {
                                string formatted_field_template = String.Format (field_template, field.Name, field.Value);
                                byte[] field_bytes = Encoding.UTF8.GetBytes (formatted_field_template);

                                stream.Write (field_bytes, 0, field_bytes.Length);
                        }
                }

                private void FormFile (Stream stream, string boundary)
                {
                        if(file_parameter == null)
                                return;
                        
                        string file_header_template = "--" + boundary + "\r\n"
                                + @"Content-Disposition: form-data; name=""{0}""; filename=""{1}""" + "\r\n"
                                + "Content-Type: " + WebHelper.GetMimeType (file_parameter.Value) + "\r\n"
                                + "\r\n";
                        byte[] crlf_bytes = Encoding.UTF8.GetBytes ("\r\n");

                        string filename = Path.GetFileName (file_parameter.Value);
                        string file_header = String.Format (file_header_template, 
                                        file_parameter.Name, filename);
                        byte[] file_header_bytes = Encoding.UTF8.GetBytes (file_header);
                        stream.Write (file_header_bytes, 0, file_header_bytes.Length);

                        using (FileStream image = new FileStream (file_parameter.Value, FileMode.Open)) {
                                StreamHelper.CopyStream (image, stream);
                        }
                        stream.Write (crlf_bytes, 0, crlf_bytes.Length);
                }


                public class Parameter {
                        private string parameter_name;
                        private string parameter_value;
                        private ParamType parameter_type;
                        public enum ParamType {File, Field};
                        
                        public Parameter (string parameterName, string parameterValue, ParamType parameterType)
                        {
                                parameter_name = parameterName;
                                parameter_value = parameterValue;
                                parameter_type = parameterType;

                                if (parameter_type == ParamType.File) {
                                        if (!File.Exists (parameter_value))
                                                throw new ArgumentException("File is not exist under the path", 
                                                                "parameterValue");
                                }
                        }

                        public string Name {
                                get { return parameter_name; }
                        }

                        public string Value {
                                get { return parameter_value; }
                        }

                        public ParamType Type {
                                get { return parameter_type; }
                        }

                        public override bool Equals (Object obj)
                        {
                                if (obj == null) return false;

                                if (this.GetType () != obj.GetType ())
                                        return false;
                                
                                Parameter param = (Parameter) obj;

                                if (!(Object.Equals (param.Name, Name) && 
                                                Object.Equals (param.Value, Value) &&
                                                Object.Equals (param.Type, Type)))
                                        return false;

                                return true;
                        }
                }
        }
}
