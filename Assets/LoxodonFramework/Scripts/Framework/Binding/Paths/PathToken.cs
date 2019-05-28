using System;
using System.Text;

namespace Loxodon.Framework.Binding.Paths
{
    [Serializable]
    public class PathToken
    {
        private Path path;
        private int pathIndex;

        private PathToken nextToken;
        public PathToken(Path path, int pathIndex)
        {
            this.path = path;
            this.pathIndex = pathIndex;
        }

        public Path Path
        {
            get { return this.path; }
        }

        public int Index { get { return this.pathIndex; } }

        public IPathNode Current
        {
            get { return path[pathIndex]; }
        }

        public bool HasNext()
        {
            if (path.Count <= 0 || this.pathIndex >= path.Count - 1)
                return false;
            return true;
        }

        public PathToken NextToken()
        {
            if (!HasNext())
                throw new IndexOutOfRangeException();

            if (this.nextToken == null)
                this.nextToken = new PathToken(path, pathIndex + 1);
            return this.nextToken;
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            this.Current.ToString();
            buf.Append(this.Current.ToString()).Append(" pathIndex:").Append(this.pathIndex);
            return buf.ToString();
        }
    }
}
