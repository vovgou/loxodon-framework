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

using Loxodon.Framework.Binding.Paths;
using Loxodon.Framework.Binding.Proxy.Sources.Object;
using System.Collections.Generic;

namespace Loxodon.Framework.Binding.Proxy.Sources.Expressions
{
    public class LuaExpressionSourceProxyFactory : TypedSourceProxyFactory<LuaExpressionSourceDescription>
    {
        private ISourceProxyFactory factory;
        private IPathParser pathParser;
        public LuaExpressionSourceProxyFactory(ISourceProxyFactory factory, IPathParser pathParser)
        {
            this.factory = factory;
            this.pathParser = pathParser;
        }

        private List<Path> FindPaths(string[] textPaths)
        {
            List<Path> paths = new List<Path>();
            if (textPaths == null)
                return paths;

            for (int i = 0; i < textPaths.Length; i++)
            {
                Path path = this.pathParser.Parse(textPaths[i]);
                if (path != null && !paths.Contains(path))
                    paths.Add(path);
            }

            return paths;
        }

        protected override bool TryCreateProxy(object source, LuaExpressionSourceDescription description, out ISourceProxy proxy)
        {
            proxy = null;
            if (source == null && !description.IsStatic)
            {
                proxy = new EmptSourceProxy(description);
                return true;
            }

            List<ISourceProxy> list = new List<ISourceProxy>();
            if (!description.IsStatic)
            {
                List<Path> paths = FindPaths(description.Paths);
                foreach (Path path in paths)
                {
                    ISourceProxy innerProxy = this.factory.CreateProxy(source, new ObjectSourceDescription() { Path = path });
                    if (innerProxy != null)
                        list.Add(innerProxy);
                }
            }
            proxy = new LuaExpressionSourceProxy(source, description.Expression, list);
            return true;
        }
    }
}