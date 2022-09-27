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

using LiteDB;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Loxodon.Framework.Data.Editors
{
    public class LiteDBExportProcessor : ExportProcessor
    {
        [SerializeField]
        protected string liteDbFilename = "data.db";

        protected override void DoExportSheet(FileInfo file, ISheet sheet, ISheetReader reader, string outputRoot)
        {
            reader.Headers.ForEach(c =>
            {
                if (c.ColumnName.Equals("id"))
                    c.ColumnName = "_id";
            });

            string dbFullname = Path.Combine(outputRoot, liteDbFilename);
            string tableName = sheet.SheetName;
            using (var db = new LiteDatabase(dbFullname))
            {
                var liteCollection = db.GetCollection(tableName);
                liteCollection.DeleteAll();
                foreach (ColumnInfo columnInfo in reader.Headers)
                {
                    if (columnInfo.IndexType != IndexType.None)
                        liteCollection.EnsureIndex(columnInfo.ColumnName, columnInfo.IndexType == IndexType.Unique ? true : false);
                }

                List<BsonDocument> list = Parse(reader);
                liteCollection.Insert(list);
            }
            Debug.LogFormat("File:{0} Sheet:{1} OK……", GetRelativePath(file.FullName), sheet.SheetName);
        }

        protected virtual List<BsonDocument> Parse(ISheetReader reader)
        {
            BsonMapper mapper = BsonMapper.Global;
            List<BsonDocument> list = new List<BsonDocument>();
            for (int i = reader.StartLine; i <= reader.TotalCount; i++)
            {
                var data = reader.ReadLine(i);
                if (data == null)
                    continue;

                list.Add(mapper.ToDocument(data));
            }
            return list;
        }

        //protected virtual BsonDocument ToBsonDocument(Dictionary<string, object> data)
        //{
        //    BsonDocument document = new BsonDocument();
        //    foreach (var kv in data)
        //        document.Add(kv.Key, ToBsonValue(kv.Value));
        //    return document;
        //}

        //protected virtual BsonValue ToBsonValue(object value)
        //{
        //    if (value == null)
        //        return BsonValue.Null;
        //    if (value.GetType().IsPrimitive)
        //    {
        //        if (value is float)
        //            return new BsonValue(Convert.ChangeType(value, typeof(double)));
        //        return new BsonValue(value);
        //    }
        //    else if (value.GetType().IsArray)
        //        return ToBsonArray(value);
        //    else if (value is IDictionary)
        //        return ToBsonDocument((IDictionary)value);
        //    return new BsonValue(value);
        //}

        //protected virtual BsonArray ToBsonArray(object value)
        //{
        //    if (value is string[])
        //    {
        //        List<BsonValue> list = ((string[])value).AsEnumerable().Select(i => new BsonValue(i)).ToList();
        //        return new BsonArray(list);
        //    }
        //    else if (value is int[])
        //    {
        //        List<BsonValue> list = ((int[])value).AsEnumerable().Select(i => new BsonValue(i)).ToList();
        //        return new BsonArray(list);
        //    }
        //    else if (value is float[])
        //    {
        //        List<BsonValue> list = ((float[])value).AsEnumerable().Select(i => new BsonValue(i)).ToList();
        //        return new BsonArray(list);
        //    }
        //    else if (value is bool[])
        //    {
        //        List<BsonValue> list = ((bool[])value).AsEnumerable().Select(i => new BsonValue(i)).ToList();
        //        return new BsonArray(list);
        //    }

        //    throw new NotSupportedException(string.Format("An unsupported data type:{0}", value.GetType()));
        //}

        //protected virtual BsonDocument ToBsonDocument(object value)
        //{
        //    if (value is Dictionary<string, string>)
        //    {
        //        BsonDocument document = new BsonDocument();
        //        foreach (var kv in (Dictionary<string, string>)value)
        //        {
        //            document.Add(kv.Key, kv.Value);
        //        }
        //        return document;
        //    }
        //    else if (value is Dictionary<string, int>)
        //    {
        //        BsonDocument document = new BsonDocument();
        //        foreach (var kv in (Dictionary<string, int>)value)
        //        {
        //            document.Add(kv.Key, kv.Value);
        //        }
        //        return document;
        //    }
        //    else if (value is Dictionary<string, float>)
        //    {
        //        BsonDocument document = new BsonDocument();
        //        foreach (var kv in (Dictionary<string, float>)value)
        //        {
        //            document.Add(kv.Key, kv.Value);
        //        }
        //        return document;
        //    }
        //    else if (value is Dictionary<string, bool>)
        //    {
        //        BsonDocument document = new BsonDocument();
        //        foreach (var kv in (Dictionary<string, bool>)value)
        //        {
        //            document.Add(kv.Key, kv.Value);
        //        }
        //        return document;
        //    }
        //    else if (value is Dictionary<string, object>)
        //    {
        //        return ToBsonDocument((Dictionary<string, object>)value);
        //    }

        //    throw new NotSupportedException(string.Format("An unsupported data type:{0}", value.GetType()));
        //}
    }
}