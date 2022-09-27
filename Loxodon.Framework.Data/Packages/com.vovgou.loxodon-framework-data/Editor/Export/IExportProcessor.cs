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

using System;
using System.Collections.Generic;

namespace Loxodon.Framework.Data.Editors
{
    public interface IExportProcessor
    {
        void ImportFiles(string inputRoot, Action<string, float> onProgress, Action<List<FileEntry>> onFinished);

        void ExportFiles(string outputRoot, List<FileEntry> files, Action<string, float> onProgress, Action onFinished);
    }

    [Serializable]
    public class FileEntry
    {
        public FileEntry()
        {
        }
        public FileEntry(string filename, List<SheetInfo> sheets)
        {
            this.Filename = filename;
            this.Sheets = sheets;
        }

        public string Filename { get; private set; }

        public List<SheetInfo> Sheets { get; private set; }

        public override bool Equals(object obj)
        {
            if (!(obj is FileEntry))
                return false;

            if (this == obj)
                return true;

            FileEntry entry = (FileEntry)obj;
            if (this.Filename.Equals(entry.Filename))
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return this.Filename.GetHashCode();
        }

        public override string ToString()
        {
            return this.Filename.ToString();
        }
    }

    [Serializable]
    public class SheetInfo
    {
        public SheetInfo()
        {
        }

        public SheetInfo(string name) : this(name, true)
        {
        }

        public SheetInfo(string name, bool isValid)
        {
            this.Name = name;
            this.IsValid = isValid;
        }

        public string Name { get; private set; }

        public bool IsValid { get; set; }
    }

    public enum LineNo
    {
        None = -1,
        Line1,
        Line2,
        Line3,
        Line4,
        Line5,
        Line6,
        Line7,
        Line8
    }
}