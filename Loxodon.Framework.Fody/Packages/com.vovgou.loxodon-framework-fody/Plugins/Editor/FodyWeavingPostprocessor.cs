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

using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;

namespace Loxodon.Framework.Fody
{
    public class FodyWeavingPostprocessor : IPostBuildPlayerScriptDLLs
    {
        private const string ASSEMBLIES_EDITOR_LIB_PATH = "Library/ScriptAssemblies/";
        private const string ASSEMBLIES_BUILD_TEMP_PATH = "Temp/StagingArea/Data/Managed/";
        public int callbackOrder => 0;

        [DidReloadScripts]
        public static void OnScriptsReloaded()
        {
            FodyWeaver.Default.Weave(ASSEMBLIES_EDITOR_LIB_PATH);
        }

        public void OnPostBuildPlayerScriptDLLs(BuildReport report)
        {
            FodyWeaver.Default.Weave(ASSEMBLIES_BUILD_TEMP_PATH);
        }
    }
}
