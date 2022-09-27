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

using Loxodon.Framework.Editors;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Loxodon.Framework.Data.Editors
{
    public abstract class ExportProcessor : ScriptableObject, IExportProcessor
    {
        protected readonly string[] EXTENSIONS = new string[] { "xls", "xlsx", "xlsm" };

        [SerializeField]
        protected LineNo columnNameLine;
        [SerializeField]
        protected LineNo typeNameLine;
        [SerializeField]
        protected LineNo commentLine;
        [SerializeField]
        protected LineNo dataStartLine;

        [NonSerialized]
        protected string projectPath;

        public ExportProcessor()
        {
            this.columnNameLine = LineNo.Line1;
            this.typeNameLine = LineNo.Line2;
            this.commentLine = LineNo.Line3;
            this.dataStartLine = LineNo.Line4;
        }

        public virtual void ImportFiles(string inputRoot, Action<string, float> onProgress, Action<List<FileEntry>> onFinished)
        {
            EditorExecutors.RunOnCoroutine(DoImportFiles(inputRoot, onProgress, onFinished));
        }

        protected virtual IEnumerator DoImportFiles(string inputRoot, Action<string, float> onProgress, Action<List<FileEntry>> onFinished)
        {
            List<FileEntry> entries = new List<FileEntry>();
            DirectoryInfo dir = new DirectoryInfo(inputRoot);
            FileInfo[] files = dir.EnumerateFiles("*.*", SearchOption.AllDirectories).Where(file => Filter(file)).ToArray();
            if (files == null || files.Length <= 0)
            {
                if (onFinished != null)
                    onFinished(entries);
                yield break;
            }

            int total = files.Length;
            for (int i = 0; i < total; i++)
            {
                var file = files[i];
                if (!file.Exists)
                    continue;

                try
                {
                    string filename = this.GetRelativePath(file.FullName);
                    List<SheetInfo> sheets = new List<SheetInfo>();

                    IWorkbook workbook = Open(file);
                    try
                    {
                        if (workbook.NumberOfSheets <= 0)
                            throw new NotSupportedException(string.Format("The file \"{0}\" is not supported.", filename));

                        for (int j = 0; j < workbook.NumberOfSheets; j++)
                        {
                            ISheet sheet = workbook.GetSheetAt(j);
                            bool isValid = Filter(file, sheet);
                            sheets.Add(new SheetInfo(sheet.SheetName, isValid));
                        }
                        entries.Add(new FileEntry(filename, sheets));
                    }
                    finally
                    {
                        if (workbook != null)
                            workbook.Close();
                    }
                }
                catch (Exception e)
                {
                    if (e is IOException)
                        Debug.LogErrorFormat("Please check if the file is opened by another application.Error:{0}", e);
                    else
                        Debug.LogErrorFormat("{0}", e);

                    if (onFinished != null)
                        onFinished(null);
                    yield break;
                }

                if (onProgress != null)
                    onProgress(string.Format("Importing file:{0}", file.Name), i / (float)total);

                yield return null;
            }

            if (onFinished != null)
                onFinished(entries);
        }

        public virtual void ExportFiles(string outputRoot, List<FileEntry> files, Action<string, float> onProgress, Action onFinished)
        {
            EditorExecutors.RunOnCoroutine(DoExportFiles(outputRoot, files, onProgress, onFinished));
        }

        protected virtual IEnumerator DoExportFiles(string outputRoot, List<FileEntry> files, Action<string, float> onProgress, Action onFinished)
        {
            int total = files.Count;
            for (int i = 0; i < total; i++)
            {
                FileEntry fileEntry = files[i];
                FileInfo file = new FileInfo(fileEntry.Filename);
                if (file.Exists)
                {
                    IWorkbook workbook = Open(file);
                    try
                    {
                        for (int j = 0; j < workbook.NumberOfSheets; j++)
                        {
                            ISheet sheet = workbook.GetSheetAt(j);
                            try
                            {
                                if (fileEntry.Sheets.Exists(info => info.Name.Equals(sheet.SheetName) && !info.IsValid))
                                    continue;

                                var reader = new NPOISheetReader(sheet, (int)columnNameLine, (int)typeNameLine, (int)commentLine, (int)dataStartLine);
                                this.DoExportSheet(file, sheet, reader, outputRoot);
                            }
                            catch (Exception e)
                            {
                                Debug.LogErrorFormat("File:{0} Sheet:{1} Failed，Exception:{2}", GetRelativePath(file.FullName), sheet.SheetName, e);
                            }

                            if (onProgress != null)
                                onProgress(string.Format("Exporting file:{0}", file.Name), i / (float)total);

                            yield return null;
                        }
                    }
                    finally
                    {
                        if (workbook != null)
                            workbook.Close();
                    }
                }
            }

            if (onFinished != null)
                onFinished();
        }

        protected virtual IWorkbook Open(FileInfo file)
        {
            IWorkbook workbook = null;
            if (file.Extension.Equals(".xlsx"))
                workbook = new XSSFWorkbook(file.OpenRead());
            else if (file.Extension.Equals(".xlsm"))
                workbook = new XSSFWorkbook(file.OpenRead());
            else if (file.Extension.Equals(".xls"))
                workbook = new HSSFWorkbook(file.OpenRead());
            return workbook;
        }

        protected abstract void DoExportSheet(FileInfo file, ISheet sheet, ISheetReader reader, string outputRoot);

        protected virtual bool Filter(FileInfo file)
        {
            return EXTENSIONS.Contains(file.Extension.ToLower().Replace(".", ""));
        }

        protected virtual bool Filter(FileInfo file, ISheet sheet)
        {
            return true;
        }

        protected virtual string GetRelativePath(string path)
        {
            if (string.IsNullOrEmpty(projectPath))
                this.projectPath = Path.GetFullPath(".").Replace("\\", "/");

            path = path.Replace("\\", "/");
            if (path.StartsWith(projectPath))
                path = path.Remove(0, projectPath.Length + 1);
            return path;
        }
    }
}
