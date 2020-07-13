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

using System.IO;
using UnityEditor;

namespace Loxodon.Framework.Editors
{
    public static class FrameworkMenus
    {
        private static string GetPackageFullPath()
        {
            string packagePath = Path.GetFullPath("Packages/com.vovgou.loxodon-framework");
            if (Directory.Exists(packagePath))
                return packagePath;

            packagePath = Path.GetFullPath("Assets/..");
            if (Directory.Exists(packagePath))
            {
                if (Directory.Exists(packagePath + "/Assets/Packages/com.vovgou.loxodon-framework/Extras~"))
                    return packagePath + "/Assets/Packages/com.vovgou.loxodon-framework";

                if (Directory.Exists(packagePath + "/Assets/LoxodonFramework/Extras~"))
                    return packagePath + "/Assets/LoxodonFramework";

                return null;
            }
            return null;
        }

        [MenuItem("Tools/Loxodon/Samples/Import Examples", false, 2000)]
        private static void ImportExamples()
        {
            string packageFullPath = GetPackageFullPath();
            AssetDatabase.ImportPackage(packageFullPath + "/Extras~/Examples.unitypackage", true);
        }

        [MenuItem("Tools/Loxodon/Samples/Import Tutorials", false, 2001)]
        private static void ImportTutorials()
        {
            string packageFullPath = GetPackageFullPath();
            AssetDatabase.ImportPackage(packageFullPath + "/Extras~/Tutorials.unitypackage", true);
        }
    }
}
