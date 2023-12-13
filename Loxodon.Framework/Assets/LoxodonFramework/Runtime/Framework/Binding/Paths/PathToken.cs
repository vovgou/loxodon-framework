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
using System.Text;

namespace Loxodon.Framework.Binding.Paths
{
    [Serializable]
    public struct PathToken
    {
        private Path path;
        private int index;
        public PathToken(Path path, int index)
        {
            this.path = path;
            this.index = index;
        }

        public Path Path
        {
            get { return this.path; }
        }

        public int Index { get { return this.index; } }

        public IPathNode Current
        {
            get { return path[index]; }
        }

        public bool HasNext()
        {
            if (index + 1 < path.Count)
                return true;
            return false;
        }

        public PathToken NextToken()
        {
            if (!HasNext())
                throw new IndexOutOfRangeException();
            return new PathToken(path, index + 1);
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            this.Current.ToString();
            buf.Append(this.Current.ToString()).Append(" Index:").Append(this.index);
            return buf.ToString();
        }
    }
}
