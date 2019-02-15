using Loxodon.Framework.Binding.Paths;
using System.Reflection;

namespace Loxodon.Framework.Binding.Proxy.Sources.Object
{
    public static class PathTokenExtension
    {
        public static MemberInfo GetMemberInfo(this PathToken token, object source)
        {
            IPathNode node = token.Current;
            var typeNode = node as TypeNode;
            if (typeNode != null)
                return null;

            var indexedNode = node as IndexedNode;
            if (indexedNode != null)
                return source.GetType().GetProperty("Item");

            var memberNode = node as MemberNode;
            if (memberNode == null)
                return null;

            var memberInfo = memberNode.MemberInfo;
            if (memberInfo == null)
                memberInfo = source.GetType().FindFirstMemberInfo(memberNode.Name);

            return memberInfo;
        }
    }
}
