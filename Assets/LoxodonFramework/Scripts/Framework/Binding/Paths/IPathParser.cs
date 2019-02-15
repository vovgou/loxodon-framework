using System.Linq.Expressions;

namespace Loxodon.Framework.Binding.Paths
{
    public interface IPathParser
    {
        /// <summary>
        /// Parser object instance path.eg:vm => vm.User.Username
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Path Parse(LambdaExpression expression);

        /// <summary>
        /// Parser text path.eg:User.Username
        /// </summary>
        /// <param name="pathText"></param>
        /// <returns></returns>
        Path Parse(string pathText);

        /// <summary>
        /// Parser static type path.
        /// </summary>
        /// <param name="pathText"></param>
        /// <returns></returns>
        Path ParseStaticPath(string pathText);

        /// <summary>
        /// Parser static type path.eg:System.Int32.MaxValue
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Path ParseStaticPath(LambdaExpression expression);

        /// <summary>
        /// Parser member name.eg:vm => vm.User
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        string ParseMemberName(LambdaExpression expression);
    }
}
