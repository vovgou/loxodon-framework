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

namespace Loxodon.Framework.Data.Editors
{
    public enum IndexType
    {
        None,
        Index,
        Unique
    }
    public class ColumnInfo
    {
        public ColumnInfo(int columnNo, string columnName, string typeName, string comment) : this(columnNo, columnName, typeName, comment, IndexType.None)
        {
        }
        public ColumnInfo(int columnNo, string columnName, string typeName, string comment, IndexType indexType)
        {
            this.ColumnNo = columnNo;
            this.ColumnName = columnName;
            this.TypeName = typeName;
            this.Comment = comment;
            this.IndexType = indexType;
        }

        public int ColumnNo { get; set; }

        public string ColumnName { get; set; }

        public string TypeName { get; set; }

        public string Comment { get; set; }

        public IndexType IndexType { get; set; }
    }
}
