using System;
using System.Reflection;

using Loxodon.Log;

namespace Loxodon.Framework.Binding
{
    public static class TypeExtensions
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(TypeExtensions));

        public static MemberInfo FindFirstMemberInfo(this Type type, string name)
        {
            var members = type.GetMember(name);
            if (members == null || members.Length <= 0)
                return null;
            return members[0];
        }

        public static MemberInfo FindFirstMemberInfo(this Type type, string name, BindingFlags flags)
        {
            var members = type.GetMember(name, flags);
            if (members == null || members.Length <= 0)
                return null;
            return members[0];
        }
    }
}
