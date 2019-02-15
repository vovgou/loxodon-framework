using System;

namespace Loxodon.Framework.Prefs
{
    public interface ISerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="encoder"></param>
        void AddTypeEncoder(ITypeEncoder encoder);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encoder"></param>
        void RemoveTypeEncoder(ITypeEncoder encoder);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type"></param>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        object Deserialize(string input, Type type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        string Serialize(object value);
    }
}
