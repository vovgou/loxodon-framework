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

using Loxodon.Framework.Security.Cryptography;
using System;
using System.Diagnostics;
using System.IO;

namespace Loxodon.Framework.XLua.Editors
{
    public class LuaCompiler
    {
        private string command;
        private IEncryptor encryptor;

        public LuaCompiler(string command) : this(command, null)
        {
        }

        public LuaCompiler(string command, IEncryptor encryptor)
        {
            this.command = command;
            this.encryptor = encryptor;
        }

        public void Compile(string inputFilename, string outputFilename, bool debug)
        {
            Compile(new FileInfo(inputFilename), new FileInfo(outputFilename), debug);
        }

        public void Compile(FileInfo inputFile, FileInfo outputFile, bool debug)
        {
            if (!inputFile.Exists)
            {
                UnityEngine.Debug.LogErrorFormat("Not found the file \"{0}\"", inputFile.FullName);
                return;
            }

            if (!outputFile.Directory.Exists)
                outputFile.Directory.Create();

            RunCMD(command, string.Format(" {0} -o \"{1}\" \"{2}\"", debug ? "" : "-s", outputFile.FullName, inputFile.FullName));

            if (this.encryptor != null && outputFile.Exists)
            {
                byte[] buffer = File.ReadAllBytes(outputFile.FullName);
                File.WriteAllBytes(outputFile.FullName, encryptor.Encrypt(buffer));
            }
        }

        public void Copy(FileInfo inputFile, FileInfo outputFile)
        {
            if (!inputFile.Exists)
            {
                UnityEngine.Debug.LogErrorFormat("Not found the file \"{0}\"", inputFile.FullName);
                return;
            }

            if (!outputFile.Directory.Exists)
                outputFile.Directory.Create();

            byte[] buffer = File.ReadAllBytes(inputFile.FullName);
            if (encryptor != null)
                buffer = encryptor.Encrypt(buffer);

            File.WriteAllBytes(outputFile.FullName, buffer);
        }

        public static void RunCMD(string command, string args)
        {
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = command;
                start.Arguments = args;

                start.RedirectStandardInput = true;
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;

                start.CreateNoWindow = true;
                start.ErrorDialog = true;
                start.UseShellExecute = false;

                Process process = Process.Start(start);
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();
                process.Close();

                if (!string.IsNullOrEmpty(output))
                    UnityEngine.Debug.Log(output);

                if (!string.IsNullOrEmpty(error))
                    UnityEngine.Debug.LogError(error);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }
    }
}