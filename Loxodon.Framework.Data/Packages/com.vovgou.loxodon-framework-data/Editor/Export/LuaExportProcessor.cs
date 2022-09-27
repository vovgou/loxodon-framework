/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Loxodon.Framework.Data.Editors
{
    public class LuaExportProcessor : ExportProcessor
    {
        protected virtual string GenerateFilename(string outputRoot, FileInfo file, ISheet sheet)
        {
            string filename = string.Format("{0}.lua", sheet.SheetName).ToLower();
            return Path.Combine(outputRoot, filename);
        }

        protected override void DoExportSheet(FileInfo file, ISheet sheet, ISheetReader reader, string outputRoot)
        {
            string fullname = this.GenerateFilename(outputRoot, file, sheet);
            StringBuilder text = Parse(reader);
            File.WriteAllText(fullname, text.ToString());
            Debug.LogFormat("File:{0} Sheet:{1} OK……", GetRelativePath(file.FullName), sheet.SheetName);
        }

        protected virtual StringBuilder Parse(ISheetReader reader)
        {
            StringBuilder buf = new StringBuilder();
            int curr = 0;

            buf.Append("---\r\n");
            buf.AppendFormat("--@module {0}\r\n", reader.Name);
            buf.Append("local M = {\r\n");
            for (int i = reader.StartLine; i <= reader.TotalCount; i++)
            {
                var data = reader.ReadLine(i);
                if (data == null)
                    continue;

                if (curr > 0)
                    buf.Append(",\r\n");

                buf.Append("  ");
                ToLua(data, buf);
                curr++;
            }
            buf.Append("\r\n}");
            buf.Append("\r\n");
            buf.Append("return M");
            return buf;
        }

        protected virtual void ToLua(IDictionary data, StringBuilder buf)
        {
            int count = data.Count;
            List<string> keys = new List<string>();
            foreach (var key in data.Keys)
                keys.Add((string)key);

            buf.Append("{ ");
            for (int i = 0; i < count; i++)
            {
                var key = keys[i];
                var value = data[key];

                buf.AppendFormat("{0}=", key);
                if (value is IList)
                {
                    ToLua((IList)value, buf);
                }
                else if (value is IDictionary)
                {
                    ToLua((IDictionary)value, buf);
                }
                else if (value is string)
                {
                    buf.AppendFormat("\"{0}\"", StringEscapeUtils.Escape((string)value));
                }
                else
                {
                    buf.AppendFormat("{0}", value);
                }

                if (i < count - 1)
                    buf.Append(",");
            }
            buf.Append(" }");
        }

        protected virtual void ToLua(IList data, StringBuilder buf)
        {
            int count = data.Count;
            buf.Append("[ ");
            for (int i = 0; i < count; i++)
            {
                var value = data[i];
                if (value is IList)
                {
                    ToLua((IList)value, buf);
                }
                else if (value is IDictionary)
                {
                    ToLua((IDictionary)value, buf);
                }
                else if (value is string)
                {
                    buf.AppendFormat("\"{0}\"", StringEscapeUtils.Escape((string)value));
                }
                else
                {
                    buf.AppendFormat("{0}", value);
                }

                if (i < count - 1)
                    buf.Append(",");
            }
            buf.Append(" ]");
        }
    }
}
