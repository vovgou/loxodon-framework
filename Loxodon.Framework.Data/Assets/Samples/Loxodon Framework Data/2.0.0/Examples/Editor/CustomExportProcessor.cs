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

//using Loxodon.Framework.Data.Editors;
//using Newtonsoft.Json;
//using NPOI.SS.UserModel;
//using System.IO;
//using System.Text;
//using UnityEngine;

//namespace Loxodon.Framework.Examples.Data
//{
//    public class CustomExportProcessor : ExportProcessor
//    {
//        protected override bool Filter(FileInfo file, ISheet sheet)
//        {
//            //自定义Sheet表单过滤方法，只导出第一个Sheet
//            var workbook = sheet.Workbook;
//            if (workbook.GetSheetIndex(sheet) != 0)
//                return false;
//            return true;
//        }

//        protected override void DoExportSheet(FileInfo file, ISheet sheet, ISheetReader reader, string outputRoot)
//        {
//            string fullname = this.NameGenerator.Generate(outputRoot, file, sheet, "json");
//            StringBuilder text = Parse(reader);
//            File.WriteAllText(fullname, text.ToString());
//            Debug.LogFormat("File:{0} Sheet:{1} OK……", GetRelativePath(file.FullName), sheet.SheetName);
//        }

//        protected StringBuilder Parse(ISheetReader reader)
//        {
//            StringBuilder buf = new StringBuilder();
//            for (int i = reader.StartLine; i <= reader.TotalCount; i++)
//            {
//                var data = reader.ReadLine(i);
//                if (data == null)
//                    continue;

//                string json = JsonConvert.SerializeObject(data, Formatting.None);
//                buf.AppendLine(json);
//            }
//            return buf;
//        }
//    }
//}
